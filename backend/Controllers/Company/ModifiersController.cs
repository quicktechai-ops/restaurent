using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,User")]
public class ModifiersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ModifiersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<ModifierListDto>>> GetAll([FromQuery] bool? isActive)
    {
        var companyId = GetCompanyId();

        var query = _context.Modifiers
            .Where(m => m.CompanyId == companyId);

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        var modifiers = await query
            .OrderBy(m => m.Name)
            .Select(m => new ModifierListDto
            {
                Id = m.ModifierId,
                Name = m.Name,
                NameAr = m.NameAr,
                Description = m.Description,
                ExtraPrice = m.ExtraPrice,
                CurrencyCode = m.CurrencyCode,
                IsActive = m.IsActive
            })
            .ToListAsync();

        return Ok(modifiers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModifierListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var modifier = await _context.Modifiers
            .Where(m => m.ModifierId == id && m.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (modifier == null)
            return NotFound(new { message = "Modifier not found" });

        return Ok(new ModifierListDto
        {
            Id = modifier.ModifierId,
            Name = modifier.Name,
            NameAr = modifier.NameAr,
            Description = modifier.Description,
            ExtraPrice = modifier.ExtraPrice,
            CurrencyCode = modifier.CurrencyCode,
            IsActive = modifier.IsActive
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<ModifierListDto>> Create([FromBody] CreateModifierRequest request)
    {
        var companyId = GetCompanyId();

        var modifier = new Modifier
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            Name = request.Name,
            NameAr = request.NameAr,
            Description = request.Description,
            ExtraPrice = request.ExtraPrice,
            CurrencyCode = request.CurrencyCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Modifiers.Add(modifier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = modifier.ModifierId }, new ModifierListDto
        {
            Id = modifier.ModifierId,
            Name = modifier.Name,
            ExtraPrice = modifier.ExtraPrice,
            IsActive = modifier.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateModifierRequest request)
    {
        var companyId = GetCompanyId();

        var modifier = await _context.Modifiers
            .FirstOrDefaultAsync(m => m.ModifierId == id && m.CompanyId == companyId);

        if (modifier == null)
            return NotFound(new { message = "Modifier not found" });

        modifier.Name = request.Name;
        modifier.NameAr = request.NameAr;
        modifier.Description = request.Description;
        modifier.ExtraPrice = request.ExtraPrice;
        modifier.CurrencyCode = request.CurrencyCode;
        modifier.BranchId = request.BranchId;
        modifier.IsActive = request.IsActive;
        modifier.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Modifier updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var modifier = await _context.Modifiers
            .FirstOrDefaultAsync(m => m.ModifierId == id && m.CompanyId == companyId);

        if (modifier == null)
            return NotFound(new { message = "Modifier not found" });

        // Check if modifier is used
        var isUsed = await _context.MenuItemModifiers.AnyAsync(mm => mm.ModifierId == id);
        if (isUsed)
            return BadRequest(new { message = "Cannot delete modifier that is assigned to menu items" });

        _context.Modifiers.Remove(modifier);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Modifier deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var modifier = await _context.Modifiers
            .FirstOrDefaultAsync(m => m.ModifierId == id && m.CompanyId == companyId);

        if (modifier == null)
            return NotFound(new { message = "Modifier not found" });

        modifier.IsActive = !modifier.IsActive;
        modifier.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Modifier is now {(modifier.IsActive ? "active" : "inactive")}" });
    }
}
