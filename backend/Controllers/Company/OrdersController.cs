using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCompanyId() => int.Parse(User.FindFirst("company_id")?.Value ?? "0");
    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int? branchId,
        [FromQuery] string? status,
        [FromQuery] string? orderType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var companyId = GetCompanyId();
        var query = _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.Table)
            .Include(o => o.Customer)
            .Include(o => o.WaiterUser)
            .Where(o => o.CompanyId == companyId)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(o => o.BranchId == branchId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.OrderStatus == status);

        if (!string.IsNullOrEmpty(orderType))
            query = query.Where(o => o.OrderType == orderType);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.CreatedAt <= toDate.Value);

        var total = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                o.OrderId,
                o.OrderNumber,
                o.OrderType,
                o.OrderStatus,
                o.BranchId,
                BranchName = o.Branch.Name,
                o.TableId,
                TableName = o.Table != null ? o.Table.TableName : null,
                o.CustomerId,
                CustomerName = o.Customer != null ? o.Customer.Name : null,
                WaiterName = o.WaiterUser != null ? o.WaiterUser.FullName : null,
                o.SubTotal,
                o.GrandTotal,
                o.PaymentStatus,
                o.CreatedAt
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, orders });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var order = await _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.Table)
            .Include(o => o.Customer)
            .Include(o => o.WaiterUser)
            .Include(o => o.CashierUser)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.MenuItem)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.MenuItemSize)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.OrderLineModifiers)
                    .ThenInclude(olm => olm.Modifier)
            .Include(o => o.OrderPayments)
                .ThenInclude(op => op.PaymentMethod)
            .Include(o => o.DeliveryDetails)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(new
        {
            order.OrderId,
            order.OrderNumber,
            order.OrderType,
            order.OrderStatus,
            order.BranchId,
            BranchName = order.Branch.Name,
            order.TableId,
            TableName = order.Table?.TableName,
            order.CustomerId,
            CustomerName = order.Customer?.Name,
            CustomerPhone = order.Customer?.Phone,
            WaiterName = order.WaiterUser?.FullName,
            CashierName = order.CashierUser?.FullName,
            order.CurrencyCode,
            order.SubTotal,
            order.TotalLineDiscount,
            order.BillDiscountPercent,
            order.BillDiscountAmount,
            order.ServiceChargePercent,
            order.ServiceChargeAmount,
            order.TaxPercent,
            order.TaxAmount,
            order.DeliveryFee,
            order.TipsAmount,
            order.GrandTotal,
            order.TotalPaid,
            order.BalanceDue,
            order.PaymentStatus,
            order.LoyaltyPointsEarned,
            order.LoyaltyPointsRedeemed,
            order.Notes,
            order.CreatedAt,
            order.PaidAt,
            Lines = order.OrderLines.Select(ol => new
            {
                ol.OrderLineId,
                ol.MenuItemId,
                MenuItemName = ol.MenuItem.Name,
                ol.MenuItemSizeId,
                SizeName = ol.MenuItemSize?.SizeName,
                ol.Quantity,
                ol.BaseUnitPrice,
                ol.ModifiersExtraPrice,
                ol.EffectiveUnitPrice,
                ol.LineGross,
                ol.DiscountPercent,
                ol.DiscountAmount,
                ol.LineNet,
                ol.Notes,
                ol.KitchenStatus,
                Modifiers = ol.OrderLineModifiers.Select(m => new
                {
                    m.ModifierId,
                    ModifierName = m.Modifier.Name,
                    m.Quantity,
                    m.ExtraPrice,
                    m.TotalPrice
                })
            }),
            Payments = order.OrderPayments.Select(p => new
            {
                p.OrderPaymentId,
                p.PaymentMethodId,
                PaymentMethodName = p.PaymentMethod.Name,
                p.Amount,
                p.CurrencyCode,
                p.Reference,
                p.CreatedAt
            }),
            DeliveryDetails = order.DeliveryDetails != null ? new
            {
                order.DeliveryDetails.AddressLine,
                order.DeliveryDetails.City,
                order.DeliveryDetails.Area,
                order.DeliveryDetails.Phone,
                order.DeliveryDetails.DriverName,
                order.DeliveryDetails.DeliveryFeeCalculated
            } : null
        });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrderRequest request)
    {
        try
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            // Get branch for tax/service charge rates
            var branch = await _context.Branches.FindAsync(request.BranchId);
            if (branch == null || branch.CompanyId != companyId)
                return BadRequest(new { message = "Invalid branch" });

            // Generate order number
            var today = DateTime.UtcNow.Date;
            var orderCount = await _context.Orders
                .Where(o => o.BranchId == request.BranchId && o.CreatedAt >= today)
                .CountAsync();
            var orderNumber = $"{DateTime.UtcNow:yyyyMMdd}-{(orderCount + 1):D4}";

            // Check if user exists in users table (company admins may not be there)
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            
            var order = new Order
            {
                CompanyId = companyId,
                BranchId = request.BranchId,
                ShiftId = request.ShiftId,
                OrderNumber = orderNumber,
                OrderType = request.OrderType,
                TableId = request.TableId,
                CustomerId = request.CustomerId,
                WaiterUserId = userExists ? userId : null,
                CashierUserId = userExists ? userId : null,
                OrderStatus = "Draft",
                CurrencyCode = branch.DefaultCurrencyCode ?? "USD",
                TaxPercent = branch.VatPercent,
                ServiceChargePercent = branch.ServiceChargePercent,
                Notes = request.Notes
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add status history
            _context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.OrderId,
                NewStatus = "Draft",
                UserId = userExists ? userId : null
            });
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order created", orderId = order.OrderId, orderNumber });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating order", error = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [HttpPost("{id}/lines")]
    public async Task<ActionResult> AddLine(int id, [FromBody] AddLineRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

        var order = await _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        if (order.OrderStatus == "Paid" || order.OrderStatus == "Voided")
            return BadRequest(new { message = "Cannot modify a paid or voided order" });

        // Get menu item
        var menuItem = await _context.MenuItems
            .Include(m => m.KitchenStation)
            .FirstOrDefaultAsync(m => m.MenuItemId == request.MenuItemId && m.CompanyId == companyId);

        if (menuItem == null)
            return BadRequest(new { message = "Menu item not found" });

        decimal basePrice = menuItem.DefaultPrice;

        // If size selected, get size price
        if (request.MenuItemSizeId.HasValue)
        {
            var size = await _context.MenuItemSizes
                .FirstOrDefaultAsync(s => s.MenuItemSizeId == request.MenuItemSizeId.Value && s.MenuItemId == menuItem.MenuItemId);
            if (size != null)
                basePrice = size.Price;
        }

        // Calculate modifiers extra price
        decimal modifiersExtra = 0;
        var orderLineModifiers = new List<OrderLineModifier>();

        if (request.Modifiers != null && request.Modifiers.Any())
        {
            foreach (var mod in request.Modifiers)
            {
                var modifier = await _context.Modifiers.FindAsync(mod.ModifierId);
                if (modifier != null)
                {
                    var totalPrice = modifier.ExtraPrice * mod.Quantity;
                    modifiersExtra += totalPrice;
                    orderLineModifiers.Add(new OrderLineModifier
                    {
                        ModifierId = mod.ModifierId,
                        Quantity = mod.Quantity,
                        ExtraPrice = modifier.ExtraPrice,
                        TotalPrice = totalPrice
                    });
                }
            }
        }

        var effectiveUnitPrice = basePrice + modifiersExtra;
        var lineGross = effectiveUnitPrice * request.Quantity;
        var discountAmount = request.DiscountPercent > 0 ? lineGross * request.DiscountPercent / 100 : 0;
        var lineNet = lineGross - discountAmount;

        var orderLine = new OrderLine
        {
            OrderId = id,
            MenuItemId = request.MenuItemId,
            MenuItemSizeId = request.MenuItemSizeId,
            Quantity = request.Quantity,
            BaseUnitPrice = basePrice,
            ModifiersExtraPrice = modifiersExtra,
            EffectiveUnitPrice = effectiveUnitPrice,
            LineGross = lineGross,
            DiscountPercent = request.DiscountPercent,
            DiscountAmount = discountAmount,
            LineNet = lineNet,
            Notes = request.Notes,
            KitchenStationId = menuItem.KitchenStationId,
            CreatedByUserId = userExists ? userId : null
        };

        _context.OrderLines.Add(orderLine);
        await _context.SaveChangesAsync();

        // Add modifiers
        foreach (var olm in orderLineModifiers)
        {
            olm.OrderLineId = orderLine.OrderLineId;
            _context.OrderLineModifiers.Add(olm);
        }
        await _context.SaveChangesAsync();

        // Recalculate order totals
        await RecalculateOrderTotals(order);

        return Ok(new { message = "Line added", orderLineId = orderLine.OrderLineId });
    }

    [HttpDelete("{orderId}/lines/{lineId}")]
    public async Task<ActionResult> RemoveLine(int orderId, int lineId)
    {
        var companyId = GetCompanyId();

        var order = await _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        if (order.OrderStatus == "Paid" || order.OrderStatus == "Voided")
            return BadRequest(new { message = "Cannot modify a paid or voided order" });

        var line = order.OrderLines.FirstOrDefault(l => l.OrderLineId == lineId);
        if (line == null)
            return NotFound(new { message = "Line not found" });

        _context.OrderLines.Remove(line);
        await _context.SaveChangesAsync();

        await RecalculateOrderTotals(order);

        return Ok(new { message = "Line removed" });
    }

    [HttpPut("{id}/discount")]
    public async Task<ActionResult> ApplyDiscount(int id, [FromBody] ApplyDiscountRequest request)
    {
        var companyId = GetCompanyId();

        var order = await _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        order.BillDiscountPercent = request.DiscountPercent;
        await RecalculateOrderTotals(order);

        return Ok(new { message = "Discount applied", grandTotal = order.GrandTotal });
    }

    [HttpPost("{id}/send-to-kitchen")]
    public async Task<ActionResult> SendToKitchen(int id)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var order = await _context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        if (!order.OrderLines.Any())
            return BadRequest(new { message = "Order has no items" });

        // Update lines that haven't been sent
        var newLines = order.OrderLines.Where(l => l.KitchenStatus == "New").ToList();
        foreach (var line in newLines)
        {
            line.KitchenStatus = "SentToKitchen";
            line.SentToKitchenAt = DateTime.UtcNow;
        }

        if (order.OrderStatus == "Draft")
        {
            order.OrderStatus = "SentToKitchen";
            _context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = id,
                OldStatus = "Draft",
                NewStatus = "SentToKitchen",
                UserId = userId
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Order sent to kitchen", linesSent = newLines.Count });
    }

    [HttpPost("{id}/pay")]
    public async Task<ActionResult> ProcessPayment(int id, [FromBody] PaymentRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

        var order = await _context.Orders
            .Include(o => o.OrderPayments)
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        if (order.PaymentStatus == "Paid")
            return BadRequest(new { message = "Order is already paid" });
        
        // Always recalculate order totals to ensure accuracy
        if (order.OrderLines.Any())
        {
            order.SubTotal = order.OrderLines.Sum(l => l.LineGross);
            order.TotalLineDiscount = order.OrderLines.Sum(l => l.DiscountAmount);
            var netAfterLineDiscount = order.SubTotal - order.TotalLineDiscount;
            order.BillDiscountAmount = order.BillDiscountPercent > 0 ? netAfterLineDiscount * order.BillDiscountPercent / 100 : 0;
            var netBeforeServiceAndTax = netAfterLineDiscount - order.BillDiscountAmount;
            order.ServiceChargeAmount = order.ServiceChargePercent > 0 ? netBeforeServiceAndTax * order.ServiceChargePercent / 100 : 0;
            var netBeforeTax = netBeforeServiceAndTax + order.ServiceChargeAmount + order.DeliveryFee;
            order.TaxAmount = order.TaxPercent > 0 ? netBeforeTax * order.TaxPercent / 100 : 0;
            order.GrandTotal = netBeforeTax + order.TaxAmount + order.TipsAmount;
            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"[PAY] Order {id}: GrandTotal={order.GrandTotal}, Payment Amount={request.Payments.Sum(p => p.Amount)}");

        foreach (var payment in request.Payments)
        {
            var orderPayment = new OrderPayment
            {
                OrderId = id,
                PaymentMethodId = payment.PaymentMethodId,
                CurrencyCode = payment.CurrencyCode ?? order.CurrencyCode,
                Amount = payment.Amount,
                AmountInOrderCurrency = payment.Amount, // TODO: Apply exchange rate if different currency
                ExchangeRateToOrderCurrency = 1,
                Reference = payment.Reference,
                GiftCardId = payment.GiftCardId,
                LoyaltyPointsUsed = payment.LoyaltyPointsUsed,
                UserId = userExists ? userId : null
            };
            _context.OrderPayments.Add(orderPayment);
        }

        await _context.SaveChangesAsync();

        // Recalculate payment totals - reload order payments from DB
        var allPayments = await _context.OrderPayments.Where(p => p.OrderId == id).ToListAsync();
        var totalPaid = allPayments.Sum(p => p.AmountInOrderCurrency);
        order.TotalPaid = Math.Round(totalPaid, 2);
        order.BalanceDue = Math.Round(order.GrandTotal - totalPaid, 2);

        if (order.BalanceDue <= 0.01m) // Use small tolerance for floating point
        {
            order.PaymentStatus = "Paid";
            order.OrderStatus = "Paid";
            order.PaidAt = DateTime.UtcNow;
            
            _context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = id,
                OldStatus = order.OrderStatus,
                NewStatus = "Paid",
                UserId = userExists ? userId : null
            });

            // Award loyalty points if customer is enrolled
            if (order.CustomerId.HasValue)
            {
                await AwardLoyaltyPoints(order);
            }

            // Deduct inventory based on recipes
            await DeductInventory(order);
        }
        else if (totalPaid > 0)
        {
            order.PaymentStatus = "PartiallyPaid";
        }

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = order.PaymentStatus == "Paid" ? "Payment complete" : "Partial payment recorded",
            totalPaid = order.TotalPaid,
            balanceDue = order.BalanceDue,
            paymentStatus = order.PaymentStatus,
            change = order.BalanceDue < 0 ? Math.Abs(order.BalanceDue) : 0
        });
    }

    private async Task AwardLoyaltyPoints(Order order)
    {
        try
        {
            // Check if customer has a loyalty account
            var loyaltyAccount = await _context.LoyaltyAccounts
                .FirstOrDefaultAsync(la => la.CustomerId == order.CustomerId);

            if (loyaltyAccount == null) return;

            // Get loyalty settings
            var settings = await _context.LoyaltySettings
                .FirstOrDefaultAsync(s => s.CompanyId == order.CompanyId);

            if (settings == null) return;

            // Calculate points to earn
            // Default: 1 point per $10 spent
            decimal amountForPoints = settings.EarnOnNetBeforeTax 
                ? (order.GrandTotal - order.TaxAmount) 
                : order.GrandTotal;

            decimal pointsEarned = Math.Floor(amountForPoints / settings.AmountUnit) * settings.PointsPerAmount;

            if (pointsEarned <= 0) return;

            // Update loyalty account
            var pointsBefore = loyaltyAccount.PointsBalance;
            loyaltyAccount.PointsBalance += pointsEarned;
            loyaltyAccount.UpdatedAt = DateTime.UtcNow;

            // Create loyalty transaction
            var transaction = new LoyaltyTransaction
            {
                LoyaltyAccountId = loyaltyAccount.LoyaltyAccountId,
                TransactionDate = DateTime.UtcNow,
                Type = "Earn",
                PointsChange = pointsEarned,
                PointsBefore = pointsBefore,
                PointsAfter = loyaltyAccount.PointsBalance,
                OrderId = order.OrderId,
                Notes = $"Points earned from order #{order.OrderNumber}"
            };

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[LOYALTY] Customer {order.CustomerId} earned {pointsEarned} points from order {order.OrderNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOYALTY ERROR] {ex.Message}");
        }
    }

    private async Task DeductInventory(Order order)
    {
        try
        {
            // Load order lines if not already loaded
            if (!order.OrderLines.Any())
            {
                await _context.Entry(order).Collection(o => o.OrderLines).LoadAsync();
            }

            foreach (var line in order.OrderLines)
            {
                // Find recipe for this menu item (and size if applicable)
                var recipe = await _context.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefaultAsync(r => 
                        r.MenuItemId == line.MenuItemId && 
                        (line.MenuItemSizeId == null || r.MenuItemSizeId == line.MenuItemSizeId) &&
                        r.IsActive);

                if (recipe == null) continue;

                foreach (var ingredient in recipe.Ingredients)
                {
                    var inventoryItem = await _context.InventoryItems
                        .FirstOrDefaultAsync(i => i.InventoryItemId == ingredient.InventoryItemId);

                    if (inventoryItem == null) continue;

                    // Calculate quantity to deduct
                    var qtyToDeduct = ingredient.QuantityPerYield * line.Quantity;
                    inventoryItem.Quantity -= qtyToDeduct;
                    inventoryItem.UpdatedAt = DateTime.UtcNow;

                    Console.WriteLine($"[INVENTORY] Deducted {qtyToDeduct} {inventoryItem.UnitOfMeasure} of {inventoryItem.Name} for order {order.OrderNumber}");
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[INVENTORY ERROR] {ex.Message}");
        }
    }

    [HttpPost("{id}/void")]
    public async Task<ActionResult> VoidOrder(int id, [FromBody] VoidOrderRequest request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CompanyId == companyId);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        if (order.OrderStatus == "Voided")
            return BadRequest(new { message = "Order is already voided" });

        var oldStatus = order.OrderStatus;
        order.OrderStatus = "Voided";
        order.VoidedAt = DateTime.UtcNow;
        order.VoidReason = request.Reason;
        order.VoidByUserId = userId;

        _context.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = id,
            OldStatus = oldStatus,
            NewStatus = "Voided",
            UserId = userId,
            Notes = request.Reason
        });

        await _context.SaveChangesAsync();

        return Ok(new { message = "Order voided" });
    }

    private async Task RecalculateOrderTotals(Order order)
    {
        await _context.Entry(order).Collection(o => o.OrderLines).LoadAsync();

        order.SubTotal = order.OrderLines.Sum(l => l.LineGross);
        order.TotalLineDiscount = order.OrderLines.Sum(l => l.DiscountAmount);

        var netAfterLineDiscount = order.SubTotal - order.TotalLineDiscount;
        order.BillDiscountAmount = order.BillDiscountPercent > 0 
            ? netAfterLineDiscount * order.BillDiscountPercent / 100 
            : 0;

        var netBeforeServiceAndTax = netAfterLineDiscount - order.BillDiscountAmount;
        order.ServiceChargeAmount = order.ServiceChargePercent > 0 
            ? netBeforeServiceAndTax * order.ServiceChargePercent / 100 
            : 0;

        var netBeforeTax = netBeforeServiceAndTax + order.ServiceChargeAmount + order.DeliveryFee;
        order.TaxAmount = order.TaxPercent > 0 
            ? netBeforeTax * order.TaxPercent / 100 
            : 0;

        order.GrandTotal = netBeforeTax + order.TaxAmount + order.TipsAmount;
        order.BalanceDue = order.GrandTotal - order.TotalPaid;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}

public class CreateOrderRequest
{
    public int BranchId { get; set; }
    public int? ShiftId { get; set; }
    public string OrderType { get; set; } = "DineIn";
    public int? TableId { get; set; }
    public int? CustomerId { get; set; }
    public string? Notes { get; set; }
}

public class AddLineRequest
{
    public int MenuItemId { get; set; }
    public int? MenuItemSizeId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal DiscountPercent { get; set; } = 0;
    public string? Notes { get; set; }
    public List<ModifierRequest>? Modifiers { get; set; }
}

public class ModifierRequest
{
    public int ModifierId { get; set; }
    public decimal Quantity { get; set; } = 1;
}

public class ApplyDiscountRequest
{
    public decimal DiscountPercent { get; set; }
}

public class PaymentRequest
{
    public List<PaymentLineRequest> Payments { get; set; } = new();
}

public class PaymentLineRequest
{
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Reference { get; set; }
    public int? GiftCardId { get; set; }
    public decimal? LoyaltyPointsUsed { get; set; }
}

public class VoidOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}
