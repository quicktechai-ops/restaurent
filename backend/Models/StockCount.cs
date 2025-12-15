using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("stock_counts")]
    public class StockCount
    {
        [Key]
        [Column("stock_count_id")]
        public int Id { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("count_date")]
        public DateTime CountDate { get; set; } = DateTime.UtcNow;

        [Column("area")]
        [StringLength(100)]
        public string? Area { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Completed";

        // Navigation properties
        public virtual ICollection<StockCountLine> Lines { get; set; } = new List<StockCountLine>();
    }

    [Table("stock_count_lines")]
    public class StockCountLine
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("stock_count_id")]
        public int StockCountId { get; set; }

        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Column("system_quantity")]
        public decimal SystemQuantity { get; set; }

        [Column("counted_quantity")]
        public decimal CountedQuantity { get; set; }

        [Column("variance")]
        public decimal Variance { get; set; }

        // Navigation properties
        [ForeignKey("StockCountId")]
        public virtual StockCount? StockCount { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}
