using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("username")]
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Column("password_hash")]
    [Required]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Column("full_name")]
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("email")]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Column("default_branch_id")]
    public int? DefaultBranchId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

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
    public Branch? DefaultBranch { get; set; }
}
