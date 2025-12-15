using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class CurrenciesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CurrenciesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<CurrencyListDto>>> GetAll()
    {
        var currencies = await _context.Currencies
            .Select(c => new CurrencyListDto
            {
                CurrencyCode = c.CurrencyCode,
                Name = c.Name,
                Symbol = c.Symbol,
                DecimalPlaces = c.DecimalPlaces,
                IsDefault = c.IsDefault,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return Ok(currencies);
    }

    [HttpPost]
    public async Task<ActionResult<CurrencyListDto>> Create([FromBody] CreateCurrencyRequest request)
    {
        var existing = await _context.Currencies.FindAsync(request.CurrencyCode);
        if (existing != null)
            return BadRequest("Currency code already exists");

        var currency = new Currency
        {
            CurrencyCode = request.CurrencyCode.ToUpper(),
            Name = request.Name,
            Symbol = request.Symbol,
            DecimalPlaces = (short)request.DecimalPlaces,
            IsDefault = false,
            IsActive = true
        };

        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();

        return Ok(new CurrencyListDto
        {
            CurrencyCode = currency.CurrencyCode,
            Name = currency.Name,
            Symbol = currency.Symbol,
            DecimalPlaces = currency.DecimalPlaces,
            IsDefault = currency.IsDefault,
            IsActive = currency.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CurrencyListDto>> Update(string id, [FromBody] UpdateCurrencyRequest request)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null) return NotFound();

        currency.Name = request.Name;
        currency.Symbol = request.Symbol;
        currency.DecimalPlaces = (short)request.DecimalPlaces;

        await _context.SaveChangesAsync();

        return Ok(new CurrencyListDto
        {
            CurrencyCode = currency.CurrencyCode,
            Name = currency.Name,
            Symbol = currency.Symbol,
            DecimalPlaces = currency.DecimalPlaces,
            IsDefault = currency.IsDefault,
            IsActive = currency.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(string id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null) return NotFound();

        currency.IsActive = !currency.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { currency.IsActive });
    }

    [HttpPatch("{id}/set-default")]
    public async Task<IActionResult> SetDefault(string id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency == null) return NotFound();

        // Unset other defaults
        var others = await _context.Currencies.Where(c => c.IsDefault).ToListAsync();
        foreach (var c in others) c.IsDefault = false;

        currency.IsDefault = true;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("exchange-rates")]
    public async Task<ActionResult<List<ExchangeRateListDto>>> GetExchangeRates()
    {
        var companyId = GetCompanyId();
        var rates = await _context.ExchangeRates
            .Where(e => e.CompanyId == companyId && (e.ValidTo == null || e.ValidTo > DateTime.UtcNow))
            .OrderByDescending(e => e.ValidFrom)
            .Select(e => new ExchangeRateListDto
            {
                Id = e.ExchangeRateId,
                BaseCurrencyCode = e.BaseCurrencyCode,
                ForeignCurrencyCode = e.ForeignCurrencyCode,
                Rate = e.Rate,
                ValidFrom = e.ValidFrom,
                ValidTo = e.ValidTo
            })
            .ToListAsync();

        return Ok(rates);
    }

    [HttpPost("exchange-rates")]
    public async Task<ActionResult<ExchangeRateListDto>> CreateExchangeRate([FromBody] CreateExchangeRateRequest request)
    {
        var companyId = GetCompanyId();
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var rate = new ExchangeRate
        {
            CompanyId = companyId,
            BaseCurrencyCode = request.BaseCurrencyCode,
            ForeignCurrencyCode = request.ForeignCurrencyCode,
            Rate = request.Rate,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            CreatedByUserId = userId
        };

        _context.ExchangeRates.Add(rate);
        await _context.SaveChangesAsync();

        return Ok(new ExchangeRateListDto
        {
            Id = rate.ExchangeRateId,
            BaseCurrencyCode = rate.BaseCurrencyCode,
            ForeignCurrencyCode = rate.ForeignCurrencyCode,
            Rate = rate.Rate,
            ValidFrom = rate.ValidFrom,
            ValidTo = rate.ValidTo
        });
    }

    [HttpDelete("exchange-rates/{id}")]
    public async Task<IActionResult> DeleteExchangeRate(int id)
    {
        var companyId = GetCompanyId();
        var rate = await _context.ExchangeRates.FirstOrDefaultAsync(e => e.ExchangeRateId == id && e.CompanyId == companyId);
        if (rate == null) return NotFound();

        _context.ExchangeRates.Remove(rate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
