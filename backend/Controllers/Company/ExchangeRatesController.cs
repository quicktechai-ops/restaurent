using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.DTOs;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/exchange-rates")]
[Authorize(Roles = "CompanyAdmin,Manager")]
public class ExchangeRatesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExchangeRatesController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<ExchangeRateListDto>>> GetAll()
    {
        var companyId = GetCompanyId();
        var rates = await _context.ExchangeRates
            .Where(e => e.CompanyId == companyId)
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

    [HttpPost]
    public async Task<ActionResult<ExchangeRateListDto>> Create([FromBody] CreateExchangeRateRequest request)
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

    [HttpPut("{id}")]
    public async Task<ActionResult<ExchangeRateListDto>> Update(int id, [FromBody] CreateExchangeRateRequest request)
    {
        var companyId = GetCompanyId();
        var rate = await _context.ExchangeRates.FirstOrDefaultAsync(e => e.ExchangeRateId == id && e.CompanyId == companyId);
        if (rate == null) return NotFound();

        rate.BaseCurrencyCode = request.BaseCurrencyCode;
        rate.ForeignCurrencyCode = request.ForeignCurrencyCode;
        rate.Rate = request.Rate;
        rate.ValidFrom = request.ValidFrom;
        rate.ValidTo = request.ValidTo;

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var rate = await _context.ExchangeRates.FirstOrDefaultAsync(e => e.ExchangeRateId == id && e.CompanyId == companyId);
        if (rate == null) return NotFound();

        _context.ExchangeRates.Remove(rate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
