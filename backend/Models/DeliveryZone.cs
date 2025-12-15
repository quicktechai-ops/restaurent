using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class DeliveryZone
{
    [Key]
    [Column("delivery_zone_id")]
    public int DeliveryZoneId { get; set; }
    
    [Column("branch_id")]
    public int BranchId { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("zone_name")]
    public string ZoneName { get; set; } = string.Empty;
    
    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("min_order_amount", TypeName = "decimal(18,2)")]
    public decimal? MinOrderAmount { get; set; }
    
    [Column("base_fee", TypeName = "decimal(18,2)")]
    public decimal BaseFee { get; set; } = 0;
    
    [Column("extra_fee_per_km", TypeName = "decimal(18,2)")]
    public decimal? ExtraFeePerKm { get; set; }
    
    [Column("max_distance_km", TypeName = "decimal(18,2)")]
    public decimal? MaxDistanceKm { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}
