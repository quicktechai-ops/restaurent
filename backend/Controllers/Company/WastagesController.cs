using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/wastages")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class WastagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WastagesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var companyId = GetCompanyId();
        var wastages = await _context.Wastages
            .Include(w => w.InventoryItem)
            .Where(w => w.CompanyId == companyId)
            .OrderByDescending(w => w.CreatedAt)
            .Take(500)
            .Select(w => new
            {
                w.Id,
                w.InventoryItemId,
                ItemName = w.InventoryItem != null ? w.InventoryItem.Name : null,
                Unit = w.InventoryItem != null ? w.InventoryItem.UnitOfMeasure : null,
                w.Quantity,
                w.Reason,
                w.CostImpact,
                w.Notes,
                RecordedBy = "User", // TODO: Get user name
                w.CreatedAt
            })
            .ToListAsync();

        return Ok(wastages);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateWastageRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var item = await _context.InventoryItems.FindAsync(request.InventoryItemId);
        if (item == null || item.CompanyId != companyId) return NotFound("Item not found");

        var costImpact = request.Quantity * item.Cost;

        var wastage = new Wastage
        {
            CompanyId = companyId,
            InventoryItemId = request.InventoryItemId,
            Quantity = request.Quantity,
            UnitCost = item.Cost,
            CostImpact = costImpact,
            Reason = request.Reason,
            Notes = request.Notes,
            RecordedBy = userId
        };

        // Update inventory quantity
        item.Quantity -= request.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        // Create stock movement
        var movement = new StockMovement
        {
            CompanyId = companyId,
            InventoryItemId = request.InventoryItemId,
            MovementType = "OUT-Waste",
            Quantity = -request.Quantity,
            UnitCost = item.Cost,
            Reference = $"Wastage: {request.Reason}",
            Notes = request.Notes,
            CreatedBy = userId
        };

        _context.Wastages.Add(wastage);
        _context.StockMovements.Add(movement);
        await _context.SaveChangesAsync();

        return Ok(new { wastage.Id });
    }
}

public class CreateWastageRequest
{
    public int InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
