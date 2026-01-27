using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize]
public class ShiftsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShiftsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] int? branchId, [FromQuery] string? status)
    {
        var companyId = GetCompanyId();
        var query = _context.Shifts
            .Include(s => s.Branch)
            .Include(s => s.CashierUser)
            .Where(s => s.CompanyId == companyId)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.Status == status);

        var shifts = await query
            .OrderByDescending(s => s.OpenTime)
            .Select(s => new
            {
                s.ShiftId,
                s.BranchId,
                BranchName = s.Branch.Name,
                s.CashierUserId,
                CashierName = s.CashierUser.FullName,
                s.OpenTime,
                s.CloseTime,
                s.OpeningCash,
                s.ClosingCash,
                s.ExpectedCash,
                s.CashDifference,
                s.Status,
                s.Notes
            })
            .ToListAsync();

        return Ok(shifts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var shift = await _context.Shifts
            .Include(s => s.Branch)
            .Include(s => s.CashierUser)
            .Include(s => s.Orders)
            .FirstOrDefaultAsync(s => s.ShiftId == id && s.CompanyId == companyId);

        if (shift == null)
            return NotFound(new { message = "Shift not found" });

        return Ok(new
        {
            shift.ShiftId,
            shift.BranchId,
            BranchName = shift.Branch.Name,
            shift.CashierUserId,
            CashierName = shift.CashierUser.FullName,
            shift.OpenTime,
            shift.CloseTime,
            shift.OpeningCash,
            shift.ClosingCash,
            shift.ExpectedCash,
            shift.CashDifference,
            shift.Status,
            shift.Notes,
            OrdersCount = shift.Orders.Count
        });
    }

    [HttpGet("current")]
    public async Task<ActionResult> GetCurrentShift([FromQuery] int branchId)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var shift = await _context.Shifts
            .Include(s => s.Branch)
            .Include(s => s.CashierUser)
            .FirstOrDefaultAsync(s => 
                s.CompanyId == companyId && 
                s.BranchId == branchId && 
                s.CashierUserId == userId && 
                s.Status == "Open");

        if (shift == null)
            return Ok(new { hasOpenShift = false });

        return Ok(new
        {
            hasOpenShift = true,
            shift = new
            {
                shift.ShiftId,
                shift.BranchId,
                BranchName = shift.Branch.Name,
                shift.CashierUserId,
                CashierName = shift.CashierUser.FullName,
                shift.OpenTime,
                shift.OpeningCash,
                shift.Status
            }
        });
    }

    [HttpPost("open")]
    public async Task<ActionResult> OpenShift([FromBody] OpenShiftRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        // Check if user already has an open shift
        var existingShift = await _context.Shifts
            .FirstOrDefaultAsync(s => 
                s.CompanyId == companyId && 
                s.CashierUserId == userId && 
                s.Status == "Open");

        if (existingShift != null)
            return BadRequest(new { message = "You already have an open shift. Please close it first." });

        var shift = new Shift
        {
            CompanyId = companyId,
            BranchId = request.BranchId,
            CashierUserId = userId,
            OpenTime = DateTime.UtcNow,
            OpeningCash = request.OpeningCash,
            Status = "Open",
            Notes = request.Notes
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Shift opened successfully", shiftId = shift.ShiftId });
    }

    [HttpPost("{id}/close")]
    public async Task<ActionResult> CloseShift(int id, [FromBody] CloseShiftRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var shift = await _context.Shifts
            .Include(s => s.Orders)
                .ThenInclude(o => o.OrderPayments)
            .FirstOrDefaultAsync(s => s.ShiftId == id && s.CompanyId == companyId);

        if (shift == null)
            return NotFound(new { message = "Shift not found" });

        if (shift.Status != "Open")
            return BadRequest(new { message = "Shift is already closed" });

        if (shift.CashierUserId != userId)
            return BadRequest(new { message = "You can only close your own shift" });

        // Calculate expected cash from orders
        var cashPayments = shift.Orders
            .Where(o => o.PaymentStatus == "Paid")
            .SelectMany(o => o.OrderPayments)
            .Where(p => p.PaymentMethod.Type == "Cash")
            .Sum(p => p.AmountInOrderCurrency);

        var expectedCash = shift.OpeningCash + cashPayments;
        var cashDifference = request.ClosingCash - expectedCash;

        shift.CloseTime = DateTime.UtcNow;
        shift.ClosingCash = request.ClosingCash;
        shift.ExpectedCash = expectedCash;
        shift.CashDifference = cashDifference;
        shift.Status = "Closed";
        shift.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Shift closed successfully",
            expectedCash,
            closingCash = request.ClosingCash,
            cashDifference
        });
    }
}

public class OpenShiftRequest
{
    public int BranchId { get; set; }
    public decimal OpeningCash { get; set; }
    public string? Notes { get; set; }
}

public class CloseShiftRequest
{
    public decimal ClosingCash { get; set; }
    public string? Notes { get; set; }
}
