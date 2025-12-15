using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.SuperAdmin;

[ApiController]
[Route("api/superadmin/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlanListDto>>> GetAll([FromQuery] bool? isActive)
    {
        var query = _context.SubscriptionPlans.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var plans = await query
            .OrderBy(p => p.SortOrder)
            .Select(p => new PlanListDto
            {
                Id = p.PlanId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CurrencyCode = p.CurrencyCode,
                BillingCycle = p.BillingCycle,
                DurationDays = p.DurationDays,
                MaxBranches = p.MaxBranches,
                MaxUsers = p.MaxUsers,
                MaxOrdersPerMonth = p.MaxOrdersPerMonth,
                Features = p.Features,
                IsActive = p.IsActive,
                SortOrder = p.SortOrder
            })
            .ToListAsync();

        return Ok(plans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlanListDto>> GetById(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null)
            return NotFound(new { message = "Plan not found" });

        return Ok(new PlanListDto
        {
            Id = plan.PlanId,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            CurrencyCode = plan.CurrencyCode,
            BillingCycle = plan.BillingCycle,
            DurationDays = plan.DurationDays,
            MaxBranches = plan.MaxBranches,
            MaxUsers = plan.MaxUsers,
            MaxOrdersPerMonth = plan.MaxOrdersPerMonth,
            Features = plan.Features,
            IsActive = plan.IsActive,
            SortOrder = plan.SortOrder
        });
    }

    [HttpPost]
    public async Task<ActionResult<PlanListDto>> Create([FromBody] CreatePlanRequest request)
    {
        // Ensure Features is valid JSON or null
        var features = string.IsNullOrWhiteSpace(request.Features) ? null : request.Features;
        
        var plan = new SubscriptionPlan
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CurrencyCode = request.CurrencyCode ?? "USD",
            BillingCycle = request.BillingCycle ?? "Monthly",
            DurationDays = request.DurationDays > 0 ? request.DurationDays : 30,
            MaxBranches = request.MaxBranches > 0 ? request.MaxBranches : 1,
            MaxUsers = request.MaxUsers > 0 ? request.MaxUsers : 5,
            MaxOrdersPerMonth = request.MaxOrdersPerMonth,
            Features = features,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plan.PlanId }, new PlanListDto
        {
            Id = plan.PlanId,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            CurrencyCode = plan.CurrencyCode,
            BillingCycle = plan.BillingCycle,
            DurationDays = plan.DurationDays,
            MaxBranches = plan.MaxBranches,
            MaxUsers = plan.MaxUsers,
            MaxOrdersPerMonth = plan.MaxOrdersPerMonth,
            Features = plan.Features,
            IsActive = plan.IsActive,
            SortOrder = plan.SortOrder
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePlanRequest request)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null)
            return NotFound(new { message = "Plan not found" });

        // Ensure Features is valid JSON or null
        var features = string.IsNullOrWhiteSpace(request.Features) ? null : request.Features;

        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.Price = request.Price;
        plan.CurrencyCode = request.CurrencyCode ?? "USD";
        plan.BillingCycle = request.BillingCycle ?? "Monthly";
        plan.DurationDays = request.DurationDays > 0 ? request.DurationDays : 30;
        plan.MaxBranches = request.MaxBranches > 0 ? request.MaxBranches : 1;
        plan.MaxUsers = request.MaxUsers > 0 ? request.MaxUsers : 5;
        plan.MaxOrdersPerMonth = request.MaxOrdersPerMonth;
        plan.Features = features;
        plan.IsActive = request.IsActive;
        plan.SortOrder = request.SortOrder;
        plan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Plan updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null)
            return NotFound(new { message = "Plan not found" });

        // Check if any companies are using this plan
        var companiesUsingPlan = await _context.Companies.AnyAsync(c => c.PlanId == id);
        if (companiesUsingPlan)
            return BadRequest(new { message = "Cannot delete plan that is being used by companies" });

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Plan deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult> Toggle(int id)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(id);
        if (plan == null)
            return NotFound(new { message = "Plan not found" });

        plan.IsActive = !plan.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Plan is now {(plan.IsActive ? "active" : "inactive")}" });
    }
}
