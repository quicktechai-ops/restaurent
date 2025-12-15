using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/stock-adjustments")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StockAdjustmentsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var adjustments = await _context.StockAdjustments
            .Include(sa => sa.Branch)
            .Include(sa => sa.User)
            .OrderByDescending(sa => sa.AdjustmentDate)
            .Take(500)
            .Select(sa => new
            {
                sa.Id,
                sa.BranchId,
                BranchName = sa.Branch != null ? sa.Branch.Name : null,
                sa.Status,
                sa.Notes,
                AdjustedBy = sa.User != null ? sa.User.FullName : "User",
                sa.AdjustmentDate
            })
            .ToListAsync();

        return Ok(adjustments);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateStockAdjustmentRequest request)
    {
        var userId = GetUserId();

        // Get the inventory item
        var inventoryItem = await _context.InventoryItems.FindAsync(request.InventoryItemId);
        if (inventoryItem == null)
            return BadRequest(new { message = "Inventory item not found" });

        var adjustment = new StockAdjustment
        {
            BranchId = null, // Stock adjustments are at company level
            Status = "Completed",
            Notes = $"{request.AdjustmentType}: {request.Quantity} - {request.Reason}. {request.Notes}",
            UserId = userId,
            AdjustmentDate = DateTime.UtcNow
        };

        _context.StockAdjustments.Add(adjustment);

        // Update inventory quantity
        if (request.AdjustmentType == "increase")
            inventoryItem.Quantity += request.Quantity;
        else
            inventoryItem.Quantity -= request.Quantity;

        await _context.SaveChangesAsync();

        return Ok(new { adjustment.Id });
    }
}

public class CreateStockAdjustmentRequest
{
    public int InventoryItemId { get; set; }
    public string AdjustmentType { get; set; } = "increase";
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
