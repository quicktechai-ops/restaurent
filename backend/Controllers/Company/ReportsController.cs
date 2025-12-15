using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/reports")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet("sales")]
    public async Task<ActionResult> GetSalesReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var companyId = GetCompanyId();
        // TODO: Implement when Orders are available
        // For now, return mock data structure
        return Ok(new
        {
            totalRevenue = 0m,
            totalOrders = 0,
            averageTicket = 0m,
            uniqueCustomers = 0,
            byCategory = new List<object>(),
            topItems = new List<object>()
        });
    }

    [HttpGet("inventory")]
    public async Task<ActionResult> GetInventoryReport()
    {
        var companyId = GetCompanyId();
        
        var items = await _context.InventoryItems
            .Where(i => i.CompanyId == companyId && i.IsActive)
            .Select(i => new
            {
                Id = i.InventoryItemId,
                i.Name,
                i.Quantity,
                i.MinLevel,
                i.Cost,
                TotalValue = i.Quantity * i.Cost,
                Unit = i.UnitOfMeasure
            })
            .ToListAsync();

        var totalItems = items.Count;
        var lowStockItems = items.Count(i => i.Quantity <= i.MinLevel);
        var totalValue = items.Sum(i => i.TotalValue);

        return Ok(new
        {
            totalItems,
            lowStockItems,
            totalValue,
            items
        });
    }

    [HttpGet("customers")]
    public async Task<ActionResult> GetCustomersReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var companyId = GetCompanyId();

        var totalCustomers = await _context.Customers.CountAsync(c => c.CompanyId == companyId);
        
        var dateFrom = from ?? DateTime.UtcNow.AddDays(-30);
        var dateTo = to ?? DateTime.UtcNow;

        var newCustomers = await _context.Customers
            .CountAsync(c => c.CompanyId == companyId && c.CreatedAt >= dateFrom && c.CreatedAt <= dateTo);

        // TODO: Calculate repeat rate and top customers when Orders are available
        var topCustomers = await _context.Customers
            .Where(c => c.CompanyId == companyId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .Select(c => new
            {
                c.CustomerId,
                c.Name,
                OrderCount = 0,
                TotalSpent = 0m,
                LastVisit = c.UpdatedAt ?? c.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            totalCustomers,
            newCustomers,
            repeatRate = 0m,
            topCustomers
        });
    }
}
