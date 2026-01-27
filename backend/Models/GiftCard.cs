using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("gift_cards")]
public class GiftCard
{
    [Key]
    [Column("gift_card_id")]
    public int GiftCardId { get; set; }
    
    [Column("company_id")]
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Column("gift_card_number")]
    public string GiftCardNumber { get; set; } = string.Empty;
    
    [Column("branch_issued_id")]
    public int BranchIssuedId { get; set; }
    
    [Column("issue_date")]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(3)]
    [Column("currency_code")]
    public string CurrencyCode { get; set; } = "USD";
    
    [Column("initial_value", TypeName = "decimal(18,2)")]
    public decimal InitialValue { get; set; }
    
    [Column("current_balance", TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }
    
    [Column("expiry_date")]
    public DateTime? ExpiryDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Active"; // Active, UsedUp, Expired, Blocked
    
    [Column("customer_id")]
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

[Table("gift_card_transactions")]
public class GiftCardTransaction
{
    [Key]
    [Column("gift_card_transaction_id")]
    public int GiftCardTransactionId { get; set; }
    
    [Column("gift_card_id")]
    public int GiftCardId { get; set; }
    
    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(20)]
    [Column("type")]
    public string Type { get; set; } = string.Empty; // Load, Redeem, Refund, Adjust
    
    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    [Column("currency_code")]
    public string CurrencyCode { get; set; } = "USD";
    
    [Column("balance_before", TypeName = "decimal(18,2)")]
    public decimal BalanceBefore { get; set; }
    
    [Column("balance_after", TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }
    
    [Column("order_id")]
    public int? OrderId { get; set; }
    
    [Column("user_id")]
    public int? UserId { get; set; }
    
    [MaxLength(255)]
    [Column("notes")]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey("GiftCardId")]
    public virtual GiftCard? GiftCard { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
