using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;
using CompanyModel = Restaurant.API.Models.Company;
using Restaurant.API.Services;

namespace Restaurant.API.Controllers.SuperAdmin;

[ApiController]
[Route("api/superadmin/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public CompaniesController(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyListDto>>> GetAll([FromQuery] string? search, [FromQuery] string? status)
    {
        var query = _context.Companies
            .Include(c => c.Plan)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => 
                c.Name.ToLower().Contains(search.ToLower()) ||
                c.Username.ToLower().Contains(search.ToLower()) ||
                (c.Phone != null && c.Phone.Contains(search)));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(c => c.Status == status);
        }

        var companies = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CompanyListDto
            {
                Id = c.CompanyId,
                Name = c.Name,
                Username = c.Username,
                Phone = c.Phone,
                Address = c.Address,
                Status = c.Status,
                PlanId = c.PlanId,
                PlanName = c.Plan != null ? c.Plan.Name : null,
                PlanExpiryDate = c.PlanExpiryDate,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyListDto>> GetById(int id)
    {
        var company = await _context.Companies
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.CompanyId == id);

        if (company == null)
            return NotFound(new { message = "Company not found" });

        return Ok(new CompanyListDto
        {
            Id = company.CompanyId,
            Name = company.Name,
            Username = company.Username,
            Phone = company.Phone,
            Address = company.Address,
            Status = company.Status,
            PlanId = company.PlanId,
            PlanName = company.Plan?.Name,
            PlanExpiryDate = company.PlanExpiryDate,
            CreatedAt = company.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<CompanyListDto>> Create([FromBody] CreateCompanyRequest request)
    {
        // Check if username already exists
        if (await _context.Companies.AnyAsync(c => c.Username == request.Username))
            return BadRequest(new { message = "Username already exists" });

        var superAdminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var company = new CompanyModel
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = _authService.HashPassword(request.Password),
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            PlanId = request.PlanId,
            PlanExpiryDate = request.PlanExpiryDate,
            Status = request.Status,
            Notes = request.Notes,
            CreatedBySuperAdminId = superAdminId,
            CreatedAt = DateTime.UtcNow
        };

        // If plan is selected, set max branches/users from plan
        if (request.PlanId.HasValue)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(request.PlanId.Value);
            if (plan != null)
            {
                company.MaxBranches = plan.MaxBranches;
                company.MaxUsers = plan.MaxUsers;
            }
        }

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Create default branch
        var branch = new Branch
        {
            CompanyId = company.CompanyId,
            Name = "Main Branch",
            Code = "MAIN",
            DefaultCurrencyCode = "USD",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = company.CompanyId }, new CompanyListDto
        {
            Id = company.CompanyId,
            Name = company.Name,
            Username = company.Username,
            Phone = company.Phone,
            Address = company.Address,
            Status = company.Status,
            PlanId = company.PlanId,
            CreatedAt = company.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCompanyRequest request)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound(new { message = "Company not found" });

        company.Name = request.Name;
        company.Email = request.Email;
        company.Phone = request.Phone;
        company.Address = request.Address;
        company.PlanId = request.PlanId;
        company.PlanExpiryDate = request.PlanExpiryDate;
        company.Status = request.Status;
        company.Notes = request.Notes;
        company.UpdatedAt = DateTime.UtcNow;

        // Update password if provided
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            company.PasswordHash = _authService.HashPassword(request.NewPassword);
        }

        // Update max branches/users from plan
        if (request.PlanId.HasValue)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(request.PlanId.Value);
            if (plan != null)
            {
                company.MaxBranches = plan.MaxBranches;
                company.MaxUsers = plan.MaxUsers;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Company updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound(new { message = "Company not found" });

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Company deleted successfully" });
    }

    [HttpPatch("{id}/reset-password")]
    public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound(new { message = "Company not found" });

        company.PasswordHash = _authService.HashPassword(request.NewPassword);
        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully" });
    }

    [HttpPut("{id}/toggle-status")]
    public async Task<ActionResult> ToggleStatus(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound(new { message = "Company not found" });

        company.Status = company.Status == "active" ? "suspended" : "active";
        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Company status changed to {company.Status}" });
    }
}
