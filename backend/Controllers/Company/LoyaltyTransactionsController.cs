using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/loyalty/transactions")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class LoyaltyTransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoyaltyTransactionsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst("user_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] string? type,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var companyId = GetCompanyId();
        var query = _context.LoyaltyTransactions
            .Include(lt => lt.LoyaltyAccount)
            .ThenInclude(la => la!.Customer)
            .Where(lt => lt.LoyaltyAccount != null && lt.LoyaltyAccount.Customer != null && lt.LoyaltyAccount.Customer.CompanyId == companyId);

        if (customerId.HasValue)
            query = query.Where(lt => lt.LoyaltyAccount != null && lt.LoyaltyAccount.CustomerId == customerId.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(lt => lt.Type == type);

        if (dateFrom.HasValue)
            query = query.Where(lt => lt.TransactionDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(lt => lt.TransactionDate <= dateTo.Value.AddDays(1));

        var transactions = await query
            .OrderByDescending(lt => lt.TransactionDate)
            .Take(500)
            .Select(lt => new
            {
                Id = lt.LoyaltyTransactionId,
                CustomerId = lt.LoyaltyAccount != null ? lt.LoyaltyAccount.CustomerId : 0,
                CustomerName = lt.LoyaltyAccount != null && lt.LoyaltyAccount.Customer != null ? lt.LoyaltyAccount.Customer.Name : null,
                TransactionType = lt.Type,
                lt.PointsChange,
                lt.PointsBefore,
                lt.PointsAfter,
                Reference = lt.OrderId != null ? $"Order #{lt.OrderId}" : null,
                lt.Notes,
                CreatedAt = lt.TransactionDate
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpPost("/api/company/loyalty/adjust")]
    public async Task<ActionResult> AdjustPoints([FromBody] AdjustPointsRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId && c.CompanyId == companyId);
        if (customer == null) return NotFound("Customer not found");

        var loyaltyAccount = await _context.LoyaltyAccounts.FirstOrDefaultAsync(la => la.CustomerId == request.CustomerId);
        if (loyaltyAccount == null)
        {
            loyaltyAccount = new LoyaltyAccount
            {
                CustomerId = request.CustomerId,
                PointsBalance = 0
            };
            _context.LoyaltyAccounts.Add(loyaltyAccount);
            await _context.SaveChangesAsync();
        }

        var pointsBefore = loyaltyAccount.PointsBalance;
        var pointsAfter = pointsBefore + request.Points;

        if (pointsAfter < 0) return BadRequest("Insufficient points balance");

        loyaltyAccount.PointsBalance = pointsAfter;
        loyaltyAccount.UpdatedAt = DateTime.UtcNow;

        var transaction = new LoyaltyTransaction
        {
            LoyaltyAccountId = loyaltyAccount.LoyaltyAccountId,
            Type = "Adjust",
            PointsChange = request.Points,
            PointsBefore = pointsBefore,
            PointsAfter = pointsAfter,
            Notes = request.Reason,
            UserId = userId
        };

        _context.LoyaltyTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(new { Id = transaction.LoyaltyTransactionId, NewBalance = pointsAfter });
    }
}

public class AdjustPointsRequest
{
    public int CustomerId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}
