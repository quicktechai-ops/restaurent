using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("stock_adjustments")]
    public class StockAdjustment
    {
        [Key]
        [Column("stock_adjustment_id")]
        public int Id { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("adjustment_date")]
        public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Completed";

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
