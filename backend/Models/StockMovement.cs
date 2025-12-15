using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("stock_movements")]
    public class StockMovement
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Column("movement_type")]
        [StringLength(30)]
        public string MovementType { get; set; } = string.Empty; // IN-Purchase, IN-Adjustment, IN-Transfer, OUT-Sales, OUT-Waste, OUT-Adjustment, OUT-Transfer

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_cost")]
        public decimal? UnitCost { get; set; }

        [Column("reference")]
        [StringLength(100)]
        public string? Reference { get; set; }

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Column("created_by")]
        public int CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}
