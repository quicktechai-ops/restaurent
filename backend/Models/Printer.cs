using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class Printer
{
    [Key]
    public int PrinterId { get; set; }
    
    public int BranchId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty; // Kitchen Printer, Receipt Printer
    
    [Required]
    [MaxLength(20)]
    public string PrinterType { get; set; } = "Receipt"; // Receipt, Kitchen, Label
    
    [Required]
    [MaxLength(20)]
    public string ConnectionType { get; set; } = "Network"; // USB, Network, Bluetooth
    
    [MaxLength(255)]
    public string? ConnectionString { get; set; } // IP address, port, etc.
    
    public int PaperWidth { get; set; } = 80; // mm
    
    public bool IsDefault { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}

public class KitchenStationPrinter
{
    [Key]
    public int KitchenStationPrinterId { get; set; }
    
    public int KitchenStationId { get; set; }
    
    public int PrinterId { get; set; }
    
    // Navigation properties
    [ForeignKey("KitchenStationId")]
    public virtual KitchenStation? KitchenStation { get; set; }
    
    [ForeignKey("PrinterId")]
    public virtual Printer? Printer { get; set; }
}

public class ReceiptTemplate
{
    [Key]
    public int ReceiptTemplateId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? BranchId { get; set; } // null = global
    
    [Required]
    [MaxLength(20)]
    public string TemplateType { get; set; } = "CustomerReceipt"; // CustomerReceipt, KitchenTicket, DailyReport
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? HeaderText { get; set; }
    
    [MaxLength(500)]
    public string? FooterText { get; set; }
    
    public bool ShowLogo { get; set; } = true;
    
    public bool ShowBarcode { get; set; } = false;
    
    [MaxLength(10)]
    public string Language { get; set; } = "en"; // en, ar, both
    
    public bool IsDefault { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}
