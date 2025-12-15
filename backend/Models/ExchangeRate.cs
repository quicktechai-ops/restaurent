using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class ExchangeRate
{
    [Key]
    public int ExchangeRateId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string BaseCurrencyCode { get; set; } = string.Empty; // e.g., USD
    
    [Required]
    [MaxLength(3)]
    public string ForeignCurrencyCode { get; set; } = string.Empty; // e.g., LBP
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal Rate { get; set; } // 1 BaseCurrency = Rate * ForeignCurrency
    
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime? ValidTo { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("CreatedByUserId")]
    public virtual User? CreatedByUser { get; set; }
}
