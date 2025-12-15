using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class LoyaltySettings
{
    [Key]
    public int LoyaltySettingsId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? BranchId { get; set; } // null = global
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal PointsPerAmount { get; set; } = 1; // e.g., 1 point
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal AmountUnit { get; set; } = 10; // per 10 currency
    
    public bool EarnOnNetBeforeTax { get; set; } = true;
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal PointsRedeemValue { get; set; } = 0.1m; // each point = 0.1
    
    public int? PointsExpiryMonths { get; set; } // null = no expiry
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}

public class LoyaltyTier
{
    [Key]
    public int LoyaltyTierId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty; // Bronze, Silver, Gold
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinTotalSpent { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinTotalPoints { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal TierDiscountPercent { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
}

public class LoyaltyAccount
{
    [Key]
    public int LoyaltyAccountId { get; set; }
    
    public int CustomerId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PointsBalance { get; set; } = 0;
    
    public int? LoyaltyTierId { get; set; }
    
    public DateTime? TierAssignedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
    
    [ForeignKey("LoyaltyTierId")]
    public virtual LoyaltyTier? LoyaltyTier { get; set; }
}

public class LoyaltyTransaction
{
    [Key]
    public int LoyaltyTransactionId { get; set; }
    
    public int LoyaltyAccountId { get; set; }
    
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // Earn, Redeem, Adjust, Expire
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PointsChange { get; set; } // +/-
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PointsBefore { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PointsAfter { get; set; }
    
    public int? OrderId { get; set; }
    
    [MaxLength(255)]
    public string? Notes { get; set; }
    
    public int? UserId { get; set; }
    
    // Navigation properties
    [ForeignKey("LoyaltyAccountId")]
    public virtual LoyaltyAccount? LoyaltyAccount { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
