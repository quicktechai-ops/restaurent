using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;

namespace Restaurant.API.Controllers.SuperAdmin;

[ApiController]
[Route("api/superadmin/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var companies = await _context.Companies.ToListAsync();
        var payments = await _context.CompanyPayments.ToListAsync();
        var branches = await _context.Branches.CountAsync();
        var users = await _context.Users.CountAsync();

        var recentCompanies = await _context.Companies
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new RecentCompanyDto
            {
                Id = c.CompanyId,
                Name = c.Name,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        var recentBillings = await _context.CompanyPayments
            .Include(p => p.Company)
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new RecentBillingDto
            {
                Id = p.PaymentId,
                CompanyName = p.Company.Name,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync();

        return Ok(new DashboardResponse
        {
            TotalCompanies = companies.Count,
            ActiveCompanies = companies.Count(c => c.Status == "active"),
            InactiveCompanies = companies.Count(c => c.Status == "inactive"),
            SuspendedCompanies = companies.Count(c => c.Status == "suspended"),
            TotalIncome = payments.Where(p => p.Status == "completed").Sum(p => p.Amount),
            TotalBranches = branches,
            TotalUsers = users,
            RecentCompanies = recentCompanies,
            RecentBillings = recentBillings
        });
    }
}
