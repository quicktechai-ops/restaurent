using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("modifiers")]
public class Modifier
{
    [Key]
    [Column("modifier_id")]
    public int ModifierId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("branch_id")]
    public int? BranchId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("name_ar")]
    [MaxLength(100)]
    public string? NameAr { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("extra_price")]
    public decimal ExtraPrice { get; set; } = 0;

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public Branch? Branch { get; set; }
    public ICollection<MenuItemModifier> MenuItemModifiers { get; set; } = new List<MenuItemModifier>();
}
