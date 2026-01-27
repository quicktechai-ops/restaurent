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
        var dateFrom = from ?? DateTime.UtcNow.AddDays(-30);
        var dateTo = to?.AddDays(1) ?? DateTime.UtcNow.AddDays(1);

        var orders = await _context.Orders
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.MenuItem)
                    .ThenInclude(mi => mi.Category)
            .Where(o => o.CompanyId == companyId && o.PaymentStatus == "Paid" && o.CreatedAt >= dateFrom && o.CreatedAt < dateTo)
            .ToListAsync();

        var totalRevenue = orders.Sum(o => o.GrandTotal);
        var totalOrders = orders.Count;
        var averageTicket = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var uniqueCustomers = orders.Where(o => o.CustomerId.HasValue).Select(o => o.CustomerId).Distinct().Count();

        // Sales by category
        var allLines = orders.SelectMany(o => o.OrderLines).ToList();
        var byCategory = allLines
            .Where(ol => ol.MenuItem?.Category != null)
            .GroupBy(ol => ol.MenuItem!.Category!.Name)
            .Select(g => new
            {
                name = g.Key,
                quantity = g.Sum(ol => ol.Quantity),
                revenue = g.Sum(ol => ol.LineNet),
                percentage = totalRevenue > 0 ? (g.Sum(ol => ol.LineNet) / totalRevenue * 100) : 0
            })
            .OrderByDescending(c => c.revenue)
            .ToList();

        // Top selling items
        var topItems = allLines
            .Where(ol => ol.MenuItem != null)
            .GroupBy(ol => ol.MenuItem!.Name)
            .Select(g => new
            {
                name = g.Key,
                quantity = g.Sum(ol => ol.Quantity),
                revenue = g.Sum(ol => ol.LineNet)
            })
            .OrderByDescending(i => i.quantity)
            .Take(10)
            .ToList();

        return Ok(new
        {
            totalRevenue,
            totalOrders,
            averageTicket,
            uniqueCustomers,
            byCategory,
            topItems
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
        var dateFrom = from ?? DateTime.UtcNow.AddDays(-30);
        var dateTo = to?.AddDays(1) ?? DateTime.UtcNow.AddDays(1);

        var totalCustomers = await _context.Customers.CountAsync(c => c.CompanyId == companyId);
        
        var newCustomers = await _context.Customers
            .CountAsync(c => c.CompanyId == companyId && c.CreatedAt >= dateFrom && c.CreatedAt < dateTo);

        // Get customer order stats
        var customerOrders = await _context.Orders
            .Where(o => o.CompanyId == companyId && o.CustomerId.HasValue && o.PaymentStatus == "Paid")
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(o => o.GrandTotal),
                LastVisit = g.Max(o => o.CreatedAt)
            })
            .ToListAsync();

        var customersWithMultipleOrders = customerOrders.Count(c => c.OrderCount > 1);
        var repeatRate = customerOrders.Count > 0 ? (decimal)customersWithMultipleOrders / customerOrders.Count * 100 : 0;

        var topCustomerIds = customerOrders.OrderByDescending(c => c.TotalSpent).Take(10).ToList();
        var customerNames = await _context.Customers
            .Where(c => topCustomerIds.Select(t => t.CustomerId).Contains(c.CustomerId))
            .ToDictionaryAsync(c => c.CustomerId, c => c.Name);

        var topCustomers = topCustomerIds.Select(c => new
        {
            id = c.CustomerId,
            name = customerNames.GetValueOrDefault(c.CustomerId ?? 0, "Unknown"),
            orderCount = c.OrderCount,
            totalSpent = c.TotalSpent,
            lastVisit = c.LastVisit
        }).ToList();

        return Ok(new
        {
            totalCustomers,
            newCustomers,
            repeatRate,
            topCustomers
        });
    }
}
