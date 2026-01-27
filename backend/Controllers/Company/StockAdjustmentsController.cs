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
        try
        {
            var adjustments = await _context.StockAdjustments
                .Include(sa => sa.Branch)
                .Include(sa => sa.User)
                .Include(sa => sa.InventoryItem)
                .OrderByDescending(sa => sa.AdjustmentDate)
                .Take(500)
                .ToListAsync();

            var result = adjustments.Select(sa => new
            {
                sa.Id,
                sa.BranchId,
                BranchName = sa.Branch?.Name,
                sa.InventoryItemId,
                ItemName = sa.InventoryItem?.Name ?? "Unknown",
                Unit = sa.InventoryItem?.UnitOfMeasure ?? "",
                sa.AdjustmentType,
                sa.Quantity,
                sa.QuantityBefore,
                sa.QuantityAfter,
                sa.Reason,
                sa.Status,
                sa.Notes,
                AdjustedBy = sa.User?.FullName ?? "System",
                CreatedAt = sa.AdjustmentDate
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateStockAdjustmentRequest request)
    {
        try
        {
            // Get the inventory item
            var inventoryItem = await _context.InventoryItems.FindAsync(request.InventoryItemId);
            if (inventoryItem == null)
                return BadRequest(new { message = "Inventory item not found" });

            var quantityBefore = inventoryItem.Quantity;
            decimal quantityAfter;

            // Calculate new quantity
            if (request.AdjustmentType == "increase")
                quantityAfter = quantityBefore + request.Quantity;
            else if (request.AdjustmentType == "decrease")
                quantityAfter = quantityBefore - request.Quantity;
            else // set
                quantityAfter = request.Quantity;

            var adjustment = new StockAdjustment
            {
                BranchId = null,
                InventoryItemId = request.InventoryItemId,
                AdjustmentType = request.AdjustmentType,
                Quantity = request.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = quantityAfter,
                Reason = request.Reason,
                Status = "Completed",
                Notes = request.Notes,
                UserId = null,
                AdjustmentDate = DateTime.UtcNow
            };

            _context.StockAdjustments.Add(adjustment);

            // Update inventory quantity
            inventoryItem.Quantity = quantityAfter;

            // Create stock movement record
            var movementType = request.AdjustmentType == "increase" ? "IN-Adjustment" : "OUT-Adjustment";
            var movementQty = request.AdjustmentType == "increase" ? request.Quantity : -request.Quantity;
            
            var stockMovement = new StockMovement
            {
                CompanyId = inventoryItem.CompanyId,
                BranchId = null,
                InventoryItemId = request.InventoryItemId,
                MovementType = movementType,
                Quantity = movementQty,
                UnitCost = inventoryItem.Cost,
                Reference = $"ADJ-{DateTime.UtcNow:yyyyMMdd}",
                Notes = $"{request.Reason}: {request.Notes}",
                CreatedBy = 0, // System
                CreatedAt = DateTime.UtcNow
            };
            _context.StockMovements.Add(stockMovement);

            await _context.SaveChangesAsync();

            return Ok(new { adjustment.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
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
