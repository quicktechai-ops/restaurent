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
public class PrintersController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrintersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<PrinterListDto>>> GetAll([FromQuery] int? branchId, [FromQuery] string? type)
    {
        var companyId = GetCompanyId();
        var query = _context.Printers
            .Include(p => p.Branch)
            .Where(p => p.Branch!.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(p => p.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(p => p.PrinterType == type);

        var printers = await query
            .OrderBy(p => p.Branch!.Name).ThenBy(p => p.Name)
            .Select(p => new PrinterListDto
            {
                Id = p.PrinterId,
                BranchId = p.BranchId,
                BranchName = p.Branch!.Name,
                Name = p.Name,
                PrinterType = p.PrinterType,
                ConnectionType = p.ConnectionType,
                ConnectionString = p.ConnectionString,
                PaperWidth = p.PaperWidth,
                IsDefault = p.IsDefault,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return Ok(printers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrinterListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var printer = await _context.Printers
            .Include(p => p.Branch)
            .FirstOrDefaultAsync(p => p.PrinterId == id && p.Branch!.CompanyId == companyId);

        if (printer == null) return NotFound();

        return Ok(new PrinterListDto
        {
            Id = printer.PrinterId,
            BranchId = printer.BranchId,
            BranchName = printer.Branch!.Name,
            Name = printer.Name,
            PrinterType = printer.PrinterType,
            ConnectionType = printer.ConnectionType,
            ConnectionString = printer.ConnectionString,
            PaperWidth = printer.PaperWidth,
            IsDefault = printer.IsDefault,
            IsActive = printer.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<PrinterListDto>> Create([FromBody] CreatePrinterRequest request)
    {
        var companyId = GetCompanyId();

        var printer = new Printer
        {
            BranchId = request.BranchId,
            Name = request.Name,
            PrinterType = request.PrinterType,
            ConnectionType = request.ConnectionType,
            ConnectionString = request.ConnectionString,
            PaperWidth = request.PaperWidth,
            IsDefault = request.IsDefault
        };

        // If this is default, unset other defaults of same type
        if (request.IsDefault)
        {
            var others = await _context.Printers
                .Where(p => p.Branch!.CompanyId == companyId && p.BranchId == request.BranchId && p.PrinterType == request.PrinterType)
                .ToListAsync();
            foreach (var p in others) p.IsDefault = false;
        }

        _context.Printers.Add(printer);
        await _context.SaveChangesAsync();

        return Ok(new PrinterListDto
        {
            Id = printer.PrinterId,
            BranchId = printer.BranchId,
            Name = printer.Name,
            PrinterType = printer.PrinterType,
            IsActive = printer.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PrinterListDto>> Update(int id, [FromBody] UpdatePrinterRequest request)
    {
        var companyId = GetCompanyId();
        var printer = await _context.Printers.FirstOrDefaultAsync(p => p.PrinterId == id && p.Branch!.CompanyId == companyId);
        if (printer == null) return NotFound();

        printer.BranchId = request.BranchId;
        printer.Name = request.Name;
        printer.PrinterType = request.PrinterType;
        printer.ConnectionType = request.ConnectionType;
        printer.ConnectionString = request.ConnectionString;
        printer.PaperWidth = request.PaperWidth;
        printer.IsDefault = request.IsDefault;
        printer.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new PrinterListDto
        {
            Id = printer.PrinterId,
            Name = printer.Name,
            IsActive = printer.IsActive
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var companyId = GetCompanyId();
        var printer = await _context.Printers.FirstOrDefaultAsync(p => p.PrinterId == id && p.Branch!.CompanyId == companyId);
        if (printer == null) return NotFound();

        printer.IsActive = !printer.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { printer.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var printer = await _context.Printers.FirstOrDefaultAsync(p => p.PrinterId == id && p.Branch!.CompanyId == companyId);
        if (printer == null) return NotFound();

        _context.Printers.Remove(printer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
