using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/purchase-orders")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public PurchaseOrdersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? status)
    {
        var companyId = GetCompanyId();
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Lines)
            .Where(po => po.Supplier != null && po.Supplier.CompanyId == companyId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(po => po.Status == status);

        var orders = await query
            .OrderByDescending(po => po.PODate)
            .Select(po => new
            {
                po.Id,
                po.OrderNumber,
                SupplierName = po.Supplier != null ? po.Supplier.Name : null,
                po.Status,
                po.ExpectedDate,
                po.TotalAmount,
                CreatedAt = po.PODate,
                LineCount = po.Lines.Count
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var po = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Lines)
                .ThenInclude(l => l.InventoryItem)
            .FirstOrDefaultAsync(p => p.Id == id && p.Supplier != null && p.Supplier.CompanyId == companyId);

        if (po == null) return NotFound();

        return Ok(new
        {
            po.Id,
            po.OrderNumber,
            po.SupplierId,
            SupplierName = po.Supplier?.Name,
            po.Status,
            po.ExpectedDate,
            po.TotalAmount,
            CreatedAt = po.PODate,
            Lines = po.Lines.Select(l => new
            {
                l.Id,
                l.InventoryItemId,
                ItemName = l.InventoryItem?.Name,
                Unit = l.InventoryItem?.UnitOfMeasure,
                l.Quantity,
                l.UnitPrice,
                l.TotalPrice,
                l.ReceivedQuantity
            })
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
    {
        var companyId = GetCompanyId();
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == request.SupplierId && s.CompanyId == companyId);
        if (supplier == null) return BadRequest(new { message = "Supplier not found" });

        var po = new PurchaseOrder
        {
            SupplierId = request.SupplierId,
            OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = "Draft",
            PODate = DateTime.UtcNow,
            ExpectedDate = request.ExpectedDate,
            BranchId = request.BranchId,
            TotalAmount = 0
        };

        if (request.Lines != null && request.Lines.Any())
        {
            foreach (var line in request.Lines)
            {
                var poLine = new PurchaseOrderLine
                {
                    InventoryItemId = line.InventoryItemId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TotalPrice = line.Quantity * line.UnitPrice
                };
                po.Lines.Add(poLine);
            }
            po.TotalAmount = po.Lines.Sum(l => l.TotalPrice);
        }

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();

        return Ok(new { po.Id, po.OrderNumber });
    }

    [HttpPatch("{id}/approve")]
    public async Task<ActionResult> Approve(int id)
    {
        var companyId = GetCompanyId();
        var po = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id && p.Supplier != null && p.Supplier.CompanyId == companyId);
        if (po == null) return NotFound();
        if (po.Status != "Draft") return BadRequest("Only draft orders can be approved");

        po.Status = "Approved";

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var po = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.Id == id && p.Supplier != null && p.Supplier.CompanyId == companyId);

        if (po == null) return NotFound();
        if (po.Status != "Draft") return BadRequest("Only draft orders can be deleted");

        _context.PurchaseOrderLines.RemoveRange(po.Lines);
        _context.PurchaseOrders.Remove(po);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreatePurchaseOrderRequest
{
    public int SupplierId { get; set; }
    public int? BranchId { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public List<CreatePOLineRequest>? Lines { get; set; }
}

public class CreatePOLineRequest
{
    public int InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
