using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Column("date")]
        public DateOnly Date { get; set; }

        [Column("clock_in")]
        public DateTime ClockIn { get; set; }

        [Column("clock_out")]
        public DateTime? ClockOut { get; set; }

        [Column("hours_worked")]
        public decimal? HoursWorked { get; set; }

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
