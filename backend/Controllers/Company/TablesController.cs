using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,User")]
public class TablesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TablesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<TableListDto>>> GetAll([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();

        var query = _context.RestaurantTables
            .Include(t => t.Branch)
            .Where(t => t.Branch.CompanyId == companyId);

        if (branchId.HasValue)
        {
            query = query.Where(t => t.BranchId == branchId.Value);
        }

        var tables = await query
            .OrderBy(t => t.BranchId)
            .ThenBy(t => t.Zone)
            .ThenBy(t => t.TableName)
            .Select(t => new TableListDto
            {
                Id = t.TableId,
                TableName = t.TableName,
                Zone = t.Zone,
                Capacity = t.Capacity,
                Status = t.Status,
                FloorNumber = t.FloorNumber,
                BranchId = t.BranchId,
                BranchName = t.Branch.Name,
                IsActive = t.IsActive
            })
            .ToListAsync();

        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var table = await _context.RestaurantTables
            .Include(t => t.Branch)
            .Where(t => t.TableId == id && t.Branch.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (table == null)
            return NotFound(new { message = "Table not found" });

        return Ok(new TableListDto
        {
            Id = table.TableId,
            TableName = table.TableName,
            Zone = table.Zone,
            Capacity = table.Capacity,
            Status = table.Status,
            FloorNumber = table.FloorNumber,
            BranchId = table.BranchId,
            BranchName = table.Branch.Name,
            IsActive = table.IsActive
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<TableListDto>> Create([FromBody] CreateTableRequest request)
    {
        var companyId = GetCompanyId();

        // Verify branch belongs to company
        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchId == request.BranchId && b.CompanyId == companyId);

        if (branch == null)
            return BadRequest(new { message = "Invalid branch" });

        var table = new RestaurantTable
        {
            BranchId = request.BranchId,
            TableName = request.TableName,
            Zone = request.Zone,
            Capacity = request.Capacity,
            FloorNumber = request.FloorNumber,
            PositionX = request.PositionX,
            PositionY = request.PositionY,
            Status = "Available",
            IsActive = true
        };

        _context.RestaurantTables.Add(table);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = table.TableId }, new TableListDto
        {
            Id = table.TableId,
            TableName = table.TableName,
            Zone = table.Zone,
            Capacity = table.Capacity,
            BranchId = table.BranchId,
            IsActive = table.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTableRequest request)
    {
        var companyId = GetCompanyId();

        var table = await _context.RestaurantTables
            .Include(t => t.Branch)
            .FirstOrDefaultAsync(t => t.TableId == id && t.Branch.CompanyId == companyId);

        if (table == null)
            return NotFound(new { message = "Table not found" });

        table.TableName = request.TableName;
        table.Zone = request.Zone;
        table.Capacity = request.Capacity;
        table.FloorNumber = request.FloorNumber;
        table.PositionX = request.PositionX;
        table.PositionY = request.PositionY;
        table.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Table updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var table = await _context.RestaurantTables
            .Include(t => t.Branch)
            .FirstOrDefaultAsync(t => t.TableId == id && t.Branch.CompanyId == companyId);

        if (table == null)
            return NotFound(new { message = "Table not found" });

        _context.RestaurantTables.Remove(table);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Table deleted successfully" });
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var companyId = GetCompanyId();

        var table = await _context.RestaurantTables
            .Include(t => t.Branch)
            .FirstOrDefaultAsync(t => t.TableId == id && t.Branch.CompanyId == companyId);

        if (table == null)
            return NotFound(new { message = "Table not found" });

        var validStatuses = new[] { "Available", "Occupied", "Reserved", "NeedsCleaning", "OutOfService" };
        if (!validStatuses.Contains(status))
            return BadRequest(new { message = "Invalid status" });

        table.Status = status;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Table status updated to {status}" });
    }
}
