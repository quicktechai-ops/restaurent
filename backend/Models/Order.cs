using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("shift_id")]
    public int? ShiftId { get; set; }

    [Column("order_number")]
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Column("order_type")]
    [Required]
    [MaxLength(20)]
    public string OrderType { get; set; } = "DineIn"; // DineIn, Takeaway, Delivery

    [Column("table_id")]
    public int? TableId { get; set; }

    [Column("waiter_user_id")]
    public int? WaiterUserId { get; set; }

    [Column("cashier_user_id")]
    public int? CashierUserId { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [Column("order_status")]
    [Required]
    [MaxLength(20)]
    public string OrderStatus { get; set; } = "Draft"; // Draft, SentToKitchen, InPreparation, Ready, Served, Paid, Voided

    [Column("currency_code")]
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    [Column("exchange_rate_to_base")]
    public decimal ExchangeRateToBase { get; set; } = 1;

    [Column("sub_total")]
    public decimal SubTotal { get; set; } = 0;

    [Column("total_line_discount")]
    public decimal TotalLineDiscount { get; set; } = 0;

    [Column("bill_discount_percent")]
    public decimal BillDiscountPercent { get; set; } = 0;

    [Column("bill_discount_amount")]
    public decimal BillDiscountAmount { get; set; } = 0;

    [Column("service_charge_percent")]
    public decimal ServiceChargePercent { get; set; } = 0;

    [Column("service_charge_amount")]
    public decimal ServiceChargeAmount { get; set; } = 0;

    [Column("tax_percent")]
    public decimal TaxPercent { get; set; } = 0;

    [Column("tax_amount")]
    public decimal TaxAmount { get; set; } = 0;

    [Column("delivery_fee")]
    public decimal DeliveryFee { get; set; } = 0;

    [Column("tips_amount")]
    public decimal TipsAmount { get; set; } = 0;

    [Column("grand_total")]
    public decimal GrandTotal { get; set; } = 0;

    [Column("net_amount_for_loyalty")]
    public decimal? NetAmountForLoyalty { get; set; }

    [Column("loyalty_points_earned")]
    public decimal? LoyaltyPointsEarned { get; set; }

    [Column("loyalty_points_redeemed")]
    public decimal? LoyaltyPointsRedeemed { get; set; }

    [Column("loyalty_discount_amount")]
    public decimal LoyaltyDiscountAmount { get; set; } = 0;

    [Column("total_paid")]
    public decimal TotalPaid { get; set; } = 0;

    [Column("balance_due")]
    public decimal BalanceDue { get; set; } = 0;

    [Column("payment_status")]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, PartiallyPaid, Paid, Overpaid

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Column("voided_at")]
    public DateTime? VoidedAt { get; set; }

    [Column("void_reason")]
    [MaxLength(255)]
    public string? VoidReason { get; set; }

    [Column("void_by_user_id")]
    public int? VoidByUserId { get; set; }

    [Column("approved_void_by_user_id")]
    public int? ApprovedVoidByUserId { get; set; }

    [Column("merged_from_order_id")]
    public int? MergedFromOrderId { get; set; }

    [Column("split_from_order_id")]
    public int? SplitFromOrderId { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Shift? Shift { get; set; }
    public RestaurantTable? Table { get; set; }
    public User? WaiterUser { get; set; }
    public User? CashierUser { get; set; }
    public Customer? Customer { get; set; }
    public User? VoidByUser { get; set; }
    public User? ApprovedVoidByUser { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public OrderDeliveryDetails? DeliveryDetails { get; set; }
}
