using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/commission-policies")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class CommissionPoliciesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommissionPoliciesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var companyId = GetCompanyId();
        var policies = await _context.CommissionPolicies
            .Include(cp => cp.Branch)
            .Where(cp => cp.CompanyId == companyId)
            .OrderBy(cp => cp.CommissionPolicyId)
            .Select(cp => new
            {
                Id = cp.CommissionPolicyId,
                cp.BranchId,
                BranchName = cp.Branch != null ? cp.Branch.Name : "All Branches",
                cp.SalesPercent,
                cp.FixedPerInvoice,
                cp.ApplyOnNetBeforeTax,
                cp.ExcludeDiscountedInvoices,
                cp.IsActive
            })
            .ToListAsync();

        return Ok(policies);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCommissionPolicyRequest request)
    {
        var companyId = GetCompanyId();

        var policy = new CommissionPolicy
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            SalesPercent = request.SalesPercent,
            FixedPerInvoice = request.FixedPerInvoice,
            ApplyOnNetBeforeTax = request.ApplyOnNetBeforeTax,
            ExcludeDiscountedInvoices = request.ExcludeDiscountedInvoices,
            IsActive = request.IsActive
        };

        _context.CommissionPolicies.Add(policy);
        await _context.SaveChangesAsync();

        return Ok(new { Id = policy.CommissionPolicyId });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateCommissionPolicyRequest request)
    {
        var companyId = GetCompanyId();
        var policy = await _context.CommissionPolicies.FirstOrDefaultAsync(cp => cp.CommissionPolicyId == id && cp.CompanyId == companyId);
        if (policy == null) return NotFound();

        policy.BranchId = request.BranchId;
        policy.SalesPercent = request.SalesPercent;
        policy.FixedPerInvoice = request.FixedPerInvoice;
        policy.ApplyOnNetBeforeTax = request.ApplyOnNetBeforeTax;
        policy.ExcludeDiscountedInvoices = request.ExcludeDiscountedInvoices;
        policy.IsActive = request.IsActive;
        policy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var policy = await _context.CommissionPolicies.FirstOrDefaultAsync(cp => cp.CommissionPolicyId == id && cp.CompanyId == companyId);
        if (policy == null) return NotFound();

        _context.CommissionPolicies.Remove(policy);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateCommissionPolicyRequest
{
    public int? BranchId { get; set; }
    public decimal SalesPercent { get; set; }
    public decimal FixedPerInvoice { get; set; }
    public bool ApplyOnNetBeforeTax { get; set; } = true;
    public bool ExcludeDiscountedInvoices { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
