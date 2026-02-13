using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/goods-receipts")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class GoodsReceiptController : ControllerBase
{
    private readonly AppDbContext _context;

    public GoodsReceiptController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var companyId = GetCompanyId();
        var receipts = await _context.GoodsReceipts
            .Include(gr => gr.Supplier)
            .Include(gr => gr.Lines)
            .Where(gr => gr.Supplier != null && gr.Supplier.CompanyId == companyId)
            .OrderByDescending(gr => gr.GRNDate)
            .Select(gr => new
            {
                gr.Id,
                gr.ReceiptNumber,
                gr.PurchaseOrderId,
                SupplierName = gr.Supplier != null ? gr.Supplier.Name : null,
                TotalAmount = gr.GrandTotal,
                gr.Status,
                CreatedAt = gr.GRNDate,
                LineCount = gr.Lines.Count
            })
            .ToListAsync();

        return Ok(receipts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var gr = await _context.GoodsReceipts
            .Include(g => g.Supplier)
            .Include(g => g.Lines)
                .ThenInclude(l => l.InventoryItem)
            .FirstOrDefaultAsync(g => g.Id == id && g.Supplier != null && g.Supplier.CompanyId == companyId);

        if (gr == null) return NotFound();

        return Ok(new
        {
            gr.Id,
            gr.ReceiptNumber,
            gr.PurchaseOrderId,
            gr.SupplierId,
            SupplierName = gr.Supplier?.Name,
            gr.Status,
            CreatedAt = gr.GRNDate,
            gr.TotalBeforeTax,
            gr.TaxAmount,
            gr.GrandTotal,
            Lines = gr.Lines.Select(l => new
            {
                l.Id,
                l.InventoryItemId,
                ItemName = l.InventoryItem?.Name,
                Unit = l.InventoryItem?.UnitOfMeasure,
                l.ReceivedQuantity,
                l.UnitCost,
                l.TotalCost
            })
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateGoodsReceiptRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        // Load the PO with lines
        PurchaseOrder? po = null;
        if (request.PurchaseOrderId.HasValue)
        {
            po = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Id == request.PurchaseOrderId.Value
                    && p.Supplier != null && p.Supplier.CompanyId == companyId);

            if (po == null)
                return BadRequest(new { message = "Purchase order not found" });

            if (po.Status != "Approved" && po.Status != "PartiallyReceived")
                return BadRequest(new { message = "Purchase order must be Approved or Partially Received" });
        }

        if (request.Lines == null || !request.Lines.Any())
            return BadRequest(new { message = "At least one line is required" });

        // Create receipt
        var receipt = new GoodsReceipt
        {
            PurchaseOrderId = request.PurchaseOrderId,
            SupplierId = po?.SupplierId ?? 0,
            BranchId = po?.BranchId,
            ReceiptNumber = $"GRN-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = "Received",
            GRNDate = DateTime.UtcNow,
            TotalBeforeTax = 0,
            TaxAmount = 0,
            GrandTotal = 0
        };

        foreach (var line in request.Lines)
        {
            if (line.ReceivedQuantity <= 0) continue;

            var grLine = new GoodsReceiptLine
            {
                InventoryItemId = line.InventoryItemId,
                ReceivedQuantity = line.ReceivedQuantity,
                UnitCost = line.UnitCost,
                TotalCost = line.ReceivedQuantity * line.UnitCost
            };
            receipt.Lines.Add(grLine);

            // Update inventory stock and cost
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.InventoryItemId == line.InventoryItemId && i.CompanyId == companyId);

            if (inventoryItem != null)
            {
                // Weighted average cost
                if (inventoryItem.CostMethod == "Average" && (inventoryItem.Quantity + line.ReceivedQuantity) > 0)
                {
                    var totalValue = (inventoryItem.Quantity * inventoryItem.Cost) + (line.ReceivedQuantity * line.UnitCost);
                    var totalQty = inventoryItem.Quantity + line.ReceivedQuantity;
                    inventoryItem.Cost = totalValue / totalQty;
                }
                else
                {
                    inventoryItem.Cost = line.UnitCost;
                }

                inventoryItem.Quantity += line.ReceivedQuantity;
                inventoryItem.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"[INVENTORY] Received {line.ReceivedQuantity} {inventoryItem.UnitOfMeasure} of {inventoryItem.Name} — new stock: {inventoryItem.Quantity}");

                // Stock movement audit trail
                _context.StockMovements.Add(new StockMovement
                {
                    CompanyId = companyId,
                    BranchId = po?.BranchId,
                    InventoryItemId = line.InventoryItemId,
                    MovementType = "IN-Purchase",
                    Quantity = line.ReceivedQuantity,
                    UnitCost = line.UnitCost,
                    Reference = receipt.ReceiptNumber,
                    Notes = po != null ? $"PO: {po.OrderNumber}" : null,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Update PO line received quantity (use OrderedQuantity if provided, so loss doesn't leave a remainder)
            if (po != null)
            {
                var poLine = po.Lines.FirstOrDefault(l => l.InventoryItemId == line.InventoryItemId);
                if (poLine != null)
                {
                    var qtyToMark = line.OrderedQuantity > 0 ? line.OrderedQuantity : line.ReceivedQuantity;
                    poLine.ReceivedQuantity += qtyToMark;
                }
            }
        }

        receipt.TotalBeforeTax = receipt.Lines.Sum(l => l.TotalCost);
        receipt.GrandTotal = receipt.TotalBeforeTax + receipt.TaxAmount;

        _context.GoodsReceipts.Add(receipt);

        // Update PO status based on received quantities
        if (po != null)
        {
            var allFullyReceived = po.Lines.All(l => l.ReceivedQuantity >= l.Quantity);
            var anyReceived = po.Lines.Any(l => l.ReceivedQuantity > 0);

            if (allFullyReceived)
                po.Status = "Received";
            else if (anyReceived)
                po.Status = "PartiallyReceived";
        }

        await _context.SaveChangesAsync();

        return Ok(new { receipt.Id, receipt.ReceiptNumber, message = "Goods received and inventory updated" });
    }
}

public class CreateGoodsReceiptRequest
{
    public int? PurchaseOrderId { get; set; }
    public string? Notes { get; set; }
    public List<CreateGRLineRequest>? Lines { get; set; }
}

public class CreateGRLineRequest
{
    public int InventoryItemId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal OrderedQuantity { get; set; }
}
