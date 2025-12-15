using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("menu_item_modifiers")]
public class MenuItemModifier
{
    [Key]
    [Column("menu_item_modifier_id")]
    public int MenuItemModifierId { get; set; }

    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("modifier_id")]
    public int ModifierId { get; set; }

    [Column("is_required")]
    public bool IsRequired { get; set; } = false;

    [Column("max_quantity")]
    public int? MaxQuantity { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    // Navigation
    public MenuItem MenuItem { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}
