using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/loyalty")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class LoyaltyController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoyaltyController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    // Loyalty Settings
    [HttpGet("settings")]
    public async Task<ActionResult<List<LoyaltySettingsDto>>> GetSettings()
    {
        var companyId = GetCompanyId();
        var settings = await _context.LoyaltySettings
            .Include(l => l.Branch)
            .Where(l => l.CompanyId == companyId)
            .Select(l => new LoyaltySettingsDto
            {
                Id = l.LoyaltySettingsId,
                BranchId = l.BranchId,
                BranchName = l.Branch != null ? l.Branch.Name : null,
                PointsPerAmount = l.PointsPerAmount,
                AmountUnit = l.AmountUnit,
                EarnOnNetBeforeTax = l.EarnOnNetBeforeTax,
                PointsRedeemValue = l.PointsRedeemValue,
                PointsExpiryMonths = l.PointsExpiryMonths
            })
            .ToListAsync();

        return Ok(settings);
    }

    [HttpPost("settings")]
    public async Task<ActionResult<LoyaltySettingsDto>> CreateOrUpdateSettings([FromBody] UpdateLoyaltySettingsRequest request)
    {
        var companyId = GetCompanyId();

        var existing = await _context.LoyaltySettings
            .FirstOrDefaultAsync(l => l.CompanyId == companyId && l.BranchId == request.BranchId);

        if (existing != null)
        {
            existing.PointsPerAmount = request.PointsPerAmount;
            existing.AmountUnit = request.AmountUnit;
            existing.EarnOnNetBeforeTax = request.EarnOnNetBeforeTax;
            existing.PointsRedeemValue = request.PointsRedeemValue;
            existing.PointsExpiryMonths = request.PointsExpiryMonths;
        }
        else
        {
            existing = new LoyaltySettings
            {
                CompanyId = companyId,
                BranchId = request.BranchId,
                PointsPerAmount = request.PointsPerAmount,
                AmountUnit = request.AmountUnit,
                EarnOnNetBeforeTax = request.EarnOnNetBeforeTax,
                PointsRedeemValue = request.PointsRedeemValue,
                PointsExpiryMonths = request.PointsExpiryMonths
            };
            _context.LoyaltySettings.Add(existing);
        }

        await _context.SaveChangesAsync();

        return Ok(new LoyaltySettingsDto
        {
            Id = existing.LoyaltySettingsId,
            BranchId = existing.BranchId,
            PointsPerAmount = existing.PointsPerAmount,
            AmountUnit = existing.AmountUnit,
            PointsRedeemValue = existing.PointsRedeemValue
        });
    }

    // Loyalty Tiers
    [HttpGet("tiers")]
    public async Task<ActionResult<List<LoyaltyTierListDto>>> GetTiers()
    {
        var companyId = GetCompanyId();
        var tiers = await _context.LoyaltyTiers
            .Where(t => t.CompanyId == companyId)
            .OrderBy(t => t.Name)
            .Select(t => new LoyaltyTierListDto
            {
                Id = t.LoyaltyTierId,
                Name = t.Name,
                MinTotalSpent = t.MinTotalSpent,
                MinTotalPoints = t.MinTotalPoints,
                TierDiscountPercent = t.TierDiscountPercent,
                IsActive = t.IsActive,
                CustomersCount = _context.LoyaltyAccounts.Count(a => a.LoyaltyTierId == t.LoyaltyTierId)
            })
            .ToListAsync();

        return Ok(tiers);
    }

    [HttpPost("tiers")]
    public async Task<ActionResult<LoyaltyTierListDto>> CreateTier([FromBody] CreateLoyaltyTierRequest request)
    {
        var companyId = GetCompanyId();

        var tier = new LoyaltyTier
        {
            CompanyId = companyId,
            Name = request.Name,
            MinTotalSpent = request.MinTotalSpent,
            MinTotalPoints = request.MinTotalPoints,
            TierDiscountPercent = request.TierDiscountPercent
        };

        _context.LoyaltyTiers.Add(tier);
        await _context.SaveChangesAsync();

        return Ok(new LoyaltyTierListDto
        {
            Id = tier.LoyaltyTierId,
            Name = tier.Name,
            MinTotalSpent = tier.MinTotalSpent,
            TierDiscountPercent = tier.TierDiscountPercent,
            IsActive = tier.IsActive
        });
    }

    [HttpPut("tiers/{id}")]
    public async Task<ActionResult<LoyaltyTierListDto>> UpdateTier(int id, [FromBody] UpdateLoyaltyTierRequest request)
    {
        var companyId = GetCompanyId();
        var tier = await _context.LoyaltyTiers.FirstOrDefaultAsync(t => t.LoyaltyTierId == id && t.CompanyId == companyId);
        if (tier == null) return NotFound();

        tier.Name = request.Name;
        tier.MinTotalSpent = request.MinTotalSpent;
        tier.MinTotalPoints = request.MinTotalPoints;
        tier.TierDiscountPercent = request.TierDiscountPercent;
        tier.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new LoyaltyTierListDto
        {
            Id = tier.LoyaltyTierId,
            Name = tier.Name,
            IsActive = tier.IsActive
        });
    }

    [HttpDelete("tiers/{id}")]
    public async Task<IActionResult> DeleteTier(int id)
    {
        var companyId = GetCompanyId();
        var tier = await _context.LoyaltyTiers.FirstOrDefaultAsync(t => t.LoyaltyTierId == id && t.CompanyId == companyId);
        if (tier == null) return NotFound();

        _context.LoyaltyTiers.Remove(tier);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
