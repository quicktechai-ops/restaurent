using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class InventoryCategory
{
    [Key]
    public int CategoryId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? ParentCategoryId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("ParentCategoryId")]
    public virtual InventoryCategory? ParentCategory { get; set; }
    
    public virtual ICollection<InventoryCategory> SubCategories { get; set; } = new List<InventoryCategory>();
}

public class UnitOfMeasure
{
    [Key]
    public int UnitId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty; // kg, g, liter
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty; // Kilogram, Gram, Liter
    
    [MaxLength(10)]
    public string? Symbol { get; set; } // kg, g, L
    
    [MaxLength(20)]
    public string? UnitGroup { get; set; } // Weight, Volume, Count - for grouping related units
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
}

public class UnitConversion
{
    [Key]
    public int ConversionId { get; set; }
    
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string FromUnitCode { get; set; } = string.Empty; // kg
    
    [Required]
    [MaxLength(20)]
    public string ToUnitCode { get; set; } = string.Empty; // g
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal ConversionFactor { get; set; } // 1000 (1 kg = 1000 g)
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
}
