using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/kitchen-stations")]
[Authorize(Roles = "CompanyAdmin,User")]
public class KitchenStationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public KitchenStationsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<KitchenStationListDto>>> GetAll([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();

        var query = _context.KitchenStations
            .Include(k => k.Branch)
            .Where(k => k.Branch.CompanyId == companyId);

        if (branchId.HasValue)
        {
            query = query.Where(k => k.BranchId == branchId.Value);
        }

        var stations = await query
            .OrderBy(k => k.DisplayOrder)
            .Select(k => new KitchenStationListDto
            {
                Id = k.KitchenStationId,
                Name = k.Name,
                Color = k.Color,
                AveragePrepTime = k.AveragePrepTime,
                DisplayOrder = k.DisplayOrder,
                BranchId = k.BranchId,
                BranchName = k.Branch.Name,
                IsActive = k.IsActive,
                MenuItemsCount = _context.MenuItems.Count(m => m.KitchenStationId == k.KitchenStationId)
            })
            .ToListAsync();

        return Ok(stations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<KitchenStationListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var station = await _context.KitchenStations
            .Include(k => k.Branch)
            .Where(k => k.KitchenStationId == id && k.Branch.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (station == null)
            return NotFound(new { message = "Kitchen station not found" });

        return Ok(new KitchenStationListDto
        {
            Id = station.KitchenStationId,
            Name = station.Name,
            Color = station.Color,
            AveragePrepTime = station.AveragePrepTime,
            DisplayOrder = station.DisplayOrder,
            BranchId = station.BranchId,
            BranchName = station.Branch.Name,
            IsActive = station.IsActive
        });
    }

    [HttpPost]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult<KitchenStationListDto>> Create([FromBody] CreateKitchenStationRequest request)
    {
        var companyId = GetCompanyId();

        // Verify branch belongs to company
        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.BranchId == request.BranchId && b.CompanyId == companyId);

        if (branch == null)
            return BadRequest(new { message = "Invalid branch" });

        var station = new KitchenStation
        {
            BranchId = request.BranchId,
            Name = request.Name,
            Color = request.Color,
            AveragePrepTime = request.AveragePrepTime,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        _context.KitchenStations.Add(station);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = station.KitchenStationId }, new KitchenStationListDto
        {
            Id = station.KitchenStationId,
            Name = station.Name,
            BranchId = station.BranchId,
            IsActive = station.IsActive
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateKitchenStationRequest request)
    {
        var companyId = GetCompanyId();

        var station = await _context.KitchenStations
            .Include(k => k.Branch)
            .FirstOrDefaultAsync(k => k.KitchenStationId == id && k.Branch.CompanyId == companyId);

        if (station == null)
            return NotFound(new { message = "Kitchen station not found" });

        station.Name = request.Name;
        station.Color = request.Color;
        station.AveragePrepTime = request.AveragePrepTime;
        station.DisplayOrder = request.DisplayOrder;
        station.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Kitchen station updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var station = await _context.KitchenStations
            .Include(k => k.Branch)
            .FirstOrDefaultAsync(k => k.KitchenStationId == id && k.Branch.CompanyId == companyId);

        if (station == null)
            return NotFound(new { message = "Kitchen station not found" });

        // Check if station has menu items
        var hasItems = await _context.MenuItems.AnyAsync(m => m.KitchenStationId == id);
        if (hasItems)
            return BadRequest(new { message = "Cannot delete station that has menu items assigned" });

        _context.KitchenStations.Remove(station);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Kitchen station deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "CompanyAdmin")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var station = await _context.KitchenStations
            .Include(k => k.Branch)
            .FirstOrDefaultAsync(k => k.KitchenStationId == id && k.Branch.CompanyId == companyId);

        if (station == null)
            return NotFound(new { message = "Kitchen station not found" });

        station.IsActive = !station.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Kitchen station is now {(station.IsActive ? "active" : "inactive")}" });
    }
}
