using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }
    
    public int CompanyId { get; set; }
    
    [MaxLength(50)]
    public string? CustomerCode { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    public int? DefaultBranchId { get; set; }
    
    [MaxLength(3)]
    public string? DefaultCurrencyCode { get; set; }
    
    [MaxLength(255)]
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("DefaultBranchId")]
    public virtual Branch? DefaultBranch { get; set; }
    
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}

public class CustomerAddress
{
    [Key]
    public int CustomerAddressId { get; set; }
    
    public int CustomerId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Label { get; set; } = string.Empty; // Home, Work, etc.
    
    [Required]
    [MaxLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? AddressLine2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Area { get; set; }
    
    [Column(TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }
    
    [Column(TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }
    
    public int? DeliveryZoneId { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    // Navigation properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
    
    [ForeignKey("DeliveryZoneId")]
    public virtual DeliveryZone? DeliveryZone { get; set; }
}
