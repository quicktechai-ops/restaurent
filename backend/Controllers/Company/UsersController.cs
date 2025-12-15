using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;
using Restaurant.API.Services;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public UsersController(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<UserListDto>>> GetAll([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();

        var query = _context.Users
            .Include(u => u.DefaultBranch)
            .Where(u => u.CompanyId == companyId && u.DeletedAt == null);

        if (branchId.HasValue)
        {
            query = query.Where(u => u.DefaultBranchId == branchId.Value);
        }

        var users = await query
            .Select(u => new UserListDto
            {
                Id = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                DefaultBranchId = u.DefaultBranchId,
                DefaultBranchName = u.DefaultBranch != null ? u.DefaultBranch.Name : null,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.UserId)
                    .Select(ur => ur.Role.Name)
                    .ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var user = await _context.Users
            .Include(u => u.DefaultBranch)
            .Where(u => u.UserId == id && u.CompanyId == companyId && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound(new { message = "User not found" });

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        return Ok(new UserListDto
        {
            Id = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            DefaultBranchId = user.DefaultBranchId,
            DefaultBranchName = user.DefaultBranch?.Name,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            Roles = roles
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserListDto>> Create([FromBody] CreateUserRequest request)
    {
        var companyId = GetCompanyId();

        // Check user limit
        var company = await _context.Companies.FindAsync(companyId);
        var currentUsers = await _context.Users.CountAsync(u => u.CompanyId == companyId && u.DeletedAt == null);
        
        if (currentUsers >= company!.MaxUsers)
            return BadRequest(new { message = $"Maximum users limit ({company.MaxUsers}) reached. Upgrade your plan." });

        // Check if username exists
        var existing = await _context.Users
            .AnyAsync(u => u.CompanyId == companyId && u.Username == request.Username);

        if (existing)
            return BadRequest(new { message = "Username already exists" });

        var user = new User
        {
            CompanyId = companyId,
            Username = request.Username,
            PasswordHash = _authService.HashPassword(request.Password),
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            DefaultBranchId = request.DefaultBranchId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assign roles
        foreach (var roleId in request.RoleIds)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.UserId }, new UserListDto
        {
            Id = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var companyId = GetCompanyId();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.CompanyId == companyId && u.DeletedAt == null);

        if (user == null)
            return NotFound(new { message = "User not found" });

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.DefaultBranchId = request.DefaultBranchId;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Update roles
        var existingRoles = await _context.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
        _context.UserRoles.RemoveRange(existingRoles);

        foreach (var roleId in request.RoleIds)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = id,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "User updated successfully" });
    }

    [HttpPatch("{id}/reset-password")]
    public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetUserPasswordRequest request)
    {
        var companyId = GetCompanyId();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.CompanyId == companyId && u.DeletedAt == null);

        if (user == null)
            return NotFound(new { message = "User not found" });

        user.PasswordHash = _authService.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.CompanyId == companyId && u.DeletedAt == null);

        if (user == null)
            return NotFound(new { message = "User not found" });

        // Soft delete
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "User deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.CompanyId == companyId && u.DeletedAt == null);

        if (user == null)
            return NotFound(new { message = "User not found" });

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"User is now {(user.IsActive ? "active" : "inactive")}" });
    }
}
