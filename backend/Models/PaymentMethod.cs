using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("payment_methods")]
public class PaymentMethod
{
    [Key]
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column("type")]
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "Cash"; // Cash, Card, GiftCard, LoyaltyPoints, Other

    [Column("requires_reference")]
    public bool RequiresReference { get; set; } = false;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
}
