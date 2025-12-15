using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Data;
using Restaurant.API.DTOs;

namespace Restaurant.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginSuperAdminAsync(LoginRequest request)
    {
        var admin = await _context.SuperAdmins
            .FirstOrDefaultAsync(s => s.Username == request.Username && s.IsActive);

        if (admin == null || !VerifyPassword(request.Password, admin.PasswordHash))
            return null;

        // Update last login
        admin.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateToken(admin.SuperAdminId, admin.Username, admin.FullName, null, "SuperAdmin", true);

        return new LoginResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = admin.SuperAdminId,
                Name = admin.FullName,
                Username = admin.Username,
                Email = admin.Email,
                IsSuperAdmin = true,
                Role = "SuperAdmin"
            }
        };
    }

    public async Task<LoginResponse?> LoginCompanyAsync(LoginRequest request)
    {
        var company = await _context.Companies
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.Username == request.Username && c.Status == "active");

        if (company == null || !VerifyPassword(request.Password, company.PasswordHash))
            return null;

        // Check plan expiry
        if (company.PlanExpiryDate.HasValue && company.PlanExpiryDate < DateTime.UtcNow)
        {
            return null; // Plan expired
        }

        var token = GenerateToken(company.CompanyId, company.Username, company.Name, company.CompanyId, "CompanyAdmin", false);

        return new LoginResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = company.CompanyId,
                Name = company.Name,
                Username = company.Username,
                Email = company.Email,
                CompanyId = company.CompanyId,
                CompanyName = company.Name,
                IsSuperAdmin = false,
                Role = "CompanyAdmin"
            }
        };
    }

    public async Task<LoginResponse?> LoginUserAsync(LoginRequest request, int companyId)
    {
        var user = await _context.Users
            .Include(u => u.Company)
            .Include(u => u.DefaultBranch)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.CompanyId == companyId && u.IsActive);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        // Check company status
        if (user.Company.Status != "active")
            return null;

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateToken(user.UserId, user.Username, user.FullName, user.CompanyId, "User", false);

        return new LoginResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = user.UserId,
                Name = user.FullName,
                Username = user.Username,
                Email = user.Email,
                CompanyId = user.CompanyId,
                CompanyName = user.Company.Name,
                BranchId = user.DefaultBranchId,
                BranchName = user.DefaultBranch?.Name,
                IsSuperAdmin = false,
                Role = "User"
            }
        };
    }

    public byte[] HashPassword(string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        return Encoding.UTF8.GetBytes(hash);
    }

    public bool VerifyPassword(string password, byte[] hash)
    {
        try
        {
            var hashString = Encoding.UTF8.GetString(hash);
            return BCrypt.Net.BCrypt.Verify(password, hashString);
        }
        catch
        {
            return false;
        }
    }

    private string GenerateToken(int userId, string username, string name, int? companyId, string role, bool isSuperAdmin)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new("username", username),
            new(ClaimTypes.Role, role),
            new("is_superadmin", isSuperAdmin.ToString().ToLower())
        };

        if (companyId.HasValue)
        {
            claims.Add(new Claim("company_id", companyId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:ExpirationDays"] ?? "7"));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
