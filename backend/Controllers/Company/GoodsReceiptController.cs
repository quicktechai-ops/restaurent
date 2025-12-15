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

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var receipts = await _context.GoodsReceipts
            .Include(gr => gr.Supplier)
            .OrderByDescending(gr => gr.GRNDate)
            .Select(gr => new
            {
                gr.Id,
                gr.ReceiptNumber,
                gr.PurchaseOrderId,
                SupplierName = gr.Supplier != null ? gr.Supplier.Name : null,
                gr.GrandTotal,
                gr.Status,
                gr.GRNDate
            })
            .ToListAsync();

        return Ok(receipts);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateGoodsReceiptRequest request)
    {
        var receipt = new GoodsReceipt
        {
            PurchaseOrderId = request.PurchaseOrderId,
            SupplierId = request.SupplierId,
            BranchId = request.BranchId,
            ReceiptNumber = $"GRN-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = "Received",
            GRNDate = DateTime.UtcNow,
            TotalBeforeTax = request.TotalBeforeTax,
            TaxAmount = request.TaxAmount,
            GrandTotal = request.TotalBeforeTax + request.TaxAmount
        };

        _context.GoodsReceipts.Add(receipt);
        await _context.SaveChangesAsync();

        return Ok(new { receipt.Id, receipt.ReceiptNumber });
    }
}

public class CreateGoodsReceiptRequest
{
    public int? PurchaseOrderId { get; set; }
    public int SupplierId { get; set; }
    public int? BranchId { get; set; }
    public decimal TotalBeforeTax { get; set; }
    public decimal TaxAmount { get; set; }
}
