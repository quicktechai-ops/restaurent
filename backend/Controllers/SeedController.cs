using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Services;

namespace Restaurant.API.Controllers;

/// <summary>
/// Seed Controller - For initial setup only, remove in production
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public SeedController(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpGet("superadmin")]
    public async Task<IActionResult> SeedSuperAdmin()
    {
        // Check if superadmin already exists
        var existing = await _context.SuperAdmins.FirstOrDefaultAsync(s => s.Username == "superadmin");
        if (existing != null)
        {
            // Update password
            existing.PasswordHash = _authService.HashPassword("Admin@123");
            await _context.SaveChangesAsync();
            return Ok(new { message = "SuperAdmin password updated", username = "superadmin", password = "Admin@123" });
        }

        var superadmin = new Models.SuperAdmin
        {
            Username = "superadmin",
            PasswordHash = _authService.HashPassword("Admin@123"),
            FullName = "Super Administrator",
            Email = "admin@restaurant.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SuperAdmins.Add(superadmin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "SuperAdmin created", username = "superadmin", password = "Admin@123" });
    }

    [HttpGet("plans")]
    public async Task<IActionResult> SeedPlans()
    {
        // Check if plans exist
        if (await _context.SubscriptionPlans.AnyAsync())
        {
            return Ok(new { message = "Plans already exist" });
        }

        var plans = new List<SubscriptionPlan>
        {
            new()
            {
                Name = "Basic",
                Description = "Perfect for small restaurants",
                Price = 29.99m,
                CurrencyCode = "USD",
                BillingCycle = "Monthly",
                DurationDays = 30,
                MaxBranches = 1,
                MaxUsers = 5,
                Features = "[\"pos_module\"]",
                IsActive = true,
                SortOrder = 1
            },
            new()
            {
                Name = "Professional",
                Description = "For growing restaurants",
                Price = 79.99m,
                CurrencyCode = "USD",
                BillingCycle = "Monthly",
                DurationDays = 30,
                MaxBranches = 3,
                MaxUsers = 15,
                Features = "[\"pos_module\", \"inventory_module\", \"loyalty_module\"]",
                IsActive = true,
                SortOrder = 2
            },
            new()
            {
                Name = "Enterprise",
                Description = "Full-featured for large operations",
                Price = 199.99m,
                CurrencyCode = "USD",
                BillingCycle = "Monthly",
                DurationDays = 30,
                MaxBranches = 10,
                MaxUsers = 50,
                Features = "[\"pos_module\", \"inventory_module\", \"loyalty_module\", \"delivery_module\", \"multi_language\", \"api_access\", \"priority_support\"]",
                IsActive = true,
                SortOrder = 3
            }
        };

        _context.SubscriptionPlans.AddRange(plans);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Plans created", count = plans.Count });
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> SeedCurrencies()
    {
        // Check if currencies exist
        if (await _context.Currencies.AnyAsync())
        {
            return Ok(new { message = "Currencies already exist" });
        }

        var currencies = new List<Currency>
        {
            new() { CurrencyCode = "USD", Name = "US Dollar", Symbol = "$", DecimalPlaces = 2, IsDefault = true, IsActive = true },
            new() { CurrencyCode = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2, IsDefault = false, IsActive = true },
            new() { CurrencyCode = "GBP", Name = "British Pound", Symbol = "£", DecimalPlaces = 2, IsDefault = false, IsActive = true },
            new() { CurrencyCode = "LBP", Name = "Lebanese Pound", Symbol = "ل.ل", DecimalPlaces = 0, IsDefault = false, IsActive = true },
            new() { CurrencyCode = "AED", Name = "UAE Dirham", Symbol = "د.إ", DecimalPlaces = 2, IsDefault = false, IsActive = true },
            new() { CurrencyCode = "SAR", Name = "Saudi Riyal", Symbol = "ر.س", DecimalPlaces = 2, IsDefault = false, IsActive = true }
        };

        _context.Currencies.AddRange(currencies);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Currencies created", count = currencies.Count });
    }

    [HttpGet("all")]
    public async Task<IActionResult> SeedAll()
    {
        await SeedCurrencies();
        await SeedPlans();
        await SeedSuperAdmin();

        return Ok(new { message = "All seed data created successfully" });
    }

    [HttpGet("reset-company-password/{username}")]
    public async Task<IActionResult> ResetCompanyPassword(string username, [FromQuery] string newPassword = "123456")
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Username == username);
        if (company == null)
            return NotFound(new { message = "Company not found" });

        company.PasswordHash = _authService.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset", username = username, password = newPassword });
    }
}
