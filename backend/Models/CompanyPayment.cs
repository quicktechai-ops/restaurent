using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("company_payments")]
public class CompanyPayment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("payment_method")]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Cash";

    [Column("payment_reference")]
    [MaxLength(100)]
    public string? PaymentReference { get; set; }

    [Column("payment_date")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "completed";

    [Column("notes")]
    [MaxLength(255)]
    public string? Notes { get; set; }

    [Column("recorded_by_super_admin_id")]
    public int? RecordedBySuperAdminId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Company Company { get; set; } = null!;
    public SuperAdmin? RecordedBySuperAdmin { get; set; }
}
