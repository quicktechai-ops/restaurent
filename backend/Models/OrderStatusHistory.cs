using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("order_status_history")]
public class OrderStatusHistory
{
    [Key]
    [Column("order_status_history_id")]
    public int OrderStatusHistoryId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("old_status")]
    [MaxLength(20)]
    public string? OldStatus { get; set; }

    [Column("new_status")]
    [Required]
    [MaxLength(20)]
    public string NewStatus { get; set; } = string.Empty;

    [Column("changed_at")]
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("notes")]
    [MaxLength(255)]
    public string? Notes { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public User? User { get; set; }
}
