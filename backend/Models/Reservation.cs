using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class Reservation
{
    [Key]
    [Column("reservation_id")]
    public int ReservationId { get; set; }
    
    [Column("branch_id")]
    public int BranchId { get; set; }
    
    [Column("customer_id")]
    public int? CustomerId { get; set; }
    
    [MaxLength(100)]
    [Column("customer_name")]
    public string? CustomerName { get; set; }
    
    [MaxLength(50)]
    [Column("customer_phone")]
    public string? CustomerPhone { get; set; }
    
    [Column("reservation_date")]
    public DateTime ReservationDate { get; set; }
    
    [Column("start_time")]
    public TimeSpan StartTime { get; set; }
    
    [Column("duration_minutes")]
    public int DurationMinutes { get; set; } = 90;
    
    [Column("party_size")]
    public int PartySize { get; set; }
    
    [Column("table_id")]
    public int? TableId { get; set; }
    
    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "Pending";
    
    [Required]
    [MaxLength(20)]
    [Column("channel")]
    public string Channel { get; set; } = "Phone";
    
    [MaxLength(255)]
    [Column("notes")]
    public string? Notes { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("created_by_user_id")]
    public int? CreatedByUserId { get; set; }
    
    // Navigation properties
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
    
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
    
    [ForeignKey("TableId")]
    public virtual RestaurantTable? Table { get; set; }
    
    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }
    
    public virtual ReservationDeposit? Deposit { get; set; }
}

public class ReservationDeposit
{
    [Key]
    public int ReservationDepositId { get; set; }
    
    public int ReservationId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, Forfeited, Refunded
    
    public int? PaymentMethodId { get; set; }
    
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public int? UserId { get; set; }
    
    // Navigation properties
    [ForeignKey("ReservationId")]
    public virtual Reservation? Reservation { get; set; }
    
    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
