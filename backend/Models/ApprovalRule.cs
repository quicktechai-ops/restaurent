using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("approval_rules")]
    public class ApprovalRule
    {
        [Key]
        [Column("approval_rule_id")]
        public int Id { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("rule_type")]
        [StringLength(30)]
        public string RuleType { get; set; } = string.Empty;

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("max_discount_percent_without_approval")]
        public decimal MaxDiscountPercent { get; set; } = 0;

        [Column("allow_price_change")]
        public bool AllowPriceChange { get; set; } = false;

        [Column("require_manager_pin_for_price_change")]
        public bool RequireManagerPinForPriceChange { get; set; } = true;

        [Column("can_void_paid_invoice")]
        public bool CanVoidPaidInvoice { get; set; } = false;

        [Column("require_manager_approval_for_void")]
        public bool RequireManagerApprovalForVoid { get; set; } = true;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}
