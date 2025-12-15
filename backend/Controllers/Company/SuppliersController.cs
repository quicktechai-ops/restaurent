using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<SupplierListDto>>> GetAll([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var companyId = GetCompanyId();
        var query = _context.Suppliers.Where(s => s.CompanyId == companyId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s => s.Name.Contains(search) || (s.ContactPerson != null && s.ContactPerson.Contains(search)));

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        var suppliers = await query
            .OrderBy(s => s.Name)
            .Select(s => new SupplierListDto
            {
                Id = s.SupplierId,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                PaymentTerms = s.PaymentTerms,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id && s.CompanyId == companyId);
        if (supplier == null) return NotFound();

        return Ok(new SupplierListDto
        {
            Id = supplier.SupplierId,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            PaymentTerms = supplier.PaymentTerms,
            IsActive = supplier.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<SupplierListDto>> Create([FromBody] CreateSupplierRequest request)
    {
        var companyId = GetCompanyId();

        var supplier = new Supplier
        {
            CompanyId = companyId,
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            PaymentTerms = request.PaymentTerms
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return Ok(new SupplierListDto
        {
            Id = supplier.SupplierId,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            IsActive = supplier.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SupplierListDto>> Update(int id, [FromBody] UpdateSupplierRequest request)
    {
        var companyId = GetCompanyId();
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id && s.CompanyId == companyId);
        if (supplier == null) return NotFound();

        supplier.Name = request.Name;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Phone = request.Phone;
        supplier.Email = request.Email;
        supplier.Address = request.Address;
        supplier.PaymentTerms = request.PaymentTerms;
        supplier.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new SupplierListDto
        {
            Id = supplier.SupplierId,
            Name = supplier.Name,
            IsActive = supplier.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id && s.CompanyId == companyId);
        if (supplier == null) return NotFound();

        supplier.IsActive = !supplier.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { supplier.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id && s.CompanyId == companyId);
        if (supplier == null) return NotFound();

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
