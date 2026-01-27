using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("order_line_modifiers")]
public class OrderLineModifier
{
    [Key]
    [Column("order_line_modifier_id")]
    public int OrderLineModifierId { get; set; }

    [Column("order_line_id")]
    public int OrderLineId { get; set; }

    [Column("modifier_id")]
    public int ModifierId { get; set; }

    [Column("quantity")]
    public decimal Quantity { get; set; } = 1;

    [Column("extra_price")]
    public decimal ExtraPrice { get; set; } // Per modifier unit

    [Column("total_price")]
    public decimal TotalPrice { get; set; } // ExtraPrice * Quantity

    // Navigation
    public OrderLine OrderLine { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}
