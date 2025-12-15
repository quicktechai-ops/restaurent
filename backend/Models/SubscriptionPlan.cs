using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("subscription_plans")]
public class SubscriptionPlan
{
    [Key]
    [Column("plan_id")]
    public int PlanId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("billing_cycle")]
    [MaxLength(20)]
    public string BillingCycle { get; set; } = "Monthly";

    [Column("duration_days")]
    public int DurationDays { get; set; }

    [Column("max_branches")]
    public int MaxBranches { get; set; }

    [Column("max_users")]
    public int MaxUsers { get; set; }

    [Column("max_orders_per_month")]
    public int? MaxOrdersPerMonth { get; set; }

    [Column("features", TypeName = "jsonb")]
    public string? Features { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
