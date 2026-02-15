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
        try
        {
            var templates = await _context.ReceiptTemplates
                .Where(rt => rt.CompanyId == companyId)
                .OrderBy(rt => rt.TemplateType)
                .ThenBy(rt => rt.Name)
                .ToListAsync();

            var result = templates.Select(rt => new
            {
                Id = rt.ReceiptTemplateId,
                rt.Name,
                Type = rt.TemplateType,
                PaperSize = rt.PaperSize ?? "80mm",
                rt.Language,
                rt.ShowLogo,
                ShowAddress = rt.ShowAddress,
                ShowPhone = rt.ShowPhone,
                ShowTaxNumber = rt.ShowTaxNumber,
                ShowOrderNumber = rt.ShowOrderNumber,
                ShowDate = rt.ShowDate,
                ShowOrderType = rt.ShowOrderType,
                ShowTable = rt.ShowTable,
                ShowCustomer = rt.ShowCustomer,
                ShowPaymentMethod = rt.ShowPaymentMethod,
                ShowItemCode = rt.ShowItemCode,
                ShowModifiers = rt.ShowModifiers,
                ShowDiscountDetails = rt.ShowDiscountDetails,
                ShowPaymentDetails = rt.ShowPaymentDetails,
                ShowTips = rt.ShowTips,
                rt.ShowBarcode,
                rt.HeaderText,
                rt.FooterText,
                FooterText2 = rt.FooterText2 ?? "",
                FooterTextAr = rt.FooterTextAr ?? "",
                FooterTextAr2 = rt.FooterTextAr2 ?? "",
                rt.IsDefault,
                rt.IsActive
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            // If new columns don't exist, return basic data
            var basicTemplates = await _context.Database
                .SqlQueryRaw<BasicReceiptTemplate>(
                    "SELECT receipt_template_id as Id, name as Name, template_type as Type, language as Language, " +
                    "show_logo as ShowLogo, show_barcode as ShowBarcode, header_text as HeaderText, footer_text as FooterText, " +
                    "is_default as IsDefault, is_active as IsActive FROM receipt_templates WHERE company_id = {0}", companyId)
                .ToListAsync();
            
            return Ok(basicTemplates.Select(rt => new
            {
                rt.Id,
                rt.Name,
                rt.Type,
                PaperSize = "80mm",
                rt.Language,
                rt.ShowLogo,
                ShowAddress = true,
                ShowPhone = true,
                ShowTaxNumber = true,
                ShowOrderNumber = true,
                ShowDate = true,
                ShowOrderType = true,
                ShowTable = true,
                ShowCustomer = true,
                ShowPaymentMethod = true,
                ShowItemCode = false,
                ShowModifiers = true,
                ShowDiscountDetails = true,
                ShowPaymentDetails = true,
                ShowTips = true,
                rt.ShowBarcode,
                rt.HeaderText,
                rt.FooterText,
                FooterText2 = "",
                FooterTextAr = "",
                FooterTextAr2 = "",
                rt.IsDefault,
                rt.IsActive
            }));
        }
    }

    private class BasicReceiptTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Language { get; set; } = "en";
        public bool ShowLogo { get; set; }
        public bool ShowBarcode { get; set; }
        public string? HeaderText { get; set; }
        public string? FooterText { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
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
            PaperSize = template.PaperSize ?? "80mm",
            template.Language,
            template.ShowLogo,
            template.ShowAddress,
            template.ShowPhone,
            template.ShowTaxNumber,
            template.ShowOrderNumber,
            template.ShowDate,
            template.ShowOrderType,
            template.ShowTable,
            template.ShowCustomer,
            template.ShowPaymentMethod,
            template.ShowItemCode,
            template.ShowModifiers,
            template.ShowDiscountDetails,
            template.ShowPaymentDetails,
            template.ShowTips,
            template.ShowBarcode,
            template.HeaderText,
            template.FooterText,
            template.FooterText2,
            template.FooterTextAr,
            template.FooterTextAr2,
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
            PaperSize = request.PaperSize ?? "80mm",
            Language = request.Language,
            ShowLogo = request.ShowLogo,
            ShowAddress = request.ShowAddress,
            ShowPhone = request.ShowPhone,
            ShowTaxNumber = request.ShowTaxNumber,
            ShowOrderNumber = request.ShowOrderNumber,
            ShowDate = request.ShowDate,
            ShowOrderType = request.ShowOrderType,
            ShowTable = request.ShowTable,
            ShowCustomer = request.ShowCustomer,
            ShowPaymentMethod = request.ShowPaymentMethod,
            ShowItemCode = request.ShowItemCode,
            ShowModifiers = request.ShowModifiers,
            ShowDiscountDetails = request.ShowDiscountDetails,
            ShowPaymentDetails = request.ShowPaymentDetails,
            ShowTips = request.ShowTips,
            ShowBarcode = request.ShowBarcode,
            HeaderText = request.HeaderText,
            FooterText = request.FooterText,
            FooterText2 = request.FooterText2,
            FooterTextAr = request.FooterTextAr,
            FooterTextAr2 = request.FooterTextAr2,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive
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
        template.PaperSize = request.PaperSize ?? "80mm";
        template.Language = request.Language;
        template.ShowLogo = request.ShowLogo;
        template.ShowAddress = request.ShowAddress;
        template.ShowPhone = request.ShowPhone;
        template.ShowTaxNumber = request.ShowTaxNumber;
        template.ShowOrderNumber = request.ShowOrderNumber;
        template.ShowDate = request.ShowDate;
        template.ShowOrderType = request.ShowOrderType;
        template.ShowTable = request.ShowTable;
        template.ShowCustomer = request.ShowCustomer;
        template.ShowPaymentMethod = request.ShowPaymentMethod;
        template.ShowItemCode = request.ShowItemCode;
        template.ShowModifiers = request.ShowModifiers;
        template.ShowDiscountDetails = request.ShowDiscountDetails;
        template.ShowPaymentDetails = request.ShowPaymentDetails;
        template.ShowTips = request.ShowTips;
        template.ShowBarcode = request.ShowBarcode;
        template.HeaderText = request.HeaderText;
        template.FooterText = request.FooterText;
        template.FooterText2 = request.FooterText2;
        template.FooterTextAr = request.FooterTextAr;
        template.FooterTextAr2 = request.FooterTextAr2;
        template.IsDefault = request.IsDefault;
        template.IsActive = request.IsActive;
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
