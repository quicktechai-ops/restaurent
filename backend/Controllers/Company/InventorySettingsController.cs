using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/inventory-settings")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class InventorySettingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventorySettingsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    // ==================== CATEGORIES ====================

    [HttpGet("categories")]
    public async Task<ActionResult<List<InventoryCategoryDto>>> GetCategories()
    {
        var companyId = GetCompanyId();
        var categories = await _context.InventoryCategories
            .Where(c => c.CompanyId == companyId)
            .Include(c => c.ParentCategory)
            .OrderBy(c => c.ParentCategoryId).ThenBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Select(c => new InventoryCategoryDto
            {
                Id = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<InventoryCategoryDto>> CreateCategory([FromBody] CreateInventoryCategoryRequest request)
    {
        var companyId = GetCompanyId();
        
        var exists = await _context.InventoryCategories
            .AnyAsync(c => c.CompanyId == companyId && 
                          c.Name.ToLower() == request.Name.ToLower() && 
                          c.ParentCategoryId == request.ParentCategoryId);
        if (exists) return BadRequest("Category already exists");

        var category = new InventoryCategory
        {
            CompanyId = companyId,
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            SortOrder = request.SortOrder
        };

        _context.InventoryCategories.Add(category);
        await _context.SaveChangesAsync();

        // Load parent name
        string? parentName = null;
        if (category.ParentCategoryId.HasValue)
        {
            parentName = await _context.InventoryCategories
                .Where(c => c.CategoryId == category.ParentCategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        return Ok(new InventoryCategoryDto
        {
            Id = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = parentName,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        });
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult<InventoryCategoryDto>> UpdateCategory(int id, [FromBody] UpdateInventoryCategoryRequest request)
    {
        var companyId = GetCompanyId();
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);
        
        if (category == null) return NotFound();

        // Prevent setting parent to itself or its own children
        if (request.ParentCategoryId == id) return BadRequest("Category cannot be its own parent");

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        // Load parent name
        string? parentName = null;
        if (category.ParentCategoryId.HasValue)
        {
            parentName = await _context.InventoryCategories
                .Where(c => c.CategoryId == category.ParentCategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        return Ok(new InventoryCategoryDto
        {
            Id = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = parentName,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        });
    }

    [HttpPatch("categories/{id}/toggle")]
    public async Task<IActionResult> ToggleCategory(int id)
    {
        var companyId = GetCompanyId();
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);
        
        if (category == null) return NotFound();

        category.IsActive = !category.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { category.IsActive });
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var companyId = GetCompanyId();
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);
        
        if (category == null) return NotFound();

        _context.InventoryCategories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================== UNITS OF MEASURE ====================

    [HttpGet("units")]
    public async Task<ActionResult<List<UnitOfMeasureDto>>> GetUnits()
    {
        var companyId = GetCompanyId();
        var units = await _context.UnitsOfMeasure
            .Where(u => u.CompanyId == companyId)
            .OrderBy(u => u.SortOrder).ThenBy(u => u.Name)
            .Select(u => new UnitOfMeasureDto
            {
                Id = u.UnitId,
                Code = u.Code,
                Name = u.Name,
                Symbol = u.Symbol,
                SortOrder = u.SortOrder,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return Ok(units);
    }

    [HttpPost("units")]
    public async Task<ActionResult<UnitOfMeasureDto>> CreateUnit([FromBody] CreateUnitRequest request)
    {
        var companyId = GetCompanyId();
        
        var exists = await _context.UnitsOfMeasure
            .AnyAsync(u => u.CompanyId == companyId && u.Code.ToLower() == request.Code.ToLower());
        if (exists) return BadRequest("Unit code already exists");

        var unit = new UnitOfMeasure
        {
            CompanyId = companyId,
            Code = request.Code,
            Name = request.Name,
            Symbol = request.Symbol,
            SortOrder = request.SortOrder
        };

        _context.UnitsOfMeasure.Add(unit);
        await _context.SaveChangesAsync();

        return Ok(new UnitOfMeasureDto
        {
            Id = unit.UnitId,
            Code = unit.Code,
            Name = unit.Name,
            Symbol = unit.Symbol,
            SortOrder = unit.SortOrder,
            IsActive = unit.IsActive
        });
    }

    [HttpPut("units/{id}")]
    public async Task<ActionResult<UnitOfMeasureDto>> UpdateUnit(int id, [FromBody] UpdateUnitRequest request)
    {
        var companyId = GetCompanyId();
        var unit = await _context.UnitsOfMeasure
            .FirstOrDefaultAsync(u => u.UnitId == id && u.CompanyId == companyId);
        
        if (unit == null) return NotFound();

        unit.Code = request.Code;
        unit.Name = request.Name;
        unit.Symbol = request.Symbol;
        unit.SortOrder = request.SortOrder;
        unit.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new UnitOfMeasureDto
        {
            Id = unit.UnitId,
            Code = unit.Code,
            Name = unit.Name,
            Symbol = unit.Symbol,
            SortOrder = unit.SortOrder,
            IsActive = unit.IsActive
        });
    }

    [HttpPatch("units/{id}/toggle")]
    public async Task<IActionResult> ToggleUnit(int id)
    {
        var companyId = GetCompanyId();
        var unit = await _context.UnitsOfMeasure
            .FirstOrDefaultAsync(u => u.UnitId == id && u.CompanyId == companyId);
        
        if (unit == null) return NotFound();

        unit.IsActive = !unit.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { unit.IsActive });
    }

    [HttpDelete("units/{id}")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        var companyId = GetCompanyId();
        var unit = await _context.UnitsOfMeasure
            .FirstOrDefaultAsync(u => u.UnitId == id && u.CompanyId == companyId);
        
        if (unit == null) return NotFound();

        _context.UnitsOfMeasure.Remove(unit);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==================== UNIT CONVERSIONS ====================

    [HttpGet("conversions")]
    public async Task<ActionResult<List<UnitConversionDto>>> GetConversions()
    {
        var companyId = GetCompanyId();
        var conversions = await _context.UnitConversions
            .Where(c => c.CompanyId == companyId)
            .OrderBy(c => c.FromUnitCode).ThenBy(c => c.ToUnitCode)
            .Select(c => new UnitConversionDto
            {
                Id = c.ConversionId,
                FromUnitCode = c.FromUnitCode,
                ToUnitCode = c.ToUnitCode,
                ConversionFactor = c.ConversionFactor,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return Ok(conversions);
    }

    [HttpPost("conversions")]
    public async Task<ActionResult<UnitConversionDto>> CreateConversion([FromBody] CreateConversionRequest request)
    {
        var companyId = GetCompanyId();
        
        // Check if conversion already exists
        var exists = await _context.UnitConversions
            .AnyAsync(c => c.CompanyId == companyId && 
                          c.FromUnitCode == request.FromUnitCode && 
                          c.ToUnitCode == request.ToUnitCode);
        if (exists) return BadRequest("Conversion already exists");

        var conversion = new UnitConversion
        {
            CompanyId = companyId,
            FromUnitCode = request.FromUnitCode,
            ToUnitCode = request.ToUnitCode,
            ConversionFactor = request.ConversionFactor
        };

        _context.UnitConversions.Add(conversion);
        
        // Also create reverse conversion automatically
        var reverseConversion = new UnitConversion
        {
            CompanyId = companyId,
            FromUnitCode = request.ToUnitCode,
            ToUnitCode = request.FromUnitCode,
            ConversionFactor = 1 / request.ConversionFactor
        };
        _context.UnitConversions.Add(reverseConversion);
        
        await _context.SaveChangesAsync();

        return Ok(new UnitConversionDto
        {
            Id = conversion.ConversionId,
            FromUnitCode = conversion.FromUnitCode,
            ToUnitCode = conversion.ToUnitCode,
            ConversionFactor = conversion.ConversionFactor,
            IsActive = conversion.IsActive
        });
    }

    [HttpPut("conversions/{id}")]
    public async Task<ActionResult<UnitConversionDto>> UpdateConversion(int id, [FromBody] UpdateConversionRequest request)
    {
        var companyId = GetCompanyId();
        var conversion = await _context.UnitConversions
            .FirstOrDefaultAsync(c => c.ConversionId == id && c.CompanyId == companyId);
        
        if (conversion == null) return NotFound();

        // Update reverse conversion too
        var reverse = await _context.UnitConversions
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && 
                                      c.FromUnitCode == conversion.ToUnitCode && 
                                      c.ToUnitCode == conversion.FromUnitCode);

        conversion.ConversionFactor = request.ConversionFactor;
        conversion.IsActive = request.IsActive;

        if (reverse != null)
        {
            reverse.ConversionFactor = 1 / request.ConversionFactor;
            reverse.IsActive = request.IsActive;
        }

        await _context.SaveChangesAsync();

        return Ok(new UnitConversionDto
        {
            Id = conversion.ConversionId,
            FromUnitCode = conversion.FromUnitCode,
            ToUnitCode = conversion.ToUnitCode,
            ConversionFactor = conversion.ConversionFactor,
            IsActive = conversion.IsActive
        });
    }

    [HttpDelete("conversions/{id}")]
    public async Task<IActionResult> DeleteConversion(int id)
    {
        var companyId = GetCompanyId();
        var conversion = await _context.UnitConversions
            .FirstOrDefaultAsync(c => c.ConversionId == id && c.CompanyId == companyId);
        
        if (conversion == null) return NotFound();

        // Also delete reverse conversion
        var reverse = await _context.UnitConversions
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && 
                                      c.FromUnitCode == conversion.ToUnitCode && 
                                      c.ToUnitCode == conversion.FromUnitCode);

        _context.UnitConversions.Remove(conversion);
        if (reverse != null) _context.UnitConversions.Remove(reverse);
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("convert")]
    public async Task<ActionResult<decimal>> Convert([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal value)
    {
        var companyId = GetCompanyId();
        
        if (from == to) return Ok(value);

        var conversion = await _context.UnitConversions
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && 
                                      c.FromUnitCode == from && 
                                      c.ToUnitCode == to && 
                                      c.IsActive);

        if (conversion == null) return BadRequest($"No conversion found from {from} to {to}");

        return Ok(value * conversion.ConversionFactor);
    }
}

// DTOs
public class InventoryCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateInventoryCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateInventoryCategoryRequest : CreateInventoryCategoryRequest
{
    public bool IsActive { get; set; } = true;
}

public class UnitOfMeasureDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateUnitRequest : CreateUnitRequest
{
    public bool IsActive { get; set; } = true;
}

public class UnitConversionDto
{
    public int Id { get; set; }
    public string FromUnitCode { get; set; } = string.Empty;
    public string ToUnitCode { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; }
    public bool IsActive { get; set; }
}

public class CreateConversionRequest
{
    public string FromUnitCode { get; set; } = string.Empty;
    public string ToUnitCode { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; }
}

public class UpdateConversionRequest
{
    public decimal ConversionFactor { get; set; }
    public bool IsActive { get; set; } = true;
}
