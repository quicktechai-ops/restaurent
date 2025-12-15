using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin")]
public class BranchesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BranchesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<BranchListDto>>> GetAll()
    {
        var companyId = GetCompanyId();

        var branches = await _context.Branches
            .Where(b => b.CompanyId == companyId && b.DeletedAt == null)
            .Select(b => new BranchListDto
            {
                Id = b.BranchId,
                Name = b.Name,
                Code = b.Code,
                City = b.City,
                Address = b.Address,
                Phone = b.Phone,
                CurrencyCode = b.DefaultCurrencyCode,
                VatPercent = b.VatPercent,
                ServiceChargePercent = b.ServiceChargePercent,
                IsActive = b.IsActive,
                TablesCount = _context.RestaurantTables.Count(t => t.BranchId == b.BranchId),
                UsersCount = _context.Users.Count(u => u.DefaultBranchId == b.BranchId)
            })
            .ToListAsync();

        return Ok(branches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BranchListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var branch = await _context.Branches
            .Where(b => b.BranchId == id && b.CompanyId == companyId && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (branch == null)
            return NotFound(new { message = "Branch not found" });

        return Ok(new BranchListDto
        {
            Id = branch.BranchId,
            Name = branch.Name,
            Code = branch.Code,
            City = branch.City,
            Address = branch.Address,
            Phone = branch.Phone,
            CurrencyCode = branch.DefaultCurrencyCode,
            VatPercent = branch.VatPercent,
            ServiceChargePercent = branch.ServiceChargePercent,
            IsActive = branch.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<BranchListDto>> Create([FromBody] CreateBranchRequest request)
    {
        var companyId = GetCompanyId();

        // Check branch limit
        var company = await _context.Companies.FindAsync(companyId);
        var currentBranches = await _context.Branches.CountAsync(b => b.CompanyId == companyId && b.DeletedAt == null);
        
        if (currentBranches >= company!.MaxBranches)
            return BadRequest(new { message = $"Maximum branches limit ({company.MaxBranches}) reached. Upgrade your plan." });

        var branch = new Branch
        {
            CompanyId = companyId,
            Name = request.Name,
            Code = request.Code,
            Country = request.Country,
            City = request.City,
            Address = request.Address,
            Phone = request.Phone,
            DefaultCurrencyCode = request.CurrencyCode,
            VatPercent = request.VatPercent,
            ServiceChargePercent = request.ServiceChargePercent,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = branch.BranchId }, new BranchListDto
        {
            Id = branch.BranchId,
            Name = branch.Name,
            Code = branch.Code,
            City = branch.City,
            Address = branch.Address,
            CurrencyCode = branch.DefaultCurrencyCode,
            VatPercent = branch.VatPercent,
            ServiceChargePercent = branch.ServiceChargePercent,
            IsActive = branch.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateBranchRequest request)
    {
        var companyId = GetCompanyId();

        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchId == id && b.CompanyId == companyId && b.DeletedAt == null);

        if (branch == null)
            return NotFound(new { message = "Branch not found" });

        branch.Name = request.Name;
        branch.Code = request.Code;
        branch.Country = request.Country;
        branch.City = request.City;
        branch.Address = request.Address;
        branch.Phone = request.Phone;
        branch.DefaultCurrencyCode = request.CurrencyCode;
        branch.VatPercent = request.VatPercent;
        branch.ServiceChargePercent = request.ServiceChargePercent;
        branch.IsActive = request.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Branch updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchId == id && b.CompanyId == companyId && b.DeletedAt == null);

        if (branch == null)
            return NotFound(new { message = "Branch not found" });

        // Soft delete
        branch.DeletedAt = DateTime.UtcNow;
        branch.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Branch deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchId == id && b.CompanyId == companyId && b.DeletedAt == null);

        if (branch == null)
            return NotFound(new { message = "Branch not found" });

        branch.IsActive = !branch.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Branch is now {(branch.IsActive ? "active" : "inactive")}" });
    }
}
