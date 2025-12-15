using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/stock-counts")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class StockCountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockCountsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var counts = await _context.StockCounts
            .OrderByDescending(sc => sc.CountDate)
            .Select(sc => new
            {
                sc.Id,
                sc.Area,
                sc.Status,
                sc.CountDate
            })
            .ToListAsync();

        return Ok(counts);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateStockCountRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var count = new StockCount
        {
            Area = request.Name ?? "General",
            Status = "Completed",
            CountDate = DateTime.UtcNow,
            BranchId = request.BranchId
        };

        _context.StockCounts.Add(count);
        await _context.SaveChangesAsync();

        return Ok(new { count.Id });
    }
}

public class CreateStockCountRequest
{
    public string? Name { get; set; }
    public int? BranchId { get; set; }
}
