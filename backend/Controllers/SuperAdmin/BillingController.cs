using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.SuperAdmin;

[ApiController]
[Route("api/superadmin/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class BillingController : ControllerBase
{
    private readonly AppDbContext _context;

    public BillingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BillingListDto>>> GetAll(
        [FromQuery] int? companyId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = _context.CompanyPayments
            .Include(p => p.Company)
            .AsQueryable();

        if (companyId.HasValue)
        {
            query = query.Where(p => p.CompanyId == companyId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= toDate.Value);
        }

        var billings = await query
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new BillingListDto
            {
                Id = p.PaymentId,
                CompanyId = p.CompanyId,
                CompanyName = p.Company.Name,
                Amount = p.Amount,
                CurrencyCode = p.CurrencyCode,
                PaymentMethod = p.PaymentMethod,
                PaymentReference = p.PaymentReference,
                PaymentDate = p.PaymentDate,
                Status = p.Status,
                Notes = p.Notes
            })
            .ToListAsync();

        return Ok(billings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BillingListDto>> GetById(int id)
    {
        var payment = await _context.CompanyPayments
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        return Ok(new BillingListDto
        {
            Id = payment.PaymentId,
            CompanyId = payment.CompanyId,
            CompanyName = payment.Company.Name,
            Amount = payment.Amount,
            CurrencyCode = payment.CurrencyCode,
            PaymentMethod = payment.PaymentMethod,
            PaymentReference = payment.PaymentReference,
            PaymentDate = payment.PaymentDate,
            Status = payment.Status,
            Notes = payment.Notes
        });
    }

    [HttpPost]
    public async Task<ActionResult<BillingListDto>> Create([FromBody] CreateBillingRequest request)
    {
        var company = await _context.Companies.FindAsync(request.CompanyId);
        if (company == null)
            return BadRequest(new { message = "Company not found" });

        var superAdminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var payment = new CompanyPayment
        {
            CompanyId = request.CompanyId,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            PaymentMethod = request.PaymentMethod,
            PaymentReference = request.PaymentReference,
            PaymentDate = request.PaymentDate,
            Status = request.Status,
            Notes = request.Notes,
            RecordedBySuperAdminId = superAdminId,
            CreatedAt = DateTime.UtcNow
        };

        _context.CompanyPayments.Add(payment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, new BillingListDto
        {
            Id = payment.PaymentId,
            CompanyId = payment.CompanyId,
            CompanyName = company.Name,
            Amount = payment.Amount,
            CurrencyCode = payment.CurrencyCode,
            PaymentMethod = payment.PaymentMethod,
            PaymentReference = payment.PaymentReference,
            PaymentDate = payment.PaymentDate,
            Status = payment.Status,
            Notes = payment.Notes
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateBillingRequest request)
    {
        var payment = await _context.CompanyPayments.FindAsync(id);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        payment.CompanyId = request.CompanyId;
        payment.Amount = request.Amount;
        payment.CurrencyCode = request.CurrencyCode;
        payment.PaymentMethod = request.PaymentMethod;
        payment.PaymentReference = request.PaymentReference;
        payment.PaymentDate = request.PaymentDate;
        payment.Status = request.Status;
        payment.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Payment updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var payment = await _context.CompanyPayments.FindAsync(id);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        _context.CompanyPayments.Remove(payment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment deleted successfully" });
    }
}
