using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("menu_item_sizes")]
public class MenuItemSize
{
    [Key]
    [Column("menu_item_size_id")]
    public int MenuItemSizeId { get; set; }

    [Column("menu_item_id")]
    public int MenuItemId { get; set; }

    [Column("size_name")]
    [Required]
    [MaxLength(50)]
    public string SizeName { get; set; } = string.Empty;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("cost")]
    public decimal? Cost { get; set; }

    // Navigation
    public MenuItem MenuItem { get; set; } = null!;
}
