using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("goods_receipts")]
    public class GoodsReceipt
    {
        [Key]
        [Column("goods_receipt_id")]
        public int Id { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("purchase_order_id")]
        public int? PurchaseOrderId { get; set; }

        [Column("grn_number")]
        [StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Column("grn_date")]
        public DateTime GRNDate { get; set; } = DateTime.UtcNow;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Received";

        [Column("currency_code")]
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        [Column("total_before_tax")]
        public decimal TotalBeforeTax { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }

        [Column("grand_total")]
        public decimal GrandTotal { get; set; }

        // Navigation properties
        [ForeignKey("SupplierId")]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        public virtual ICollection<GoodsReceiptLine> Lines { get; set; } = new List<GoodsReceiptLine>();
    }

    [Table("goods_receipt_lines")]
    public class GoodsReceiptLine
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("goods_receipt_id")]
        public int GoodsReceiptId { get; set; }

        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Column("received_quantity")]
        public decimal ReceivedQuantity { get; set; }

        [Column("unit_cost")]
        public decimal UnitCost { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        // Navigation properties
        [ForeignKey("GoodsReceiptId")]
        public virtual GoodsReceipt? GoodsReceipt { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}
