using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Restaurant.API.DTOs;
using Restaurant.API.Services;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// SuperAdmin Login
    /// </summary>
    [HttpPost("superadmin/login")]
    public async Task<ActionResult<LoginResponse>> SuperAdminLogin([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginSuperAdminAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(result);
    }

    /// <summary>
    /// Company Admin Login
    /// </summary>
    [HttpPost("company/login")]
    public async Task<ActionResult<LoginResponse>> CompanyLogin([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginCompanyAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid username or password, or company is inactive/expired" });

        return Ok(result);
    }

    /// <summary>
    /// User Login (requires company context)
    /// </summary>
    [HttpPost("user/login")]
    public async Task<ActionResult<LoginResponse>> UserLogin([FromBody] LoginRequest request, [FromHeader(Name = "X-Company-Id")] int companyId)
    {
        if (companyId <= 0)
            return BadRequest(new { message = "Company ID is required" });

        var result = await _authService.LoginUserAsync(request, companyId);
        if (result == null)
            return Unauthorized(new { message = "Invalid username or password" });

        return Ok(result);
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserInfo> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var username = User.FindFirst("username")?.Value;
        var companyId = User.FindFirst("company_id")?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var isSuperAdmin = User.FindFirst("is_superadmin")?.Value == "true";

        return Ok(new UserInfo
        {
            Id = int.Parse(userId ?? "0"),
            Name = name ?? "",
            Username = username ?? "",
            CompanyId = string.IsNullOrEmpty(companyId) ? null : int.Parse(companyId),
            Role = role,
            IsSuperAdmin = isSuperAdmin
        });
    }
}
