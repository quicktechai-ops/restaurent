using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("order_delivery_details")]
public class OrderDeliveryDetails
{
    [Key]
    [Column("order_delivery_details_id")]
    public int OrderDeliveryDetailsId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("customer_address_id")]
    public int? CustomerAddressId { get; set; }

    [Column("delivery_zone_id")]
    public int? DeliveryZoneId { get; set; }

    [Column("address_line")]
    [MaxLength(500)]
    public string? AddressLine { get; set; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Column("area")]
    [MaxLength(100)]
    public string? Area { get; set; }

    [Column("phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Column("distance_km")]
    public decimal? DistanceKm { get; set; }

    [Column("delivery_fee_calculated")]
    public decimal DeliveryFeeCalculated { get; set; } = 0;

    [Column("driver_name")]
    [MaxLength(100)]
    public string? DriverName { get; set; }

    [Column("driver_phone")]
    [MaxLength(50)]
    public string? DriverPhone { get; set; }

    [Column("estimated_delivery_time")]
    public DateTime? EstimatedDeliveryTime { get; set; }

    [Column("out_for_delivery_at")]
    public DateTime? OutForDeliveryAt { get; set; }

    [Column("delivered_at")]
    public DateTime? DeliveredAt { get; set; }

    [Column("delivery_notes")]
    [MaxLength(500)]
    public string? DeliveryNotes { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public CustomerAddress? CustomerAddress { get; set; }
    public DeliveryZone? DeliveryZone { get; set; }
}
