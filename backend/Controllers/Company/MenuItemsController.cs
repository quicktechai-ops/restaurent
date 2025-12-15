using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/menu-items")]
[Authorize(Roles = "CompanyAdmin,User")]
public class MenuItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MenuItemsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuItemListDto>>> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] bool? isActive,
        [FromQuery] string? search)
    {
        var companyId = GetCompanyId();

        var query = _context.MenuItems
            .Include(m => m.Category)
            .Include(m => m.KitchenStation)
            .Include(m => m.Sizes)
            .Where(m => m.CompanyId == companyId);

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(m => m.Name.Contains(search) || (m.Code != null && m.Code.Contains(search)));
        }

        var items = await query
            .OrderBy(m => m.Category.SortOrder)
            .ThenBy(m => m.Name)
            .Select(m => new MenuItemListDto
            {
                Id = m.MenuItemId,
                Name = m.Name,
                NameAr = m.NameAr,
                Code = m.Code,
                Description = m.Description,
                CategoryId = m.CategoryId,
                CategoryName = m.Category.Name,
                DefaultPrice = m.DefaultPrice,
                CurrencyCode = m.CurrencyCode,
                TaxIncluded = m.TaxIncluded,
                AllowSizes = m.AllowSizes,
                IsActive = m.IsActive,
                IsAvailable = m.IsAvailable,
                IsFeatured = m.IsFeatured,
                ImageUrl = m.ImageUrl,
                KitchenStationId = m.KitchenStationId,
                KitchenStationName = m.KitchenStation != null ? m.KitchenStation.Name : null,
                Sizes = m.Sizes.Select(s => new MenuItemSizeDto
                {
                    Id = s.MenuItemSizeId,
                    SizeName = s.SizeName,
                    Price = s.Price,
                    CurrencyCode = s.CurrencyCode
                }).ToList()
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var item = await _context.MenuItems
            .Include(m => m.Category)
            .Include(m => m.KitchenStation)
            .Include(m => m.Sizes)
            .Where(m => m.MenuItemId == id && m.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (item == null)
            return NotFound(new { message = "Menu item not found" });

        return Ok(new MenuItemListDto
        {
            Id = item.MenuItemId,
            Name = item.Name,
            NameAr = item.NameAr,
            Code = item.Code,
            Description = item.Description,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name,
            DefaultPrice = item.DefaultPrice,
            CurrencyCode = item.CurrencyCode,
            TaxIncluded = item.TaxIncluded,
            AllowSizes = item.AllowSizes,
            IsActive = item.IsActive,
            IsAvailable = item.IsAvailable,
            IsFeatured = item.IsFeatured,
            ImageUrl = item.ImageUrl,
            KitchenStationId = item.KitchenStationId,
            KitchenStationName = item.KitchenStation?.Name,
            Sizes = item.Sizes.Select(s => new MenuItemSizeDto
            {
                Id = s.MenuItemSizeId,
                SizeName = s.SizeName,
                Price = s.Price,
                CurrencyCode = s.CurrencyCode
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<MenuItemListDto>> Create([FromBody] CreateMenuItemRequest request)
    {
        var companyId = GetCompanyId();

        var item = new MenuItem
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            NameAr = request.NameAr,
            Code = request.Code,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            DefaultPrice = request.DefaultPrice,
            CurrencyCode = request.CurrencyCode,
            TaxIncluded = request.TaxIncluded,
            AllowSizes = request.AllowSizes,
            KitchenStationId = request.KitchenStationId,
            ImageUrl = request.ImageUrl,
            Calories = request.Calories,
            PrepTimeMinutes = request.PrepTimeMinutes,
            Allergens = request.Allergens,
            IsActive = true,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync();

        // Add sizes
        foreach (var size in request.Sizes)
        {
            _context.MenuItemSizes.Add(new MenuItemSize
            {
                MenuItemId = item.MenuItemId,
                SizeName = size.SizeName,
                Price = size.Price,
                CurrencyCode = size.CurrencyCode,
                Cost = size.Cost
            });
        }

        // Add modifiers
        foreach (var modifierId in request.ModifierIds)
        {
            _context.MenuItemModifiers.Add(new MenuItemModifier
            {
                MenuItemId = item.MenuItemId,
                ModifierId = modifierId
            });
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.MenuItemId }, new MenuItemListDto
        {
            Id = item.MenuItemId,
            Name = item.Name,
            DefaultPrice = item.DefaultPrice,
            IsActive = item.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateMenuItemRequest request)
    {
        var companyId = GetCompanyId();

        var item = await _context.MenuItems
            .Include(m => m.Sizes)
            .FirstOrDefaultAsync(m => m.MenuItemId == id && m.CompanyId == companyId);

        if (item == null)
            return NotFound(new { message = "Menu item not found" });

        item.CategoryId = request.CategoryId;
        item.BranchId = request.BranchId;
        item.Name = request.Name;
        item.NameAr = request.NameAr;
        item.Code = request.Code;
        item.Description = request.Description;
        item.DescriptionAr = request.DescriptionAr;
        item.DefaultPrice = request.DefaultPrice;
        item.CurrencyCode = request.CurrencyCode;
        item.TaxIncluded = request.TaxIncluded;
        item.AllowSizes = request.AllowSizes;
        item.KitchenStationId = request.KitchenStationId;
        item.ImageUrl = request.ImageUrl;
        item.Calories = request.Calories;
        item.PrepTimeMinutes = request.PrepTimeMinutes;
        item.Allergens = request.Allergens;
        item.IsActive = request.IsActive;
        item.IsAvailable = request.IsAvailable;
        item.IsFeatured = request.IsFeatured;
        item.UpdatedAt = DateTime.UtcNow;

        // Update sizes - remove old and add new
        _context.MenuItemSizes.RemoveRange(item.Sizes);
        foreach (var size in request.Sizes)
        {
            _context.MenuItemSizes.Add(new MenuItemSize
            {
                MenuItemId = id,
                SizeName = size.SizeName,
                Price = size.Price,
                CurrencyCode = size.CurrencyCode,
                Cost = size.Cost
            });
        }

        // Update modifiers
        var existingModifiers = await _context.MenuItemModifiers.Where(mm => mm.MenuItemId == id).ToListAsync();
        _context.MenuItemModifiers.RemoveRange(existingModifiers);
        foreach (var modifierId in request.ModifierIds)
        {
            _context.MenuItemModifiers.Add(new MenuItemModifier
            {
                MenuItemId = id,
                ModifierId = modifierId
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Menu item updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemId == id && m.CompanyId == companyId);

        if (item == null)
            return NotFound(new { message = "Menu item not found" });

        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Menu item deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemId == id && m.CompanyId == companyId);

        if (item == null)
            return NotFound(new { message = "Menu item not found" });

        item.IsActive = !item.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Menu item is now {(item.IsActive ? "active" : "inactive")}" });
    }

    [HttpPatch("{id}/availability")]
    public async Task<ActionResult> ToggleAvailability(int id)
    {
        var companyId = GetCompanyId();

        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.MenuItemId == id && m.CompanyId == companyId);

        if (item == null)
            return NotFound(new { message = "Menu item not found" });

        item.IsAvailable = !item.IsAvailable;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Menu item is now {(item.IsAvailable ? "available" : "86'd (unavailable)")}" });
    }
}
