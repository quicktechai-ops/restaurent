using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("super_admins")]
public class SuperAdmin
{
    [Key]
    [Column("super_admin_id")]
    public int SuperAdminId { get; set; }

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

    [Column("email")]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
