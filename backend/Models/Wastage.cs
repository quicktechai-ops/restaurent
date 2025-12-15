using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("wastages")]
    public class Wastage
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

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_cost")]
        public decimal UnitCost { get; set; }

        [Column("cost_impact")]
        public decimal CostImpact { get; set; }

        [Column("reason")]
        [StringLength(50)]
        public string Reason { get; set; } = string.Empty; // Expired, Damaged, Spoiled, Spilled, Theft, Quality, Other

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Column("recorded_by")]
        public int RecordedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}
