using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models;

[Table("restaurant_tables")]
public class RestaurantTable
{
    [Key]
    [Column("table_id")]
    public int TableId { get; set; }

    [Column("branch_id")]
    public int BranchId { get; set; }

    [Column("table_name")]
    [Required]
    [MaxLength(50)]
    public string TableName { get; set; } = string.Empty;

    [Column("zone")]
    [MaxLength(50)]
    public string? Zone { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Available"; // Available, Occupied, Reserved, NeedsCleaning

    [Column("floor_number")]
    public int FloorNumber { get; set; } = 1;

    [Column("position_x")]
    public int? PositionX { get; set; }

    [Column("position_y")]
    public int? PositionY { get; set; }

    [Column("section_id")]
    public int? SectionId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation
    public Branch Branch { get; set; } = null!;
}
