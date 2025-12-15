using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.DTOs;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/receipt-templates")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class ReceiptTemplatesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReceiptTemplatesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var companyId = GetCompanyId();
        var templates = await _context.ReceiptTemplates
            .Where(rt => rt.CompanyId == companyId)
            .OrderBy(rt => rt.TemplateType)
            .ThenBy(rt => rt.Name)
            .Select(rt => new
            {
                Id = rt.ReceiptTemplateId,
                rt.Name,
                Type = rt.TemplateType,
                rt.Language,
                rt.ShowLogo,
                rt.ShowBarcode,
                rt.HeaderText,
                rt.FooterText,
                rt.IsDefault,
                rt.IsActive
            })
            .ToListAsync();

        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var template = await _context.ReceiptTemplates.FirstOrDefaultAsync(rt => rt.ReceiptTemplateId == id && rt.CompanyId == companyId);
        if (template == null) return NotFound();

        return Ok(new
        {
            Id = template.ReceiptTemplateId,
            template.Name,
            Type = template.TemplateType,
            template.Language,
            template.ShowLogo,
            template.ShowBarcode,
            template.HeaderText,
            template.FooterText,
            template.IsDefault,
            template.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateReceiptTemplateRequest request)
    {
        var companyId = GetCompanyId();

        if (request.IsDefault)
        {
            var existingDefaults = await _context.ReceiptTemplates
                .Where(rt => rt.CompanyId == companyId && rt.TemplateType == request.TemplateType && rt.IsDefault)
                .ToListAsync();
            foreach (var t in existingDefaults) t.IsDefault = false;
        }

        var template = new ReceiptTemplate
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            Name = request.Name,
            TemplateType = request.TemplateType,
            Language = request.Language,
            ShowLogo = request.ShowLogo,
            ShowBarcode = request.ShowBarcode,
            HeaderText = request.HeaderText,
            FooterText = request.FooterText,
            IsDefault = request.IsDefault
        };

        _context.ReceiptTemplates.Add(template);
        await _context.SaveChangesAsync();

        return Ok(new { Id = template.ReceiptTemplateId });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateReceiptTemplateRequest request)
    {
        var companyId = GetCompanyId();
        var template = await _context.ReceiptTemplates.FirstOrDefaultAsync(rt => rt.ReceiptTemplateId == id && rt.CompanyId == companyId);
        if (template == null) return NotFound();

        if (request.IsDefault && !template.IsDefault)
        {
            var existingDefaults = await _context.ReceiptTemplates
                .Where(rt => rt.CompanyId == companyId && rt.TemplateType == request.TemplateType && rt.IsDefault && rt.ReceiptTemplateId != id)
                .ToListAsync();
            foreach (var t in existingDefaults) t.IsDefault = false;
        }

        template.BranchId = request.BranchId;
        template.Name = request.Name;
        template.TemplateType = request.TemplateType;
        template.Language = request.Language;
        template.ShowLogo = request.ShowLogo;
        template.ShowBarcode = request.ShowBarcode;
        template.HeaderText = request.HeaderText;
        template.FooterText = request.FooterText;
        template.IsDefault = request.IsDefault;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var template = await _context.ReceiptTemplates.FirstOrDefaultAsync(rt => rt.ReceiptTemplateId == id && rt.CompanyId == companyId);
        if (template == null) return NotFound();

        _context.ReceiptTemplates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
