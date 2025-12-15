using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

public class Employee
{
    [Key]
    public int EmployeeId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? UserId { get; set; } // If they log in to system
    
    public int BranchId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Position { get; set; } = string.Empty; // Waiter, Chef, Cashier
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? BaseSalary { get; set; }
    
    public DateTime? HireDate { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}

public class CommissionPolicy
{
    [Key]
    public int CommissionPolicyId { get; set; }
    
    public int CompanyId { get; set; }
    
    public int? BranchId { get; set; } // null = global
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal SalesPercent { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FixedPerInvoice { get; set; } = 0;
    
    public bool ApplyOnNetBeforeTax { get; set; } = true;
    
    public bool ExcludeDiscountedInvoices { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    
    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
}
