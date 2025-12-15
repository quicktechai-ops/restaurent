using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using System.Security.Claims;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,User")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<CompanyDashboardDto>> GetDashboard()
    {
        var companyId = GetCompanyId();

        var company = await _context.Companies
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);

        if (company == null)
            return NotFound(new { message = "Company not found" });

        var totalBranches = await _context.Branches.CountAsync(b => b.CompanyId == companyId && b.DeletedAt == null);
        var totalUsers = await _context.Users.CountAsync(u => u.CompanyId == companyId && u.DeletedAt == null);
        var totalMenuItems = await _context.MenuItems.CountAsync(m => m.CompanyId == companyId);
        var totalCategories = await _context.Categories.CountAsync(c => c.CompanyId == companyId);
        var totalTables = await _context.RestaurantTables
            .Include(t => t.Branch)
            .CountAsync(t => t.Branch.CompanyId == companyId);

        return Ok(new CompanyDashboardDto
        {
            TotalBranches = totalBranches,
            TotalUsers = totalUsers,
            TotalMenuItems = totalMenuItems,
            TotalCategories = totalCategories,
            TotalTables = totalTables,
            CompanyName = company.Name,
            PlanName = company.Plan?.Name ?? "No Plan",
            PlanExpiryDate = company.PlanExpiryDate,
            MaxBranches = company.MaxBranches,
            MaxUsers = company.MaxUsers
        });
    }
}
