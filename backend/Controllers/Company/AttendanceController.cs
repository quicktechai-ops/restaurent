using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/attendance")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttendanceController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? date, [FromQuery] int? employeeId)
    {
        var companyId = GetCompanyId();
        var query = _context.Attendances
            .Include(a => a.Employee)
            .Where(a => a.CompanyId == companyId);

        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
            query = query.Where(a => a.Date == parsedDate);

        if (employeeId.HasValue)
            query = query.Where(a => a.Employee.EmployeeId == employeeId.Value);

        var attendance = await query
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.ClockIn)
            .Take(500)
            .Select(a => new
            {
                a.Id,
                a.EmployeeId,
                EmployeeName = a.Employee != null ? a.Employee.FullName : null,
                Date = a.Date.ToString("yyyy-MM-dd"),
                a.ClockIn,
                a.ClockOut,
                a.HoursWorked
            })
            .ToListAsync();

        return Ok(attendance);
    }

    [HttpPost("clock-in")]
    public async Task<ActionResult> ClockIn([FromBody] ClockInRequest request)
    {
        var companyId = GetCompanyId();

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId && e.CompanyId == companyId);
        if (employee == null) return NotFound("Employee not found");

        // Check if already clocked in today
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await _context.Attendances.FirstOrDefaultAsync(a => 
            a.EmployeeId == request.EmployeeId && 
            a.Date == today && 
            a.ClockOut == null);

        if (existing != null) return BadRequest("Employee already clocked in");

        var attendance = new Attendance
        {
            CompanyId = companyId,
            EmployeeId = request.EmployeeId,
            Date = today,
            ClockIn = DateTime.UtcNow
        };

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();

        return Ok(new { attendance.Id });
    }

    [HttpPatch("{id}/clock-out")]
    public async Task<ActionResult> ClockOut(int id)
    {
        var companyId = GetCompanyId();
        var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == companyId);

        if (attendance == null) return NotFound();
        if (attendance.ClockOut != null) return BadRequest("Already clocked out");

        attendance.ClockOut = DateTime.UtcNow;
        attendance.HoursWorked = (decimal)(attendance.ClockOut.Value - attendance.ClockIn).TotalHours;

        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class ClockInRequest
{
    public int EmployeeId { get; set; }
}
