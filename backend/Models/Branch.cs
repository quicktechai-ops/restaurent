using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("branches")]
public class Branch
{
    [Key]
    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    [MaxLength(50)]
    public string? Code { get; set; }

    [Column("country")]
    [MaxLength(100)]
    public string? Country { get; set; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Column("address")]
    [MaxLength(255)]
    public string? Address { get; set; }

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("default_currency_code")]
    [MaxLength(3)]
    public string DefaultCurrencyCode { get; set; } = "USD";

    [Column("vat_percent")]
    public decimal VatPercent { get; set; } = 0;

    [Column("service_charge_percent")]
    public decimal ServiceChargePercent { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by_user_id")]
    public int? CreatedByUserId { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("updated_by_user_id")]
    public int? UpdatedByUserId { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public Currency? DefaultCurrency { get; set; }
    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
}
