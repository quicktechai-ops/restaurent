using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("menu_items")]
public class MenuItem
{
    [Key]
    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("branch_id")]
    public int? BranchId { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("name_ar")]
    [MaxLength(100)]
    public string? NameAr { get; set; }

    [Column("code")]
    [MaxLength(50)]
    public string? Code { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("description_ar")]
    [MaxLength(255)]
    public string? DescriptionAr { get; set; }

    [Column("default_price")]
    public decimal DefaultPrice { get; set; }

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("tax_included")]
    public bool TaxIncluded { get; set; } = false;

    [Column("allow_sizes")]
    public bool AllowSizes { get; set; } = false;

    [Column("has_recipe")]
    public bool HasRecipe { get; set; } = false;

    [Column("kitchen_station_id")]
    public int? KitchenStationId { get; set; }

    [Column("is_visible_online_menu")]
    public bool IsVisibleOnlineMenu { get; set; } = true;

    [Column("item_commission_per_unit")]
    public decimal? ItemCommissionPerUnit { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("image_url")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Column("calories")]
    public int? Calories { get; set; }

    [Column("prep_time_minutes")]
    public int? PrepTimeMinutes { get; set; }

    [Column("allergens")]
    [MaxLength(255)]
    public string? Allergens { get; set; }

    [Column("is_available")]
    public bool IsAvailable { get; set; } = true;

    [Column("is_featured")]
    public bool IsFeatured { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public Branch? Branch { get; set; }
    public Category Category { get; set; } = null!;
    public KitchenStation? KitchenStation { get; set; }
    public ICollection<MenuItemSize> Sizes { get; set; } = new List<MenuItemSize>();
    public ICollection<MenuItemModifier> MenuItemModifiers { get; set; } = new List<MenuItemModifier>();
}
