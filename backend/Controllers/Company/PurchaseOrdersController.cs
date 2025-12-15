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

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? status)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
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
                po.PODate
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);

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
            po.PODate
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreatePurchaseOrderRequest request)
    {
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

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();

        return Ok(new { po.Id, po.OrderNumber });
    }

    [HttpPatch("{id}/approve")]
    public async Task<ActionResult> Approve(int id)
    {
        var po = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id);
        if (po == null) return NotFound();
        if (po.Status != "Draft") return BadRequest("Only draft orders can be approved");

        po.Status = "Approved";

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var po = await _context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id);

        if (po == null) return NotFound();
        if (po.Status != "Draft") return BadRequest("Only draft orders can be deleted");

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
}
