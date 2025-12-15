using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/approval-rules")]
[Authorize(Roles = "CompanyAdmin")]
public class ApprovalRulesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApprovalRulesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var companyId = GetCompanyId();
        var rules = await _context.ApprovalRules
            .Include(ar => ar.Role)
            .Where(ar => ar.CompanyId == companyId)
            .OrderBy(ar => ar.RuleType)
            .Select(ar => new
            {
                ar.Id,
                ar.RuleType,
                ar.RoleId,
                RoleName = ar.Role != null ? ar.Role.Name : null,
                ar.MaxDiscountPercent,
                ar.AllowPriceChange,
                ar.RequireManagerPinForPriceChange,
                ar.CanVoidPaidInvoice,
                ar.RequireManagerApprovalForVoid
            })
            .ToListAsync();

        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateApprovalRuleRequest request)
    {
        var companyId = GetCompanyId();

        var rule = new ApprovalRule
        {
            CompanyId = companyId,
            RuleType = request.RuleType,
            RoleId = request.RoleId,
            MaxDiscountPercent = request.MaxDiscountPercent,
            AllowPriceChange = request.AllowPriceChange,
            RequireManagerPinForPriceChange = request.RequireManagerPinForPriceChange,
            CanVoidPaidInvoice = request.CanVoidPaidInvoice,
            RequireManagerApprovalForVoid = request.RequireManagerApprovalForVoid
        };

        _context.ApprovalRules.Add(rule);
        await _context.SaveChangesAsync();

        return Ok(new { rule.Id });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateApprovalRuleRequest request)
    {
        var companyId = GetCompanyId();
        var rule = await _context.ApprovalRules.FirstOrDefaultAsync(ar => ar.Id == id && ar.CompanyId == companyId);
        if (rule == null) return NotFound();

        rule.RuleType = request.RuleType;
        rule.RoleId = request.RoleId;
        rule.MaxDiscountPercent = request.MaxDiscountPercent;
        rule.AllowPriceChange = request.AllowPriceChange;
        rule.RequireManagerPinForPriceChange = request.RequireManagerPinForPriceChange;
        rule.CanVoidPaidInvoice = request.CanVoidPaidInvoice;
        rule.RequireManagerApprovalForVoid = request.RequireManagerApprovalForVoid;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var rule = await _context.ApprovalRules.FirstOrDefaultAsync(ar => ar.Id == id && ar.CompanyId == companyId);
        if (rule == null) return NotFound();

        _context.ApprovalRules.Remove(rule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateApprovalRuleRequest
{
    public string RuleType { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public decimal MaxDiscountPercent { get; set; }
    public bool AllowPriceChange { get; set; }
    public bool RequireManagerPinForPriceChange { get; set; } = true;
    public bool CanVoidPaidInvoice { get; set; }
    public bool RequireManagerApprovalForVoid { get; set; } = true;
}
