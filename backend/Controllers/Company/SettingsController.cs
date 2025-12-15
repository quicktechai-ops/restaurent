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
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SettingsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<SystemSettingDto>>> GetAll([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();
        var query = _context.SystemSettings
            .Include(s => s.Branch)
            .Where(s => s.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value || s.BranchId == null);
        else
            query = query.Where(s => s.BranchId == null);

        var settings = await query
            .OrderBy(s => s.SettingKey)
            .Select(s => new SystemSettingDto
            {
                Id = s.SettingId,
                BranchId = s.BranchId,
                BranchName = s.Branch != null ? s.Branch.Name : null,
                SettingKey = s.SettingKey,
                SettingValue = s.SettingValue,
                SettingType = s.SettingType,
                Description = s.Description,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        return Ok(settings);
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<SystemSettingDto>> GetByKey(string key, [FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();

        // Try branch-specific first, then global
        var setting = await _context.SystemSettings
            .Include(s => s.Branch)
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.SettingKey == key && s.BranchId == branchId);

        if (setting == null && branchId.HasValue)
        {
            setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.SettingKey == key && s.BranchId == null);
        }

        if (setting == null) return NotFound();

        return Ok(new SystemSettingDto
        {
            Id = setting.SettingId,
            BranchId = setting.BranchId,
            BranchName = setting.Branch?.Name,
            SettingKey = setting.SettingKey,
            SettingValue = setting.SettingValue,
            SettingType = setting.SettingType,
            Description = setting.Description,
            UpdatedAt = setting.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<SystemSettingDto>> CreateOrUpdate([FromBody] UpdateSystemSettingRequest request)
    {
        var companyId = GetCompanyId();
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var existing = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.SettingKey == request.SettingKey && s.BranchId == request.BranchId);

        if (existing != null)
        {
            existing.SettingValue = request.SettingValue;
            existing.SettingType = request.SettingType;
            existing.Description = request.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedByUserId = userId;
        }
        else
        {
            existing = new SystemSetting
            {
                CompanyId = companyId,
                BranchId = request.BranchId,
                SettingKey = request.SettingKey,
                SettingValue = request.SettingValue,
                SettingType = request.SettingType,
                Description = request.Description,
                UpdatedByUserId = userId
            };
            _context.SystemSettings.Add(existing);
        }

        await _context.SaveChangesAsync();

        return Ok(new SystemSettingDto
        {
            Id = existing.SettingId,
            BranchId = existing.BranchId,
            SettingKey = existing.SettingKey,
            SettingValue = existing.SettingValue,
            SettingType = existing.SettingType,
            UpdatedAt = existing.UpdatedAt
        });
    }

    [HttpPost("bulk")]
    public async Task<ActionResult> BulkUpdate([FromBody] List<UpdateSystemSettingRequest> requests)
    {
        var companyId = GetCompanyId();
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        foreach (var request in requests)
        {
            var existing = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.SettingKey == request.SettingKey && s.BranchId == request.BranchId);

            if (existing != null)
            {
                existing.SettingValue = request.SettingValue;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedByUserId = userId;
            }
            else
            {
                _context.SystemSettings.Add(new SystemSetting
                {
                    CompanyId = companyId,
                    BranchId = request.BranchId,
                    SettingKey = request.SettingKey,
                    SettingValue = request.SettingValue,
                    SettingType = request.SettingType,
                    Description = request.Description,
                    UpdatedByUserId = userId
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Settings updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingId == id && s.CompanyId == companyId);
        if (setting == null) return NotFound();

        _context.SystemSettings.Remove(setting);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Receipt Templates
    [HttpGet("receipt-templates")]
    public async Task<ActionResult<List<ReceiptTemplateListDto>>> GetReceiptTemplates([FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();
        var query = _context.ReceiptTemplates
            .Include(r => r.Branch)
            .Where(r => r.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(r => r.BranchId == branchId.Value || r.BranchId == null);

        var templates = await query
            .OrderBy(r => r.TemplateType).ThenBy(r => r.Name)
            .Select(r => new ReceiptTemplateListDto
            {
                Id = r.ReceiptTemplateId,
                BranchId = r.BranchId,
                BranchName = r.Branch != null ? r.Branch.Name : null,
                TemplateType = r.TemplateType,
                Name = r.Name,
                HeaderText = r.HeaderText,
                FooterText = r.FooterText,
                ShowLogo = r.ShowLogo,
                ShowBarcode = r.ShowBarcode,
                Language = r.Language,
                IsDefault = r.IsDefault,
                IsActive = r.IsActive
            })
            .ToListAsync();

        return Ok(templates);
    }

    [HttpPost("receipt-templates")]
    public async Task<ActionResult<ReceiptTemplateListDto>> CreateReceiptTemplate([FromBody] CreateReceiptTemplateRequest request)
    {
        var companyId = GetCompanyId();

        var template = new ReceiptTemplate
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            TemplateType = request.TemplateType,
            Name = request.Name,
            HeaderText = request.HeaderText,
            FooterText = request.FooterText,
            ShowLogo = request.ShowLogo,
            ShowBarcode = request.ShowBarcode,
            Language = request.Language,
            IsDefault = request.IsDefault
        };

        _context.ReceiptTemplates.Add(template);
        await _context.SaveChangesAsync();

        return Ok(new ReceiptTemplateListDto
        {
            Id = template.ReceiptTemplateId,
            Name = template.Name,
            TemplateType = template.TemplateType,
            IsActive = template.IsActive
        });
    }

    [HttpPut("receipt-templates/{id}")]
    public async Task<ActionResult<ReceiptTemplateListDto>> UpdateReceiptTemplate(int id, [FromBody] UpdateReceiptTemplateRequest request)
    {
        var companyId = GetCompanyId();
        var template = await _context.ReceiptTemplates.FirstOrDefaultAsync(r => r.ReceiptTemplateId == id && r.CompanyId == companyId);
        if (template == null) return NotFound();

        template.BranchId = request.BranchId;
        template.TemplateType = request.TemplateType;
        template.Name = request.Name;
        template.HeaderText = request.HeaderText;
        template.FooterText = request.FooterText;
        template.ShowLogo = request.ShowLogo;
        template.ShowBarcode = request.ShowBarcode;
        template.Language = request.Language;
        template.IsDefault = request.IsDefault;
        template.IsActive = request.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ReceiptTemplateListDto
        {
            Id = template.ReceiptTemplateId,
            Name = template.Name,
            IsActive = template.IsActive
        });
    }

    [HttpDelete("receipt-templates/{id}")]
    public async Task<IActionResult> DeleteReceiptTemplate(int id)
    {
        var companyId = GetCompanyId();
        var template = await _context.ReceiptTemplates.FirstOrDefaultAsync(r => r.ReceiptTemplateId == id && r.CompanyId == companyId);
        if (template == null) return NotFound();

        _context.ReceiptTemplates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
