using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<EmployeeListDto>>> GetAll([FromQuery] int? branchId, [FromQuery] string? position)
    {
        var companyId = GetCompanyId();
        var query = _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.User)
            .Where(e => e.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(position))
            query = query.Where(e => e.Position == position);

        var employees = await query
            .OrderBy(e => e.FullName)
            .Select(e => new EmployeeListDto
            {
                Id = e.EmployeeId,
                UserId = e.UserId,
                Username = e.User != null ? e.User.Username : null,
                BranchId = e.BranchId,
                BranchName = e.Branch!.Name,
                FullName = e.FullName,
                Position = e.Position,
                BaseSalary = e.BaseSalary,
                HireDate = e.HireDate,
                Phone = e.Phone,
                Email = e.Email,
                IsActive = e.IsActive
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var employee = await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == id && e.CompanyId == companyId);

        if (employee == null) return NotFound();

        return Ok(new EmployeeListDto
        {
            Id = employee.EmployeeId,
            UserId = employee.UserId,
            Username = employee.User?.Username,
            BranchId = employee.BranchId,
            BranchName = employee.Branch!.Name,
            FullName = employee.FullName,
            Position = employee.Position,
            BaseSalary = employee.BaseSalary,
            HireDate = employee.HireDate,
            Phone = employee.Phone,
            Email = employee.Email,
            IsActive = employee.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeListDto>> Create([FromBody] CreateEmployeeRequest request)
    {
        var companyId = GetCompanyId();

        var employee = new Employee
        {
            CompanyId = companyId,
            UserId = request.UserId,
            BranchId = request.BranchId,
            FullName = request.FullName,
            Position = request.Position,
            BaseSalary = request.BaseSalary,
            HireDate = request.HireDate,
            Phone = request.Phone,
            Email = request.Email
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Ok(new EmployeeListDto
        {
            Id = employee.EmployeeId,
            BranchId = employee.BranchId,
            FullName = employee.FullName,
            Position = employee.Position,
            IsActive = employee.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeListDto>> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        var companyId = GetCompanyId();
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.CompanyId == companyId);
        if (employee == null) return NotFound();

        employee.UserId = request.UserId;
        employee.BranchId = request.BranchId;
        employee.FullName = request.FullName;
        employee.Position = request.Position;
        employee.BaseSalary = request.BaseSalary;
        employee.HireDate = request.HireDate;
        employee.Phone = request.Phone;
        employee.Email = request.Email;
        employee.IsActive = request.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new EmployeeListDto
        {
            Id = employee.EmployeeId,
            FullName = employee.FullName,
            Position = employee.Position,
            IsActive = employee.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.CompanyId == companyId);
        if (employee == null) return NotFound();

        employee.IsActive = !employee.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { employee.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id && e.CompanyId == companyId);
        if (employee == null) return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("positions")]
    public async Task<ActionResult<List<string>>> GetPositions()
    {
        var companyId = GetCompanyId();
        var positions = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => e.Position)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();

        return Ok(positions);
    }
}
