using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("order_lines")]
public class OrderLine
{
    [Key]
    [Column("order_line_id")]
    public int OrderLineId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("menu_item_size_id")]
    public int? MenuItemSizeId { get; set; }

    [Column("quantity")]
    public decimal Quantity { get; set; } = 1;

    [Column("base_unit_price")]
    public decimal BaseUnitPrice { get; set; }

    [Column("modifiers_extra_price")]
    public decimal ModifiersExtraPrice { get; set; } = 0;

    [Column("effective_unit_price")]
    public decimal EffectiveUnitPrice { get; set; } // BaseUnitPrice + ModifiersExtraPrice

    [Column("line_gross")]
    public decimal LineGross { get; set; } // EffectiveUnitPrice * Quantity

    [Column("discount_percent")]
    public decimal DiscountPercent { get; set; } = 0;

    [Column("discount_amount")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column("line_net")]
    public decimal LineNet { get; set; } // LineGross - DiscountAmount

    [Column("notes")]
    [MaxLength(255)]
    public string? Notes { get; set; }

    [Column("kitchen_status")]
    [MaxLength(20)]
    public string KitchenStatus { get; set; } = "New"; // New, SentToKitchen, InProgress, Ready, Served, Cancelled

    [Column("kitchen_station_id")]
    public int? KitchenStationId { get; set; }

    [Column("sent_to_kitchen_at")]
    public DateTime? SentToKitchenAt { get; set; }

    [Column("ready_at")]
    public DateTime? ReadyAt { get; set; }

    [Column("served_at")]
    public DateTime? ServedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by_user_id")]
    public int? CreatedByUserId { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
    public MenuItemSize? MenuItemSize { get; set; }
    public KitchenStation? KitchenStation { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<OrderLineModifier> OrderLineModifiers { get; set; } = new List<OrderLineModifier>();
}
