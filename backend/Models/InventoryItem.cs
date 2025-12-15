using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class InventoryItem
{
    [Key]
    public int InventoryItemId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Code { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string UnitOfMeasure { get; set; } = string.Empty; // kg, g, liter, pcs
    
    [MaxLength(50)]
    public string? Category { get; set; } // Food, Beverage, Packaging
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal MinLevel { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal ReorderQty { get; set; } = 0;
    
    [MaxLength(20)]
    public string CostMethod { get; set; } = "Average"; // Average, Last
    
    [Column("quantity", TypeName = "decimal(18,4)")]
    public decimal Quantity { get; set; } = 0; // Current stock quantity
    
    [Column("cost", TypeName = "decimal(18,4)")]
    public decimal Cost { get; set; } = 0; // Unit cost
    
    [Column("currency_code")]
    [MaxLength(3)]
    public string? CurrencyCode { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("CurrencyCode")]
    public virtual Currency? Currency { get; set; }
    
    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}

public class Recipe
{
    [Key]
    public int RecipeId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int MenuItemId { get; set; }
    
    public int? MenuItemSizeId { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal YieldQuantity { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("MenuItemId")]
    public virtual MenuItem? MenuItem { get; set; }
    
    [ForeignKey("MenuItemSizeId")]
    public virtual MenuItemSize? MenuItemSize { get; set; }
    
    public virtual ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}

public class RecipeIngredient
{
    [Key]
    public int RecipeIngredientId { get; set; }
    
    public int RecipeId { get; set; }
    
    public int InventoryItemId { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal QuantityPerYield { get; set; }
    
    // Navigation properties
    [ForeignKey("RecipeId")]
    public virtual Recipe? Recipe { get; set; }
    
    [ForeignKey("InventoryItemId")]
    public virtual InventoryItem? InventoryItem { get; set; }
}
