using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/inventory")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<InventoryItemListDto>>> GetAll([FromQuery] string? search, [FromQuery] string? category)
    {
        var companyId = GetCompanyId();
        var query = _context.InventoryItems
            .Include(i => i.RecipeIngredients)
            .Where(i => i.CompanyId == companyId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Name.Contains(search) || (i.Code != null && i.Code.Contains(search)));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(i => i.Category == category);

        var items = await query
            .OrderBy(i => i.Name)
            .Select(i => new InventoryItemListDto
            {
                Id = i.InventoryItemId,
                Name = i.Name,
                Code = i.Code,
                UnitOfMeasure = i.UnitOfMeasure,
                Category = i.Category,
                MinLevel = i.MinLevel,
                ReorderQty = i.ReorderQty,
                CostMethod = i.CostMethod,
                Quantity = i.Quantity,
                Cost = i.Cost,
                CurrencyCode = i.CurrencyCode,
                IsActive = i.IsActive,
                RecipesCount = i.RecipeIngredients.Count
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItemListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.InventoryItemId == id && i.CompanyId == companyId);

        if (item == null) return NotFound();

        return Ok(new InventoryItemListDto
        {
            Id = item.InventoryItemId,
            Name = item.Name,
            Code = item.Code,
            UnitOfMeasure = item.UnitOfMeasure,
            Category = item.Category,
            MinLevel = item.MinLevel,
            ReorderQty = item.ReorderQty,
            CostMethod = item.CostMethod,
            Quantity = item.Quantity,
            Cost = item.Cost,
            CurrencyCode = item.CurrencyCode,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItemListDto>> Create([FromBody] CreateInventoryItemRequest request)
    {
        var companyId = GetCompanyId();

        if (!string.IsNullOrEmpty(request.Code))
        {
            var exists = await _context.InventoryItems.AnyAsync(i => i.CompanyId == companyId && i.Code == request.Code);
            if (exists) return BadRequest(new { message = "Item code already exists" });
        }

        var item = new InventoryItem
        {
            CompanyId = companyId,
            Name = request.Name,
            Code = request.Code,
            UnitOfMeasure = request.UnitOfMeasure,
            Category = request.Category,
            MinLevel = request.MinLevel,
            ReorderQty = request.ReorderQty,
            CostMethod = request.CostMethod,
            Quantity = request.Quantity,
            Cost = request.Cost,
            CurrencyCode = request.CurrencyCode
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new InventoryItemListDto
        {
            Id = item.InventoryItemId,
            Name = item.Name,
            Code = item.Code,
            UnitOfMeasure = item.UnitOfMeasure,
            Category = item.Category,
            IsActive = item.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<InventoryItemListDto>> Update(int id, [FromBody] UpdateInventoryItemRequest request)
    {
        var companyId = GetCompanyId();
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == id && i.CompanyId == companyId);
        if (item == null) return NotFound();

        item.Name = request.Name;
        item.Code = request.Code;
        item.UnitOfMeasure = request.UnitOfMeasure;
        item.Category = request.Category;
        item.MinLevel = request.MinLevel;
        item.ReorderQty = request.ReorderQty;
        item.CostMethod = request.CostMethod;
        item.Quantity = request.Quantity;
        item.Cost = request.Cost;
        item.CurrencyCode = request.CurrencyCode;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new InventoryItemListDto
        {
            Id = item.InventoryItemId,
            Name = item.Name,
            Code = item.Code,
            UnitOfMeasure = item.UnitOfMeasure,
            IsActive = item.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == id && i.CompanyId == companyId);
        if (item == null) return NotFound();

        item.IsActive = !item.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { item.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == id && i.CompanyId == companyId);
        if (item == null) return NotFound();

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        var companyId = GetCompanyId();
        var categories = await _context.InventoryItems
            .Where(i => i.CompanyId == companyId && i.Category != null)
            .Select(i => i.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(categories);
    }
}
