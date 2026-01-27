using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("order_payments")]
public class OrderPayment
{
    [Key]
    [Column("order_payment_id")]
    public int OrderPaymentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [Column("currency_code")]
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("amount_in_order_currency")]
    public decimal AmountInOrderCurrency { get; set; }

    [Column("exchange_rate_to_order_currency")]
    public decimal ExchangeRateToOrderCurrency { get; set; } = 1;

    [Column("reference")]
    [MaxLength(100)]
    public string? Reference { get; set; } // Card approval code, etc.

    [Column("gift_card_id")]
    public int? GiftCardId { get; set; }

    [Column("loyalty_points_used")]
    public decimal? LoyaltyPointsUsed { get; set; }

    [Column("notes")]
    [MaxLength(255)]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("user_id")]
    public int? UserId { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public GiftCard? GiftCard { get; set; }
    public User? User { get; set; }
}
