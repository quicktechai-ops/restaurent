using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("categories")]
public class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("branch_id")]
    public int? BranchId { get; set; }

    [Column("parent_category_id")]
    public int? ParentCategoryId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("name_ar")]
    [MaxLength(100)]
    public string? NameAr { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("image")]
    [MaxLength(500)]
    public string? Image { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public Branch? Branch { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
