using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("companies")]
public class Company
{
    [Key]
    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("username")]
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Column("password_hash")]
    [Required]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Column("email")]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("address")]
    [MaxLength(255)]
    public string? Address { get; set; }

    [Column("logo")]
    [MaxLength(500)]
    public string? Logo { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "active";

    [Column("plan_id")]
    public int? PlanId { get; set; }

    [Column("plan_expiry_date")]
    public DateTime? PlanExpiryDate { get; set; }

    [Column("max_branches")]
    public int MaxBranches { get; set; } = 1;

    [Column("max_users")]
    public int MaxUsers { get; set; } = 5;

    [Column("trial_ends_at")]
    public DateTime? TrialEndsAt { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by_super_admin_id")]
    public int? CreatedBySuperAdminId { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public SubscriptionPlan? Plan { get; set; }
    public SuperAdmin? CreatedBySuperAdmin { get; set; }
    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
