using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,Manager,Cashier")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<CustomerListDto>>> GetAll([FromQuery] string? search, [FromQuery] bool? isActive)
    {
        var companyId = GetCompanyId();
        var query = _context.Customers
            .Include(c => c.DefaultBranch)
            .Include(c => c.Addresses)
            .Where(c => c.CompanyId == companyId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search) || 
                                     (c.Phone != null && c.Phone.Contains(search)) ||
                                     (c.Email != null && c.Email.Contains(search)));
        }

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var customers = await query
            .OrderBy(c => c.Name)
            .Select(c => new CustomerListDto
            {
                Id = c.CustomerId,
                CustomerCode = c.CustomerCode,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                DefaultBranchId = c.DefaultBranchId,
                DefaultBranchName = c.DefaultBranch != null ? c.DefaultBranch.Name : null,
                IsActive = c.IsActive,
                AddressesCount = c.Addresses.Count,
                OrdersCount = 0 // TODO: Add orders count when orders are implemented
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var customer = await _context.Customers
            .Include(c => c.DefaultBranch)
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.CustomerId == id && c.CompanyId == companyId);

        if (customer == null) return NotFound();

        return Ok(new CustomerListDto
        {
            Id = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            DefaultBranchId = customer.DefaultBranchId,
            DefaultBranchName = customer.DefaultBranch?.Name,
            IsActive = customer.IsActive,
            AddressesCount = customer.Addresses.Count
        });
    }

    [HttpPost]
    public async Task<ActionResult<CustomerListDto>> Create([FromBody] CreateCustomerRequest request)
    {
        var companyId = GetCompanyId();

        var customer = new Customer
        {
            CompanyId = companyId,
            CustomerCode = request.CustomerCode,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            DefaultBranchId = request.DefaultBranchId,
            DefaultCurrencyCode = request.DefaultCurrencyCode,
            Notes = request.Notes
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return Ok(new CustomerListDto
        {
            Id = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            IsActive = customer.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerListDto>> Update(int id, [FromBody] UpdateCustomerRequest request)
    {
        var companyId = GetCompanyId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id && c.CompanyId == companyId);
        if (customer == null) return NotFound();

        customer.CustomerCode = request.CustomerCode;
        customer.Name = request.Name;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.DefaultBranchId = request.DefaultBranchId;
        customer.DefaultCurrencyCode = request.DefaultCurrencyCode;
        customer.Notes = request.Notes;
        customer.IsActive = request.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new CustomerListDto
        {
            Id = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            IsActive = customer.IsActive
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id && c.CompanyId == companyId);
        if (customer == null) return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Customer Addresses
    [HttpGet("{customerId}/addresses")]
    public async Task<ActionResult<List<CustomerAddressDto>>> GetAddresses(int customerId)
    {
        var companyId = GetCompanyId();
        var addresses = await _context.CustomerAddresses
            .Include(a => a.DeliveryZone)
            .Where(a => a.CustomerId == customerId && a.Customer!.CompanyId == companyId)
            .Select(a => new CustomerAddressDto
            {
                Id = a.CustomerAddressId,
                Label = a.Label,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                Area = a.Area,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                DeliveryZoneId = a.DeliveryZoneId,
                DeliveryZoneName = a.DeliveryZone != null ? a.DeliveryZone.ZoneName : null,
                IsDefault = a.IsDefault
            })
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpPost("{customerId}/addresses")]
    public async Task<ActionResult<CustomerAddressDto>> AddAddress(int customerId, [FromBody] CreateCustomerAddressRequest request)
    {
        var companyId = GetCompanyId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CompanyId == companyId);
        if (customer == null) return NotFound();

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            Area = request.Area,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DeliveryZoneId = request.DeliveryZoneId,
            IsDefault = request.IsDefault
        };

        if (request.IsDefault)
        {
            var existingAddresses = await _context.CustomerAddresses.Where(a => a.CustomerId == customerId).ToListAsync();
            foreach (var a in existingAddresses) a.IsDefault = false;
        }

        _context.CustomerAddresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(new CustomerAddressDto
        {
            Id = address.CustomerAddressId,
            Label = address.Label,
            AddressLine1 = address.AddressLine1,
            City = address.City,
            IsDefault = address.IsDefault
        });
    }

    [HttpDelete("addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        var companyId = GetCompanyId();
        var address = await _context.CustomerAddresses
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.CustomerAddressId == addressId && a.Customer!.CompanyId == companyId);
        if (address == null) return NotFound();

        _context.CustomerAddresses.Remove(address);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Customer Stats
    [HttpGet("{customerId}/stats")]
    public async Task<ActionResult> GetCustomerStats(int customerId)
    {
        var companyId = GetCompanyId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CompanyId == companyId);
        if (customer == null) return NotFound();

        var orders = await _context.Orders
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.MenuItem)
            .Where(o => o.CustomerId == customerId && o.CompanyId == companyId && o.PaymentStatus == "Paid")
            .ToListAsync();

        var totalOrders = orders.Count;
        var totalSpent = orders.Sum(o => o.GrandTotal);
        var averageTicket = totalOrders > 0 ? totalSpent / totalOrders : 0;
        var totalVisits = orders.Select(o => o.CreatedAt.Date).Distinct().Count();

        // Get favorite items
        var favoriteItems = orders
            .SelectMany(o => o.OrderLines)
            .GroupBy(ol => new { ol.MenuItemId, ol.MenuItem?.Name })
            .Select(g => new { Name = g.Key.Name ?? "Unknown", Quantity = g.Sum(ol => ol.Quantity) })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToList();

        return Ok(new
        {
            TotalOrders = totalOrders,
            TotalSpent = totalSpent,
            AverageTicket = averageTicket,
            TotalVisits = totalVisits,
            FavoriteItems = favoriteItems
        });
    }

    // Customer Orders
    [HttpGet("{customerId}/orders")]
    public async Task<ActionResult> GetCustomerOrders(int customerId)
    {
        var companyId = GetCompanyId();
        var orders = await _context.Orders
            .Include(o => o.OrderLines)
            .Where(o => o.CustomerId == customerId && o.CompanyId == companyId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(50)
            .Select(o => new
            {
                Id = o.OrderId,
                o.OrderNumber,
                o.OrderType,
                ItemCount = o.OrderLines.Count,
                o.GrandTotal,
                Status = o.PaymentStatus == "Paid" ? "Paid" : o.OrderStatus,
                o.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    // Customer Loyalty
    [HttpGet("{customerId}/loyalty")]
    public async Task<ActionResult> GetCustomerLoyalty(int customerId)
    {
        try
        {
            var companyId = GetCompanyId();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CompanyId == companyId);
            if (customer == null) return NotFound();

            var loyaltyAccount = await _context.LoyaltyAccounts
                .Include(la => la.LoyaltyTier)
                .FirstOrDefaultAsync(la => la.CustomerId == customerId);

            if (loyaltyAccount == null)
                return Ok(null);

            return Ok(new
            {
                CardNumber = $"L{customerId:D6}",
                TierName = loyaltyAccount.LoyaltyTier?.Name,
                PointsBalance = loyaltyAccount.PointsBalance,
                MemberSince = loyaltyAccount.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    // Enroll customer in loyalty program
    [HttpPost("{customerId}/loyalty/enroll")]
    public async Task<ActionResult> EnrollInLoyalty(int customerId)
    {
        try
        {
            var companyId = GetCompanyId();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.CompanyId == companyId);
            if (customer == null) return NotFound(new { message = "Customer not found" });

            // Check if already enrolled
            var existingAccount = await _context.LoyaltyAccounts.FirstOrDefaultAsync(la => la.CustomerId == customerId);
            if (existingAccount != null)
                return BadRequest(new { message = "Customer is already enrolled in loyalty program" });

            // Get the first (default) tier if exists
            var defaultTier = await _context.LoyaltyTiers
                .Where(t => t.CompanyId == companyId)
                .OrderBy(t => t.MinTotalSpent)
                .FirstOrDefaultAsync();

            var loyaltyAccount = new LoyaltyAccount
            {
                CustomerId = customerId,
                LoyaltyTierId = defaultTier?.LoyaltyTierId,
                PointsBalance = 0
            };

            _context.LoyaltyAccounts.Add(loyaltyAccount);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Customer enrolled in loyalty program",
                CardNumber = $"L{customerId:D6}",
                TierName = defaultTier?.Name,
                PointsBalance = 0
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }
}
