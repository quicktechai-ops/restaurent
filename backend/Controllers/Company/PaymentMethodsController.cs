using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/payment-methods")]
[Authorize(Roles = "CompanyAdmin,User")]
public class PaymentMethodsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PaymentMethodsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodListDto>>> GetAll([FromQuery] bool? isActive)
    {
        var companyId = GetCompanyId();

        var query = _context.PaymentMethods
            .Where(p => p.CompanyId == companyId);

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var methods = await query
            .OrderBy(p => p.SortOrder)
            .Select(p => new PaymentMethodListDto
            {
                Id = p.PaymentMethodId,
                Name = p.Name,
                Type = p.Type,
                RequiresReference = p.RequiresReference,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder
            })
            .ToListAsync();

        return Ok(methods);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentMethodListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var method = await _context.PaymentMethods
            .Where(p => p.PaymentMethodId == id && p.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (method == null)
            return NotFound(new { message = "Payment method not found" });

        return Ok(new PaymentMethodListDto
        {
            Id = method.PaymentMethodId,
            Name = method.Name,
            Type = method.Type,
            RequiresReference = method.RequiresReference,
            IsActive = method.IsActive,
            SortOrder = method.SortOrder
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<PaymentMethodListDto>> Create([FromBody] CreatePaymentMethodRequest request)
    {
        var companyId = GetCompanyId();

        var method = new PaymentMethod
        {
            CompanyId = companyId,
            Name = request.Name,
            Type = request.Type,
            RequiresReference = request.RequiresReference,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentMethods.Add(method);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = method.PaymentMethodId }, new PaymentMethodListDto
        {
            Id = method.PaymentMethodId,
            Name = method.Name,
            Type = method.Type,
            IsActive = method.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePaymentMethodRequest request)
    {
        var companyId = GetCompanyId();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.PaymentMethodId == id && p.CompanyId == companyId);

        if (method == null)
            return NotFound(new { message = "Payment method not found" });

        method.Name = request.Name;
        method.Type = request.Type;
        method.RequiresReference = request.RequiresReference;
        method.SortOrder = request.SortOrder;
        method.IsActive = request.IsActive;
        method.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Payment method updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.PaymentMethodId == id && p.CompanyId == companyId);

        if (method == null)
            return NotFound(new { message = "Payment method not found" });

        _context.PaymentMethods.Remove(method);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment method deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var method = await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.PaymentMethodId == id && p.CompanyId == companyId);

        if (method == null)
            return NotFound(new { message = "Payment method not found" });

        method.IsActive = !method.IsActive;
        method.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Payment method is now {(method.IsActive ? "active" : "inactive")}" });
    }
}
