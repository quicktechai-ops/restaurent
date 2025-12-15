using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("kitchen_stations")]
public class KitchenStation
{
    [Key]
    [Column("kitchen_station_id")]
    public int KitchenStationId { get; set; }

    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Column("color")]
    [MaxLength(20)]
    public string? Color { get; set; }

    [Column("average_prep_time")]
    public int AveragePrepTime { get; set; } = 10;

    [Column("display_order")]
    public int DisplayOrder { get; set; } = 0;

    [Column("alert_after_minutes")]
    public int AlertAfterMinutes { get; set; } = 15;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation
    public Branch Branch { get; set; } = null!;
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
