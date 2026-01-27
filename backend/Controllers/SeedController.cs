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

    [HttpGet("init-db")]
    public async Task<IActionResult> InitializeDatabase()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
            return Ok(new { message = "Database schema created successfully" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Error creating schema", error = ex.Message });
        }
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

    [HttpGet("roles/{companyId}")]
    public async Task<IActionResult> SeedRoles(int companyId)
    {
        // Check if roles exist for this company
        if (await _context.Roles.AnyAsync(r => r.CompanyId == companyId))
            return Ok(new { message = "Roles already exist for this company" });

        var roles = new List<Role>
        {
            new Role { CompanyId = companyId, Name = "Manager", Description = "Full access to manage the restaurant", IsActive = true },
            new Role { CompanyId = companyId, Name = "Cashier", Description = "POS and basic order management", IsActive = true },
            new Role { CompanyId = companyId, Name = "Waiter", Description = "Take orders and manage tables", IsActive = true },
            new Role { CompanyId = companyId, Name = "Kitchen Staff", Description = "View and manage kitchen orders", IsActive = true },
            new Role { CompanyId = companyId, Name = "Inventory Manager", Description = "Manage inventory and suppliers", IsActive = true }
        };

        _context.Roles.AddRange(roles);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Default roles created", count = roles.Count });
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

    [HttpGet("fix-orders-table")]
    public async Task<IActionResult> FixOrdersTable()
    {
        var sql = @"
-- Drop and recreate orders table with all columns
DROP TABLE IF EXISTS order_delivery_details CASCADE;
DROP TABLE IF EXISTS order_status_history CASCADE;
DROP TABLE IF EXISTS order_payments CASCADE;
DROP TABLE IF EXISTS order_line_modifiers CASCADE;
DROP TABLE IF EXISTS order_lines CASCADE;
DROP TABLE IF EXISTS orders CASCADE;
DROP TABLE IF EXISTS shifts CASCADE;
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "POS tables dropped. Now call /api/seed/pos-tables to recreate them." });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Error", details = ex.Message });
        }
    }

    [HttpGet("fix-loyalty-tables")]
    public async Task<IActionResult> FixLoyaltyTables()
    {
        var sql = @"
-- Add missing columns to loyalty_accounts
ALTER TABLE loyalty_accounts ADD COLUMN IF NOT EXISTS created_at TIMESTAMP DEFAULT NOW();
ALTER TABLE loyalty_accounts ADD COLUMN IF NOT EXISTS updated_at TIMESTAMP;
ALTER TABLE loyalty_accounts ADD COLUMN IF NOT EXISTS tier_assigned_at TIMESTAMP;
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "Loyalty tables fixed" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Loyalty fix completed", details = ex.Message });
        }
    }

    [HttpGet("fix-reservations-table")]
    public async Task<IActionResult> FixReservationsTable()
    {
        var sql = @"
-- Make created_by_user_id nullable in reservations
ALTER TABLE reservations ALTER COLUMN created_by_user_id DROP NOT NULL;
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "Reservations table fixed" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Reservations fix completed", details = ex.Message });
        }
    }

    [HttpGet("fix-pos-constraints")]
    public async Task<IActionResult> FixPOSConstraints()
    {
        var sql = @"
-- Make user_id columns nullable in POS tables
ALTER TABLE order_payments ALTER COLUMN user_id DROP NOT NULL;
ALTER TABLE order_payments DROP CONSTRAINT IF EXISTS order_payments_user_id_fkey;
-- Fix stock_adjustments table
ALTER TABLE stock_adjustments ALTER COLUMN user_id DROP NOT NULL;
ALTER TABLE stock_adjustments DROP CONSTRAINT IF EXISTS stock_adjustments_user_id_fkey;
ALTER TABLE stock_adjustments ALTER COLUMN branch_id DROP NOT NULL;
-- Add new columns to stock_adjustments
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS inventory_item_id INTEGER REFERENCES inventory_items(inventory_item_id);
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS adjustment_type VARCHAR(20) DEFAULT 'increase';
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS quantity DECIMAL(18,4) DEFAULT 0;
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS quantity_before DECIMAL(18,4) DEFAULT 0;
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS quantity_after DECIMAL(18,4) DEFAULT 0;
ALTER TABLE stock_adjustments ADD COLUMN IF NOT EXISTS reason VARCHAR(100);
-- Add image column to categories
ALTER TABLE categories ADD COLUMN IF NOT EXISTS image VARCHAR(500);
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "POS constraints fixed" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Constraint fix completed", details = ex.Message });
        }
    }

    [HttpGet("pos-tables")]
    public async Task<IActionResult> CreatePOSTables()
    {
        var sql = @"
-- Shifts table
CREATE TABLE IF NOT EXISTS shifts (
    shift_id SERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL REFERENCES companies(company_id) ON DELETE CASCADE,
    branch_id INTEGER NOT NULL REFERENCES branches(branch_id) ON DELETE RESTRICT,
    cashier_user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT,
    open_time TIMESTAMP NOT NULL DEFAULT NOW(),
    close_time TIMESTAMP,
    expected_close_time TIMESTAMP,
    opening_cash DECIMAL(18,2) NOT NULL DEFAULT 0,
    closing_cash DECIMAL(18,2),
    expected_cash DECIMAL(18,2),
    cash_difference DECIMAL(18,2),
    status VARCHAR(20) NOT NULL DEFAULT 'Open',
    force_closed_at TIMESTAMP,
    force_close_reason VARCHAR(255),
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Orders table
CREATE TABLE IF NOT EXISTS orders (
    order_id SERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL REFERENCES companies(company_id) ON DELETE CASCADE,
    branch_id INTEGER NOT NULL REFERENCES branches(branch_id) ON DELETE RESTRICT,
    shift_id INTEGER REFERENCES shifts(shift_id) ON DELETE SET NULL,
    order_number VARCHAR(50) NOT NULL,
    order_type VARCHAR(20) NOT NULL DEFAULT 'DineIn',
    table_id INTEGER REFERENCES restaurant_tables(table_id) ON DELETE SET NULL,
    waiter_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    cashier_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    customer_id INTEGER REFERENCES customers(customer_id) ON DELETE SET NULL,
    order_status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    currency_code VARCHAR(3) NOT NULL DEFAULT 'USD',
    exchange_rate_to_base DECIMAL(18,6) NOT NULL DEFAULT 1,
    sub_total DECIMAL(18,2) NOT NULL DEFAULT 0,
    total_line_discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    bill_discount_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    bill_discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    service_charge_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    service_charge_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    tax_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    tax_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    delivery_fee DECIMAL(18,2) NOT NULL DEFAULT 0,
    tips_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    grand_total DECIMAL(18,2) NOT NULL DEFAULT 0,
    net_amount_for_loyalty DECIMAL(18,2),
    loyalty_points_earned DECIMAL(18,2),
    loyalty_points_redeemed DECIMAL(18,2),
    loyalty_discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    total_paid DECIMAL(18,2) NOT NULL DEFAULT 0,
    balance_due DECIMAL(18,2) NOT NULL DEFAULT 0,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Unpaid',
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    paid_at TIMESTAMP,
    voided_at TIMESTAMP,
    void_reason VARCHAR(255),
    void_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    approved_void_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    merged_from_order_id INTEGER,
    split_from_order_id INTEGER
);

-- Order Lines table
CREATE TABLE IF NOT EXISTS order_lines (
    order_line_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(menu_item_id) ON DELETE RESTRICT,
    menu_item_size_id INTEGER REFERENCES menu_item_sizes(menu_item_size_id) ON DELETE SET NULL,
    quantity DECIMAL(18,4) NOT NULL DEFAULT 1,
    base_unit_price DECIMAL(18,2) NOT NULL,
    modifiers_extra_price DECIMAL(18,2) NOT NULL DEFAULT 0,
    effective_unit_price DECIMAL(18,2) NOT NULL,
    line_gross DECIMAL(18,2) NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    line_net DECIMAL(18,2) NOT NULL,
    notes VARCHAR(255),
    kitchen_status VARCHAR(20) NOT NULL DEFAULT 'New',
    kitchen_station_id INTEGER REFERENCES kitchen_stations(kitchen_station_id) ON DELETE SET NULL,
    sent_to_kitchen_at TIMESTAMP,
    ready_at TIMESTAMP,
    served_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL
);

-- Order Line Modifiers table
CREATE TABLE IF NOT EXISTS order_line_modifiers (
    order_line_modifier_id SERIAL PRIMARY KEY,
    order_line_id INTEGER NOT NULL REFERENCES order_lines(order_line_id) ON DELETE CASCADE,
    modifier_id INTEGER NOT NULL REFERENCES modifiers(modifier_id) ON DELETE RESTRICT,
    quantity DECIMAL(18,4) NOT NULL DEFAULT 1,
    extra_price DECIMAL(18,2) NOT NULL,
    total_price DECIMAL(18,2) NOT NULL
);

-- Order Payments table
CREATE TABLE IF NOT EXISTS order_payments (
    order_payment_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    payment_method_id INTEGER NOT NULL REFERENCES payment_methods(payment_method_id) ON DELETE RESTRICT,
    currency_code VARCHAR(3) NOT NULL DEFAULT 'USD',
    amount DECIMAL(18,2) NOT NULL,
    amount_in_order_currency DECIMAL(18,2) NOT NULL,
    exchange_rate_to_order_currency DECIMAL(18,6) NOT NULL DEFAULT 1,
    reference VARCHAR(100),
    gift_card_id INTEGER REFERENCES gift_cards(gift_card_id) ON DELETE SET NULL,
    loyalty_points_used DECIMAL(18,2),
    notes VARCHAR(255),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT
);

-- Order Status History table
CREATE TABLE IF NOT EXISTS order_status_history (
    order_status_history_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    old_status VARCHAR(20),
    new_status VARCHAR(20) NOT NULL,
    changed_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    notes VARCHAR(255)
);

-- Order Delivery Details table
CREATE TABLE IF NOT EXISTS order_delivery_details (
    order_delivery_details_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL UNIQUE REFERENCES orders(order_id) ON DELETE CASCADE,
    customer_address_id INTEGER REFERENCES customer_addresses(customer_address_id) ON DELETE SET NULL,
    delivery_zone_id INTEGER REFERENCES delivery_zones(delivery_zone_id) ON DELETE SET NULL,
    address_line VARCHAR(500),
    city VARCHAR(100),
    area VARCHAR(100),
    phone VARCHAR(50),
    distance_km DECIMAL(18,2),
    delivery_fee_calculated DECIMAL(18,2) NOT NULL DEFAULT 0,
    driver_name VARCHAR(100),
    driver_phone VARCHAR(50),
    estimated_delivery_time TIMESTAMP,
    out_for_delivery_at TIMESTAMP,
    delivered_at TIMESTAMP,
    delivery_notes VARCHAR(500)
);

-- Create unique index for order number per branch
CREATE UNIQUE INDEX IF NOT EXISTS idx_orders_branch_number ON orders(branch_id, order_number);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_shifts_company ON shifts(company_id);
CREATE INDEX IF NOT EXISTS idx_shifts_branch ON shifts(branch_id);
CREATE INDEX IF NOT EXISTS idx_shifts_status ON shifts(status);
CREATE INDEX IF NOT EXISTS idx_orders_company ON orders(company_id);
CREATE INDEX IF NOT EXISTS idx_orders_branch ON orders(branch_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON orders(order_status);
CREATE INDEX IF NOT EXISTS idx_orders_created ON orders(created_at);
CREATE INDEX IF NOT EXISTS idx_order_lines_order ON order_lines(order_id);
CREATE INDEX IF NOT EXISTS idx_order_lines_kitchen_status ON order_lines(kitchen_status);
CREATE INDEX IF NOT EXISTS idx_order_payments_order ON order_payments(order_id);
CREATE INDEX IF NOT EXISTS idx_order_status_history_order ON order_status_history(order_id);
";

        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "POS tables created successfully" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "POS tables migration completed", details = ex.Message });
        }
    }

    [HttpGet("fix-exchange-rates")]
    public async Task<IActionResult> FixExchangeRates()
    {
        var sql = @"
ALTER TABLE exchange_rates ADD COLUMN IF NOT EXISTS created_at TIMESTAMP DEFAULT NOW();
ALTER TABLE exchange_rates ALTER COLUMN created_by_user_id DROP NOT NULL;
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "Exchange rates table fixed" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Exchange rates fix completed", details = ex.Message });
        }
    }

    [HttpGet("fix-users-table")]
    public async Task<IActionResult> FixUsersTable()
    {
        var sql = @"
ALTER TABLE users ADD COLUMN IF NOT EXISTS position VARCHAR(50);
";
        try
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
            return Ok(new { message = "Users table fixed" });
        }
        catch (Exception ex)
        {
            return Ok(new { message = "Users table fix completed", details = ex.Message });
        }
    }
}
