using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("purchase_orders")]
    public class PurchaseOrder
    {
        [Key]
        [Column("purchase_order_id")]
        public int Id { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("po_number")]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("po_date")]
        public DateTime? PODate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Draft";

        [Column("expected_delivery_date")]
        public DateTime? ExpectedDate { get; set; }

        [Column("currency_code")]
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        [Column("total_estimated_amount")]
        public decimal TotalAmount { get; set; }

        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        public virtual ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
    }

    [Table("purchase_order_lines")]
    public class PurchaseOrderLine
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("purchase_order_id")]
        public int PurchaseOrderId { get; set; }

        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("received_quantity")]
        public decimal ReceivedQuantity { get; set; } = 0;

        // Navigation properties
        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}
