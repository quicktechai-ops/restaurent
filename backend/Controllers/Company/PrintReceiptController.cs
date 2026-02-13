using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Restaurant.API.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/print-receipt")]
[Authorize(Roles = "CompanyAdmin,Manager,Cashier")]
public class PrintReceiptController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrintReceiptController(AppDbContext context)
    {
        _context = context;
    }

    public class ReceiptLineDto
    {
        public string Name { get; set; } = "";
        public string? SizeName { get; set; }
        public int Quantity { get; set; }
        public decimal EffectivePrice { get; set; }
        public decimal LineNet { get; set; }
        public decimal DiscountPercent { get; set; }
        public List<ModifierDetailDto> Modifiers { get; set; } = new();
        public string? Notes { get; set; }
    }

    public class ModifierDetailDto
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class PrintReceiptRequest
    {
        public string OrderNumber { get; set; } = "";
        public string OrderType { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string? TableName { get; set; }
        public string? CustomerName { get; set; }
        public List<ReceiptLineDto> Lines { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TotalLineDiscount { get; set; }
        public decimal BillDiscountPercent { get; set; }
        public decimal BillDiscountAmount { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal VatPercent { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string CompanyName { get; set; } = "";
    }

    [HttpPost]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public IActionResult Print([FromBody] PrintReceiptRequest request)
    {
        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return StatusCode(500, new { message = "Direct printing is only supported on Windows." });

            var printDoc = new PrintDocument();
            var defaultName = new PrinterSettings().PrinterName;
            printDoc.PrinterSettings.PrinterName = defaultName;

            if (!printDoc.PrinterSettings.IsValid)
                return StatusCode(500, new { message = $"Printer '{defaultName}' is not valid." });

            // 80mm ≈ 302 pixels at 96 DPI. Use 10000 height for continuous feed.
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Receipt", 302, 10000);
            printDoc.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            printDoc.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                float W = e.MarginBounds.Width;  // usable width
                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                // Fonts
                var fCompany  = new Font("Segoe UI", 13f, FontStyle.Bold);
                var fBranch   = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                var fMeta     = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                var fMetaBold = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                var fColHead  = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                var fItemName = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fItemVal  = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                var fDetail   = new Font("Segoe UI", 7f, FontStyle.Regular);
                var fTotals   = new Font("Segoe UI", 9f, FontStyle.Regular);
                var fGrand    = new Font("Segoe UI", 13f, FontStyle.Bold);
                var fFooter   = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                var fFooterSm = new Font("Segoe UI", 7.5f, FontStyle.Regular);

                var sfL = new StringFormat { Alignment = StringAlignment.Near };
                var sfR = new StringFormat { Alignment = StringAlignment.Far };
                var sfC = new StringFormat { Alignment = StringAlignment.Center };

                // --- Company Name ---
                g.DrawString(request.CompanyName, fCompany, Brushes.Black,
                    new RectangleF(x, y, W, 28), sfC);
                y += 28;

                // --- Branch ---
                g.DrawString(request.BranchName, fBranch, Brushes.Gray,
                    new RectangleF(x, y, W, 16), sfC);
                y += 20;

                // --- Dashed divider ---
                y = DrawDashedLine(g, x, y, W);

                // --- Meta rows ---
                y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Order #", "#" + request.OrderNumber);
                y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Date", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
                y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Type", request.OrderType);
                if (!string.IsNullOrEmpty(request.TableName))
                    y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Table", request.TableName);
                if (!string.IsNullOrEmpty(request.CustomerName))
                    y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Customer", request.CustomerName);
                y = DrawMetaRow(g, x, y, W, fMeta, fMetaBold, "Payment", request.PaymentMethod);

                y = DrawDashedLine(g, x, y, W);

                // --- Column headers ---
                float colQty = 30, colPrice = 48, colTotal = 52;
                float colName = W - colQty - colPrice - colTotal;

                g.DrawString("ITEM", fColHead, Brushes.Black, new RectangleF(x, y, colName, 14), sfL);
                g.DrawString("QTY", fColHead, Brushes.Black, new RectangleF(x + colName, y, colQty, 14), sfC);
                g.DrawString("PRICE", fColHead, Brushes.Black, new RectangleF(x + colName + colQty, y, colPrice, 14), sfR);
                g.DrawString("TOTAL", fColHead, Brushes.Black, new RectangleF(x + colName + colQty + colPrice, y, colTotal, 14), sfR);
                y += 16;

                // Thin solid line
                g.DrawLine(Pens.LightGray, x, y, x + W, y);
                y += 4;

                // --- Items ---
                foreach (var item in request.Lines)
                {
                    var name = item.Name;
                    if (!string.IsNullOrEmpty(item.SizeName))
                        name += $" ({item.SizeName})";

                    // Measure name height for wrapping
                    var nameSize = g.MeasureString(name, fItemName, (int)colName);
                    float rowH = Math.Max(nameSize.Height, 16);

                    g.DrawString(name, fItemName, Brushes.Black, new RectangleF(x, y, colName, rowH), sfL);
                    g.DrawString(item.Quantity.ToString(), fItemVal, Brushes.Black, new RectangleF(x + colName, y, colQty, 16), sfC);
                    g.DrawString(Money(item.EffectivePrice), fItemVal, Brushes.Black, new RectangleF(x + colName + colQty, y, colPrice, 16), sfR);
                    g.DrawString(Money(item.LineNet), fMetaBold, Brushes.Black, new RectangleF(x + colName + colQty + colPrice, y, colTotal, 16), sfR);
                    y += rowH + 2;

                    // Modifiers
                    if (item.Modifiers.Any())
                    {
                        var modText = "  + " + string.Join(", ", item.Modifiers.Select(m =>
                            m.Quantity > 1 ? $"{m.Name} x{m.Quantity}" : m.Name));
                        g.DrawString(modText, fDetail, Brushes.Gray, x + 6, y);
                        y += 12;
                    }

                    // Notes
                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        g.DrawString("  " + item.Notes, fDetail, Brushes.Gray, x + 6, y);
                        y += 12;
                    }

                    // Line discount
                    if (item.DiscountPercent > 0)
                    {
                        g.DrawString($"  Discount: -{item.DiscountPercent}%", fDetail, Brushes.Red, x + 6, y);
                        y += 12;
                    }

                    // Dotted separator between items
                    using var dotPen = new Pen(Color.LightGray, 0.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                    g.DrawLine(dotPen, x, y + 1, x + W, y + 1);
                    y += 4;
                }

                y = DrawDashedLine(g, x, y, W);

                // --- Totals ---
                y = DrawTotalRow(g, x, y, W, fTotals, fTotals, "Subtotal", Money(request.Subtotal), false);

                if (request.TotalLineDiscount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotals, fTotals, "Line Discounts", "-" + Money(request.TotalLineDiscount), true);

                if (request.BillDiscountPercent > 0)
                    y = DrawTotalRow(g, x, y, W, fTotals, fTotals, $"Bill Discount ({request.BillDiscountPercent}%)", "-" + Money(request.BillDiscountAmount), true);

                if (request.ServiceChargeAmount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotals, fTotals, $"Service ({request.ServiceChargePercent}%)", Money(request.ServiceChargeAmount), false);

                if (request.VatAmount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotals, fTotals, $"VAT ({request.VatPercent}%)", Money(request.VatAmount), false);

                // --- Grand Total with double lines ---
                y += 4;
                g.DrawLine(new Pen(Color.Black, 1.5f), x, y, x + W, y);
                y += 3;
                g.DrawLine(new Pen(Color.Black, 0.5f), x, y, x + W, y);
                y += 8;

                g.DrawString("TOTAL", fGrand, Brushes.Black, new RectangleF(x, y, W / 2, 26), sfL);
                g.DrawString(Money(request.GrandTotal), fGrand, Brushes.Black, new RectangleF(x + W / 2, y, W / 2, 26), sfR);
                y += 26;

                g.DrawLine(new Pen(Color.Black, 0.5f), x, y, x + W, y);
                y += 3;
                g.DrawLine(new Pen(Color.Black, 1.5f), x, y, x + W, y);
                y += 14;

                // --- Footer ---
                g.DrawString("Thank you for your visit!", fFooter, Brushes.Black,
                    new RectangleF(x, y, W, 16), sfC);
                y += 18;
                g.DrawString("We look forward to seeing you again.", fFooterSm, Brushes.Gray,
                    new RectangleF(x, y, W, 14), sfC);

                e.HasMorePages = false;
            };

            printDoc.Print();
            return Ok(new { message = "Receipt printed successfully", printer = defaultName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Print failed: {ex.Message}", detail = ex.ToString() });
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawDashedLine(Graphics g, float x, float y, float w)
    {
        y += 6;
        using var pen = new Pen(Color.Black, 0.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        g.DrawLine(pen, x, y, x + w, y);
        return y + 8;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawMetaRow(Graphics g, float x, float y, float w, Font fLabel, Font fValue, string label, string value)
    {
        g.DrawString(label, fLabel, Brushes.Black, x, y);
        g.DrawString(value, fValue, Brushes.Black, new RectangleF(x, y, w, 16), new StringFormat { Alignment = StringAlignment.Far });
        return y + 18;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawTotalRow(Graphics g, float x, float y, float w, Font fLabel, Font fValue, string label, string value, bool isRed)
    {
        var brush = isRed ? Brushes.Red : Brushes.Black;
        g.DrawString(label, fLabel, brush, x, y);
        g.DrawString(value, fValue, brush, new RectangleF(x, y, w, 18), new StringFormat { Alignment = StringAlignment.Far });
        return y + 20;
    }

    private static string Money(decimal n) => $"${n:N2}";
}
