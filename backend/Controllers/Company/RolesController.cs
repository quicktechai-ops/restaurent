using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin")]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("company_id")?.Value;
        return int.Parse(companyIdClaim ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleListDto>>> GetAll()
    {
        var companyId = GetCompanyId();

        var roles = await _context.Roles
            .Include(r => r.Branch)
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.CompanyId == companyId)
            .Select(r => new RoleListDto
            {
                Id = r.RoleId,
                Name = r.Name,
                Description = r.Description,
                BranchId = r.BranchId,
                BranchName = r.Branch != null ? r.Branch.Name : null,
                IsActive = r.IsActive,
                UsersCount = _context.UserRoles.Count(ur => ur.RoleId == r.RoleId),
                Permissions = r.RolePermissions.Select(rp => rp.Permission.Code).ToList()
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("permissions")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissions()
    {
        var permissions = await _context.Permissions
            .Select(p => new PermissionDto
            {
                Id = p.PermissionId,
                Code = p.Code,
                Description = p.Description
            })
            .ToListAsync();

        return Ok(permissions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();

        var role = await _context.Roles
            .Include(r => r.Branch)
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.RoleId == id && r.CompanyId == companyId)
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(new { message = "Role not found" });

        return Ok(new RoleListDto
        {
            Id = role.RoleId,
            Name = role.Name,
            Description = role.Description,
            BranchId = role.BranchId,
            BranchName = role.Branch?.Name,
            IsActive = role.IsActive,
            Permissions = role.RolePermissions.Select(rp => rp.Permission.Code).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<RoleListDto>> Create([FromBody] CreateRoleRequest request)
    {
        var companyId = GetCompanyId();

        var role = new Role
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Assign permissions
        foreach (var permissionId in request.PermissionIds)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = role.RoleId,
                PermissionId = permissionId
            });
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, new RoleListDto
        {
            Id = role.RoleId,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateRoleRequest request)
    {
        var companyId = GetCompanyId();

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == id && r.CompanyId == companyId);

        if (role == null)
            return NotFound(new { message = "Role not found" });

        role.Name = request.Name;
        role.Description = request.Description;
        role.BranchId = request.BranchId;
        role.IsActive = request.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        // Update permissions
        var existingPermissions = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
        _context.RolePermissions.RemoveRange(existingPermissions);

        foreach (var permissionId in request.PermissionIds)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = id,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Role updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == id && r.CompanyId == companyId);

        if (role == null)
            return NotFound(new { message = "Role not found" });

        // Check if role is in use
        var usersWithRole = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id);
        if (usersWithRole)
            return BadRequest(new { message = "Cannot delete role that is assigned to users" });

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Role deleted successfully" });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == id && r.CompanyId == companyId);

        if (role == null)
            return NotFound(new { message = "Role not found" });

        role.IsActive = !role.IsActive;
        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Role is now {(role.IsActive ? "active" : "inactive")}" });
    }
}
