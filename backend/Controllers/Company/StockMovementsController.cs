using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/stock-movements")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class StockMovementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockMovementsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int? itemId,
        [FromQuery] string? type,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var companyId = GetCompanyId();
        var query = _context.StockMovements
            .Include(sm => sm.InventoryItem)
            .Where(sm => sm.CompanyId == companyId);

        if (itemId.HasValue)
            query = query.Where(sm => sm.InventoryItemId == itemId.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(sm => sm.MovementType == type);

        if (dateFrom.HasValue)
            query = query.Where(sm => sm.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(sm => sm.CreatedAt <= dateTo.Value.AddDays(1));

        var movements = await query
            .OrderByDescending(sm => sm.CreatedAt)
            .Take(500)
            .Select(sm => new
            {
                sm.Id,
                sm.InventoryItemId,
                ItemName = sm.InventoryItem != null ? sm.InventoryItem.Name : null,
                Unit = sm.InventoryItem != null ? sm.InventoryItem.UnitOfMeasure : null,
                sm.MovementType,
                sm.Quantity,
                sm.UnitCost,
                sm.Reference,
                sm.Notes,
                sm.CreatedAt
            })
            .ToListAsync();

        return Ok(movements);
    }
}
