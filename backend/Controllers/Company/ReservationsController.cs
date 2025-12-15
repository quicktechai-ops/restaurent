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
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReservationsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult<List<ReservationListDto>>> GetAll(
        [FromQuery] int? branchId, 
        [FromQuery] DateTime? date,
        [FromQuery] string? status)
    {
        var companyId = GetCompanyId();
        var query = _context.Reservations
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .Include(r => r.Deposit)
            .Where(r => r.Branch!.CompanyId == companyId);

        if (branchId.HasValue)
            query = query.Where(r => r.BranchId == branchId.Value);

        if (date.HasValue)
            query = query.Where(r => r.ReservationDate.Date == date.Value.Date);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        var reservations = await query
            .OrderBy(r => r.ReservationDate).ThenBy(r => r.StartTime)
            .Select(r => new ReservationListDto
            {
                Id = r.ReservationId,
                BranchId = r.BranchId,
                BranchName = r.Branch!.Name,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer != null ? r.Customer.Name : r.CustomerName,
                CustomerPhone = r.Customer != null ? r.Customer.Phone : r.CustomerPhone,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                DurationMinutes = r.DurationMinutes,
                PartySize = r.PartySize,
                TableId = r.TableId,
                TableName = r.Table != null ? r.Table.TableName : null,
                Status = r.Status,
                Channel = r.Channel,
                Notes = r.Notes,
                HasDeposit = r.Deposit != null,
                DepositAmount = r.Deposit != null ? r.Deposit.Amount : null,
                DepositStatus = r.Deposit != null ? r.Deposit.Status : null
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationListDto>> GetById(int id)
    {
        var companyId = GetCompanyId();
        var reservation = await _context.Reservations
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .Include(r => r.Deposit)
            .FirstOrDefaultAsync(r => r.ReservationId == id && r.Branch!.CompanyId == companyId);

        if (reservation == null) return NotFound();

        return Ok(new ReservationListDto
        {
            Id = reservation.ReservationId,
            BranchId = reservation.BranchId,
            BranchName = reservation.Branch!.Name,
            CustomerId = reservation.CustomerId,
            CustomerName = reservation.Customer?.Name ?? reservation.CustomerName,
            CustomerPhone = reservation.Customer?.Phone ?? reservation.CustomerPhone,
            ReservationDate = reservation.ReservationDate,
            StartTime = reservation.StartTime,
            DurationMinutes = reservation.DurationMinutes,
            PartySize = reservation.PartySize,
            TableId = reservation.TableId,
            TableName = reservation.Table?.TableName,
            Status = reservation.Status,
            Channel = reservation.Channel,
            Notes = reservation.Notes,
            HasDeposit = reservation.Deposit != null,
            DepositAmount = reservation.Deposit?.Amount,
            DepositStatus = reservation.Deposit?.Status
        });
    }

    [HttpPost]
    public async Task<ActionResult<ReservationListDto>> Create([FromBody] CreateReservationRequest request)
    {
        var companyId = GetCompanyId();
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        var reservation = new Reservation
        {
            BranchId = request.BranchId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            ReservationDate = request.ReservationDate,
            StartTime = request.StartTime,
            DurationMinutes = request.DurationMinutes,
            PartySize = request.PartySize,
            TableId = request.TableId,
            Channel = request.Channel,
            Notes = request.Notes,
            CreatedByUserId = userId
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Create deposit if required
        if (request.RequireDeposit && request.DepositAmount.HasValue)
        {
            var deposit = new ReservationDeposit
            {
                ReservationId = reservation.ReservationId,
                Amount = request.DepositAmount.Value,
                CurrencyCode = request.DepositCurrencyCode,
                Status = "Pending"
            };
            _context.ReservationDeposits.Add(deposit);
            await _context.SaveChangesAsync();
        }

        return Ok(new ReservationListDto
        {
            Id = reservation.ReservationId,
            BranchId = reservation.BranchId,
            ReservationDate = reservation.ReservationDate,
            StartTime = reservation.StartTime,
            PartySize = reservation.PartySize,
            Status = reservation.Status
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReservationListDto>> Update(int id, [FromBody] UpdateReservationRequest request)
    {
        var companyId = GetCompanyId();
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id && r.Branch!.CompanyId == companyId);
        if (reservation == null) return NotFound();

        reservation.BranchId = request.BranchId;
        reservation.CustomerId = request.CustomerId;
        reservation.CustomerName = request.CustomerName;
        reservation.CustomerPhone = request.CustomerPhone;
        reservation.ReservationDate = request.ReservationDate;
        reservation.StartTime = request.StartTime;
        reservation.DurationMinutes = request.DurationMinutes;
        reservation.PartySize = request.PartySize;
        reservation.TableId = request.TableId;
        reservation.Channel = request.Channel;
        reservation.Notes = request.Notes;
        reservation.Status = request.Status;
        
        await _context.SaveChangesAsync();

        return Ok(new ReservationListDto
        {
            Id = reservation.ReservationId,
            Status = reservation.Status
        });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReservationStatusRequest request)
    {
        var companyId = GetCompanyId();
        var reservation = await _context.Reservations
            .Include(r => r.Deposit)
            .FirstOrDefaultAsync(r => r.ReservationId == id && r.Branch!.CompanyId == companyId);
        if (reservation == null) return NotFound();

        reservation.Status = request.Status;
        
        // Handle deposit forfeiture on no-show
        if (request.Status == "NoShow" && reservation.Deposit != null && reservation.Deposit.Status == "Paid")
        {
            reservation.Deposit.Status = "Forfeited";
        }

        await _context.SaveChangesAsync();

        return Ok(new { reservation.Status });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == id && r.Branch!.CompanyId == companyId);
        if (reservation == null) return NotFound();

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class UpdateReservationStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
