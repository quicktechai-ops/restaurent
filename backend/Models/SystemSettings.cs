using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class SystemSetting
{
    [Key]
    public int SettingId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? BranchId { get; set; } // null = global setting
    
    [Required]
    [MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;
    
    public string? SettingValue { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string SettingType { get; set; } = "String"; // String, Integer, Decimal, Boolean, JSON
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? UpdatedByUserId { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
    
    [ForeignKey("UpdatedByUserId")]
    public virtual User? UpdatedByUser { get; set; }
}

public class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }
    
    public int CompanyId { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public int? UserId { get; set; }
    
    public int? BranchId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty; // Login, CreateOrder, ApplyDiscount, etc.
    
    [MaxLength(50)]
    public string? EntityName { get; set; } // Order, StockAdjustment, etc.
    
    public int? EntityId { get; set; }
    
    public string? Details { get; set; } // JSON before/after
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}
