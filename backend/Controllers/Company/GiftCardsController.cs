using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/gift-cards")]
[Authorize(Roles = "CompanyAdmin,Manager,Cashier")]
public class GiftCardsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GiftCardsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<GiftCardListDto>>> GetAll([FromQuery] string? status, [FromQuery] int? branchId)
    {
        var companyId = GetCompanyId();
        var query = _context.GiftCards
            .Include(g => g.BranchIssued)
            .Include(g => g.Customer)
            .Where(g => g.CompanyId == companyId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(g => g.Status == status);

        if (branchId.HasValue)
            query = query.Where(g => g.BranchIssuedId == branchId.Value);

        var cards = await query
            .OrderByDescending(g => g.IssueDate)
            .Select(g => new GiftCardListDto
            {
                Id = g.GiftCardId,
                GiftCardNumber = g.GiftCardNumber,
                BranchIssuedId = g.BranchIssuedId,
                BranchIssuedName = g.BranchIssued!.Name,
                IssueDate = g.IssueDate,
                CurrencyCode = g.CurrencyCode,
                InitialValue = g.InitialValue,
                CurrentBalance = g.CurrentBalance,
                ExpiryDate = g.ExpiryDate,
                Status = g.Status,
                CustomerId = g.CustomerId,
                CustomerName = g.Customer != null ? g.Customer.Name : null
            })
            .ToListAsync();

        return Ok(cards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GiftCardListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var card = await _context.GiftCards
            .Include(g => g.BranchIssued)
            .Include(g => g.Customer)
            .FirstOrDefaultAsync(g => g.GiftCardId == id && g.CompanyId == companyId);

        if (card == null) return NotFound();

        return Ok(new GiftCardListDto
        {
            Id = card.GiftCardId,
            GiftCardNumber = card.GiftCardNumber,
            BranchIssuedId = card.BranchIssuedId,
            BranchIssuedName = card.BranchIssued!.Name,
            IssueDate = card.IssueDate,
            CurrencyCode = card.CurrencyCode,
            InitialValue = card.InitialValue,
            CurrentBalance = card.CurrentBalance,
            ExpiryDate = card.ExpiryDate,
            Status = card.Status,
            CustomerId = card.CustomerId,
            CustomerName = card.Customer?.Name
        });
    }

    [HttpGet("lookup/{number}")]
    public async Task<ActionResult<GiftCardListDto>> LookupByNumber(string number)
    {
        var companyId = GetCompanyId();
        var card = await _context.GiftCards
            .Include(g => g.BranchIssued)
            .FirstOrDefaultAsync(g => g.GiftCardNumber == number && g.CompanyId == companyId);

        if (card == null) return NotFound(new { message = "Gift card not found" });

        return Ok(new GiftCardListDto
        {
            Id = card.GiftCardId,
            GiftCardNumber = card.GiftCardNumber,
            CurrentBalance = card.CurrentBalance,
            Status = card.Status,
            ExpiryDate = card.ExpiryDate
        });
    }

    [HttpPost]
    public async Task<ActionResult<GiftCardListDto>> Create([FromBody] CreateGiftCardRequest request)
    {
        var companyId = GetCompanyId();
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var cardNumber = request.GiftCardNumber;
        if (string.IsNullOrEmpty(cardNumber))
        {
            cardNumber = GenerateGiftCardNumber();
        }

        // Check uniqueness
        var exists = await _context.GiftCards.AnyAsync(g => g.CompanyId == companyId && g.GiftCardNumber == cardNumber);
        if (exists) return BadRequest(new { message = "Gift card number already exists" });

        var card = new GiftCard
        {
            CompanyId = companyId,
            GiftCardNumber = cardNumber,
            BranchIssuedId = request.BranchIssuedId,
            CurrencyCode = request.CurrencyCode,
            InitialValue = request.InitialValue,
            CurrentBalance = request.InitialValue,
            ExpiryDate = request.ExpiryDate,
            CustomerId = request.CustomerId
        };

        _context.GiftCards.Add(card);

        // Create initial load transaction
        var transaction = new GiftCardTransaction
        {
            GiftCardId = card.GiftCardId,
            Type = "Load",
            Amount = request.InitialValue,
            CurrencyCode = request.CurrencyCode,
            BalanceBefore = 0,
            BalanceAfter = request.InitialValue,
            UserId = userId,
            Notes = "Initial load"
        };

        await _context.SaveChangesAsync();

        transaction.GiftCardId = card.GiftCardId;
        _context.GiftCardTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(new GiftCardListDto
        {
            Id = card.GiftCardId,
            GiftCardNumber = card.GiftCardNumber,
            InitialValue = card.InitialValue,
            CurrentBalance = card.CurrentBalance,
            Status = card.Status
        });
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<GiftCardTransactionDto>>> GetTransactions(int id)
    {
        var companyId = GetCompanyId();
        var card = await _context.GiftCards.FirstOrDefaultAsync(g => g.GiftCardId == id && g.CompanyId == companyId);
        if (card == null) return NotFound();

        var transactions = await _context.GiftCardTransactions
            .Where(t => t.GiftCardId == id)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new GiftCardTransactionDto
            {
                Id = t.GiftCardTransactionId,
                TransactionDate = t.TransactionDate,
                Type = t.Type,
                Amount = t.Amount,
                BalanceBefore = t.BalanceBefore,
                BalanceAfter = t.BalanceAfter,
                Notes = t.Notes
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpPatch("{id}/block")]
    public async Task<IActionResult> Block(int id)
    {
        var companyId = GetCompanyId();
        var card = await _context.GiftCards.FirstOrDefaultAsync(g => g.GiftCardId == id && g.CompanyId == companyId);
        if (card == null) return NotFound();

        card.Status = "Blocked";
        await _context.SaveChangesAsync();

        return Ok(new { card.Status });
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var companyId = GetCompanyId();
        var card = await _context.GiftCards.FirstOrDefaultAsync(g => g.GiftCardId == id && g.CompanyId == companyId);
        if (card == null) return NotFound();

        if (card.CurrentBalance > 0)
            card.Status = "Active";
        else
            card.Status = "UsedUp";

        await _context.SaveChangesAsync();

        return Ok(new { card.Status });
    }

    private string GenerateGiftCardNumber()
    {
        var random = new Random();
        return $"GC{DateTime.Now:yyyyMMdd}{random.Next(100000, 999999)}";
    }
}
