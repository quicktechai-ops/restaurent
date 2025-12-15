using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/audit-logs")]
[Authorize(Roles = "CompanyAdmin")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<AuditLogDto>>> GetAll(
        [FromQuery] int? branchId,
        [FromQuery] int? userId,
        [FromQuery] string? actionType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var companyId = GetCompanyId();
        var query = _context.AuditLogs
            .Include(a => a.User)
            .Include(a => a.Branch)
            .Where(a => a.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(a => a.BranchId == branchId.Value);

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        if (!string.IsNullOrEmpty(actionType))
            query = query.Where(a => a.ActionType == actionType);

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        var total = await query.CountAsync();

        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.AuditLogId,
                Timestamp = a.Timestamp,
                UserId = a.UserId,
                Username = a.User != null ? a.User.Username : null,
                BranchId = a.BranchId,
                BranchName = a.Branch != null ? a.Branch.Name : null,
                ActionType = a.ActionType,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Details = a.Details
            })
            .ToListAsync();

        return Ok(new { data = logs, total, page, pageSize });
    }

    [HttpGet("action-types")]
    public async Task<ActionResult<List<string>>> GetActionTypes()
    {
        var companyId = GetCompanyId();
        var types = await _context.AuditLogs
            .Where(a => a.CompanyId == companyId)
            .Select(a => a.ActionType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

        return Ok(types);
    }
}

public class AuditLogDto
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}
