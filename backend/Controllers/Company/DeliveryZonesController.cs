using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/delivery-zones")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class DeliveryZonesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeliveryZonesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<DeliveryZoneListDto>>> GetAll([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();
        var query = _context.DeliveryZones
            .Include(d => d.Branch)
            .Where(d => d.Branch!.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(d => d.BranchId == branchId.Value);

        var zones = await query
            .OrderBy(d => d.Branch!.Name).ThenBy(d => d.ZoneName)
            .Select(d => new DeliveryZoneListDto
            {
                Id = d.DeliveryZoneId,
                BranchId = d.BranchId,
                BranchName = d.Branch!.Name,
                ZoneName = d.ZoneName,
                Description = d.Description,
                MinOrderAmount = d.MinOrderAmount,
                BaseFee = d.BaseFee,
                ExtraFeePerKm = d.ExtraFeePerKm,
                MaxDistanceKm = d.MaxDistanceKm,
                IsActive = d.IsActive
            })
            .ToListAsync();

        return Ok(zones);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryZoneListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var zone = await _context.DeliveryZones
            .Include(d => d.Branch)
            .FirstOrDefaultAsync(d => d.DeliveryZoneId == id && d.Branch!.CompanyId == companyId);

        if (zone == null) return NotFound();

        return Ok(new DeliveryZoneListDto
        {
            Id = zone.DeliveryZoneId,
            BranchId = zone.BranchId,
            BranchName = zone.Branch!.Name,
            ZoneName = zone.ZoneName,
            Description = zone.Description,
            MinOrderAmount = zone.MinOrderAmount,
            BaseFee = zone.BaseFee,
            ExtraFeePerKm = zone.ExtraFeePerKm,
            MaxDistanceKm = zone.MaxDistanceKm,
            IsActive = zone.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryZoneListDto>> Create([FromBody] CreateDeliveryZoneRequest request)
    {
        var companyId = GetCompanyId();

        var zone = new DeliveryZone
        {
            BranchId = request.BranchId,
            ZoneName = request.ZoneName,
            Description = request.Description,
            MinOrderAmount = request.MinOrderAmount,
            BaseFee = request.BaseFee,
            ExtraFeePerKm = request.ExtraFeePerKm,
            MaxDistanceKm = request.MaxDistanceKm
        };

        _context.DeliveryZones.Add(zone);
        await _context.SaveChangesAsync();

        return Ok(new DeliveryZoneListDto
        {
            Id = zone.DeliveryZoneId,
            BranchId = zone.BranchId,
            ZoneName = zone.ZoneName,
            BaseFee = zone.BaseFee,
            IsActive = zone.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DeliveryZoneListDto>> Update(int id, [FromBody] UpdateDeliveryZoneRequest request)
    {
        var companyId = GetCompanyId();
        var zone = await _context.DeliveryZones.FirstOrDefaultAsync(d => d.DeliveryZoneId == id && d.Branch!.CompanyId == companyId);
        if (zone == null) return NotFound();

        zone.BranchId = request.BranchId;
        zone.ZoneName = request.ZoneName;
        zone.Description = request.Description;
        zone.MinOrderAmount = request.MinOrderAmount;
        zone.BaseFee = request.BaseFee;
        zone.ExtraFeePerKm = request.ExtraFeePerKm;
        zone.MaxDistanceKm = request.MaxDistanceKm;
        zone.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new DeliveryZoneListDto
        {
            Id = zone.DeliveryZoneId,
            BranchId = zone.BranchId,
            ZoneName = zone.ZoneName,
            BaseFee = zone.BaseFee,
            IsActive = zone.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();
        var zone = await _context.DeliveryZones.FirstOrDefaultAsync(d => d.DeliveryZoneId == id && d.Branch!.CompanyId == companyId);
        if (zone == null) return NotFound();

        zone.IsActive = !zone.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { zone.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var zone = await _context.DeliveryZones.FirstOrDefaultAsync(d => d.DeliveryZoneId == id && d.Branch!.CompanyId == companyId);
        if (zone == null) return NotFound();

        _context.DeliveryZones.Remove(zone);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
