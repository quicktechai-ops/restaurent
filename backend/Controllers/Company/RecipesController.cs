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
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RecipesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<RecipeListDto>>> GetAll()
    {
        var companyId = GetCompanyId();
        var recipes = await _context.Recipes
            .Include(r => r.MenuItem)
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .Where(r => r.CompanyId == companyId)
            .Select(r => new RecipeListDto
            {
                Id = r.RecipeId,
                MenuItemId = r.MenuItemId,
                MenuItemName = r.MenuItem!.Name,
                Yield = (int)r.YieldQuantity,
                IngredientCount = r.Ingredients.Count,
                EstimatedCost = 0, // Cost calculation requires inventory transactions
                IsActive = r.IsActive
            })
            .ToListAsync();

        return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDetailDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var recipe = await _context.Recipes
            .Include(r => r.MenuItem)
            .Include(r => r.Ingredients)
                .ThenInclude(i => i.InventoryItem)
            .FirstOrDefaultAsync(r => r.RecipeId == id && r.CompanyId == companyId);

        if (recipe == null) return NotFound();

        return Ok(new RecipeDetailDto
        {
            Id = recipe.RecipeId,
            MenuItemId = recipe.MenuItemId,
            MenuItemName = recipe.MenuItem!.Name,
            Yield = (int)recipe.YieldQuantity,
            IsActive = recipe.IsActive,
            Ingredients = recipe.Ingredients.Select(i => new RecipeIngredientDto
            {
                Id = i.RecipeIngredientId,
                InventoryItemId = i.InventoryItemId,
                InventoryItemName = i.InventoryItem?.Name ?? "",
                Quantity = i.QuantityPerYield,
                Unit = i.InventoryItem?.UnitOfMeasure ?? ""
            }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<RecipeListDto>> Create([FromBody] CreateRecipeRequest request)
    {
        var companyId = GetCompanyId();

        // Check if recipe already exists for this menu item
        var existing = await _context.Recipes
            .FirstOrDefaultAsync(r => r.CompanyId == companyId && r.MenuItemId == request.MenuItemId);
        
        if (existing != null)
            return BadRequest("Recipe already exists for this menu item");

        var recipe = new Recipe
        {
            CompanyId = companyId,
            MenuItemId = request.MenuItemId,
            YieldQuantity = request.Yield,
            IsActive = true
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        // Add ingredients
        foreach (var ing in request.Ingredients)
        {
            var ingredient = new RecipeIngredient
            {
                RecipeId = recipe.RecipeId,
                InventoryItemId = ing.InventoryItemId,
                QuantityPerYield = ing.Quantity
            };
            _context.RecipeIngredients.Add(ingredient);
        }
        await _context.SaveChangesAsync();

        return Ok(new RecipeListDto
        {
            Id = recipe.RecipeId,
            MenuItemId = recipe.MenuItemId,
            Yield = (int)recipe.YieldQuantity,
            IngredientCount = request.Ingredients.Count,
            IsActive = recipe.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecipeListDto>> Update(int id, [FromBody] UpdateRecipeRequest request)
    {
        var companyId = GetCompanyId();
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.RecipeId == id && r.CompanyId == companyId);

        if (recipe == null) return NotFound();

        recipe.MenuItemId = request.MenuItemId;
        recipe.YieldQuantity = request.Yield;

        // Remove old ingredients
        _context.RecipeIngredients.RemoveRange(recipe.Ingredients);

        // Add new ingredients
        foreach (var ing in request.Ingredients)
        {
            var ingredient = new RecipeIngredient
            {
                RecipeId = recipe.RecipeId,
                InventoryItemId = ing.InventoryItemId,
                QuantityPerYield = ing.Quantity
            };
            _context.RecipeIngredients.Add(ingredient);
        }

        await _context.SaveChangesAsync();

        return Ok(new RecipeListDto
        {
            Id = recipe.RecipeId,
            MenuItemId = recipe.MenuItemId,
            Yield = (int)recipe.YieldQuantity,
            IngredientCount = request.Ingredients.Count,
            IsActive = recipe.IsActive
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.RecipeId == id && r.CompanyId == companyId);

        if (recipe == null) return NotFound();

        _context.RecipeIngredients.RemoveRange(recipe.Ingredients);
        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// DTOs for Recipes
public class RecipeListDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string? MenuItemName { get; set; }
    public int Yield { get; set; }
    public int IngredientCount { get; set; }
    public decimal EstimatedCost { get; set; }
    public bool IsActive { get; set; }
}

public class RecipeDetailDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string? MenuItemName { get; set; }
    public int Yield { get; set; }
    public bool IsActive { get; set; }
    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
}

public class RecipeIngredientDto
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateRecipeRequest
{
    public int MenuItemId { get; set; }
    public int Yield { get; set; } = 1;
    public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
}

public class UpdateRecipeRequest : CreateRecipeRequest { }

public class RecipeIngredientRequest
{
    public int InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
}
