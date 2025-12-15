using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class GiftCard
{
    [Key]
    public int GiftCardId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string GiftCardNumber { get; set; } = string.Empty;
    
    public int BranchIssuedId { get; set; }
    
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal InitialValue { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, UsedUp, Expired, Blocked
    
    public int? CustomerId { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("BranchIssuedId")]
    public virtual Branch? BranchIssued { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
    
    public virtual ICollection<GiftCardTransaction> Transactions { get; set; } = new List<GiftCardTransaction>();
}

public class GiftCardTransaction
{
    [Key]
    public int GiftCardTransactionId { get; set; }
    
    public int GiftCardId { get; set; }
    
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // Load, Redeem, Refund, Adjust
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceBefore { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }
    
    public int? OrderId { get; set; }
    
    public int? UserId { get; set; }
    
    [MaxLength(255)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey("GiftCardId")]
    public virtual GiftCard? GiftCard { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
