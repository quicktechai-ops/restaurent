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
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryListDto>>> GetAll([FromQuery] bool? isActive)
    {
        var companyId = GetCompanyId();

        var query = _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.CompanyId == companyId);

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var categories = await query
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryListDto
            {
                Id = c.CategoryId,
                Name = c.Name,
                NameAr = c.NameAr,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                Image = c.Image,
                ItemsCount = _context.MenuItems.Count(m => m.CategoryId == c.CategoryId)
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.CategoryId == id && c.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (category == null)
            return NotFound(new { message = "Category not found" });

        return Ok(new CategoryListDto
        {
            Id = category.CategoryId,
            Name = category.Name,
            NameAr = category.NameAr,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            Image = category.Image
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<CategoryListDto>> Create([FromBody] CreateCategoryRequest request)
    {
        var companyId = GetCompanyId();

        var category = new Category
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            Name = request.Name,
            NameAr = request.NameAr,
            ParentCategoryId = request.ParentCategoryId,
            SortOrder = request.SortOrder,
            Image = request.Image,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, new CategoryListDto
        {
            Id = category.CategoryId,
            Name = category.Name,
            NameAr = category.NameAr,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            Image = category.Image
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        category.Name = request.Name;
        category.NameAr = request.NameAr;
        category.ParentCategoryId = request.ParentCategoryId;
        category.BranchId = request.BranchId;
        category.SortOrder = request.SortOrder;
        category.Image = request.Image;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Category updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        // Check if category has items
        var hasItems = await _context.MenuItems.AnyAsync(m => m.CategoryId == id);
        if (hasItems)
            return BadRequest(new { message = "Cannot delete category that has menu items" });

        // Check if category has subcategories
        var hasSubcategories = await _context.Categories.AnyAsync(c => c.ParentCategoryId == id);
        if (hasSubcategories)
            return BadRequest(new { message = "Cannot delete category that has subcategories" });

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Category deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CompanyId == companyId);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        category.IsActive = !category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Category is now {(category.IsActive ? "active" : "inactive")}" });
    }
}
