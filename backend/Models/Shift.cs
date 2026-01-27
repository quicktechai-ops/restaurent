using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("shifts")]
public class Shift
{
    [Key]
    [Column("shift_id")]
    public int ShiftId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("cashier_user_id")]
    public int CashierUserId { get; set; }

    [Column("open_time")]
    public DateTime OpenTime { get; set; } = DateTime.UtcNow;

    [Column("close_time")]
    public DateTime? CloseTime { get; set; }

    [Column("expected_close_time")]
    public DateTime? ExpectedCloseTime { get; set; }

    [Column("opening_cash")]
    public decimal OpeningCash { get; set; }

    [Column("closing_cash")]
    public decimal? ClosingCash { get; set; }

    [Column("expected_cash")]
    public decimal? ExpectedCash { get; set; }

    [Column("cash_difference")]
    public decimal? CashDifference { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Open"; // Open, Closed, ForceClosedBySystem

    [Column("force_closed_at")]
    public DateTime? ForceClosedAt { get; set; }

    [Column("force_close_reason")]
    [MaxLength(255)]
    public string? ForceCloseReason { get; set; }

    [Column("notes")]
    [MaxLength(500)]
    public string? Notes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public User CashierUser { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
