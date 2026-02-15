using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Restaurant.API.Controllers.Company;

[ApiController]
[Route("api/company/print-receipt")]
[Authorize(Roles = "CompanyAdmin,Manager,Cashier")]
public class PrintReceiptController : ControllerBase
{
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
        public DateTime? DateTime { get; set; }
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

            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Receipt", 283, 10000);
            printDoc.DefaultPageSettings.Margins = new Margins(8, 8, 8, 8);

            printDoc.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                float W = e.MarginBounds.Width;
                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                var fTitle = new Font("Segoe UI", 14f, FontStyle.Bold);
                var fBranch = new Font("Segoe UI", 9f, FontStyle.Regular);
                var fLabel = new Font("Segoe UI", 9f, FontStyle.Regular);
                var fValue = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fColHead = new Font("Segoe UI", 8f, FontStyle.Bold);
                var fItem = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fItemDetail = new Font("Segoe UI", 8f, FontStyle.Regular);
                var fSmall = new Font("Segoe UI", 7.5f, FontStyle.Regular);
                var fTotal = new Font("Segoe UI", 9f, FontStyle.Regular);
                var fGrand = new Font("Segoe UI", 14f, FontStyle.Bold);
                var fFooter = new Font("Segoe UI", 9f, FontStyle.Bold);
                var fFooterSm = new Font("Segoe UI", 8f, FontStyle.Regular);

                var sfL = new StringFormat { Alignment = StringAlignment.Near };
                var sfR = new StringFormat { Alignment = StringAlignment.Far };
                var sfC = new StringFormat { Alignment = StringAlignment.Center };

                // Company Name
                g.DrawString(request.CompanyName, fTitle, Brushes.Black, new RectangleF(x, y, W, 22), sfC);
                y += 22;

                // Branch
                g.DrawString(request.BranchName, fBranch, Brushes.Gray, new RectangleF(x, y, W, 16), sfC);
                y += 18;

                // Dashed line
                y = DrawDash(g, x, y, W);

                // Meta info
                y = DrawRow(g, x, y, W, fLabel, fValue, "Order #", "#" + request.OrderNumber);
                y = DrawRow(g, x, y, W, fLabel, fValue, "Date", (request.DateTime ?? DateTime.Now).ToString("MM/dd/yyyy hh:mm tt"));
                y = DrawRow(g, x, y, W, fLabel, fValue, "Type", request.OrderType);
                if (!string.IsNullOrEmpty(request.TableName))
                    y = DrawRow(g, x, y, W, fLabel, fValue, "Table", request.TableName);
                if (!string.IsNullOrEmpty(request.CustomerName))
                    y = DrawRow(g, x, y, W, fLabel, fValue, "Customer", request.CustomerName);
                y = DrawRow(g, x, y, W, fLabel, fValue, "Payment", request.PaymentMethod);

                y = DrawDash(g, x, y, W);

                // Column headers
                float cName = W - 120, cQty = 30, cPrice = 45, cTotal = 45;
                g.DrawString("ITEM", fColHead, Brushes.Black, x, y);
                g.DrawString("QTY", fColHead, Brushes.Black, new RectangleF(x + cName, y, cQty, 14), sfC);
                g.DrawString("PRICE", fColHead, Brushes.Black, new RectangleF(x + cName + cQty, y, cPrice, 14), sfR);
                g.DrawString("TOTAL", fColHead, Brushes.Black, new RectangleF(x + cName + cQty + cPrice, y, cTotal, 14), sfR);
                y += 16;

                using var linePen = new Pen(Color.LightGray, 0.5f);
                g.DrawLine(linePen, x, y, x + W, y);
                y += 4;

                // Items
                foreach (var item in request.Lines)
                {
                    var name = item.Name;
                    if (!string.IsNullOrEmpty(item.SizeName)) name += $" ({item.SizeName})";

                    var nameSize = g.MeasureString(name, fItem, (int)cName);
                    float rowH = Math.Max(nameSize.Height, 16);

                    g.DrawString(name, fItem, Brushes.Black, new RectangleF(x, y, cName, rowH), sfL);
                    g.DrawString(item.Quantity.ToString(), fItemDetail, Brushes.Black, new RectangleF(x + cName, y, cQty, 16), sfC);
                    g.DrawString(Money(item.EffectivePrice), fItemDetail, Brushes.Black, new RectangleF(x + cName + cQty, y, cPrice, 16), sfR);
                    g.DrawString(Money(item.LineNet), fValue, Brushes.Black, new RectangleF(x + cName + cQty + cPrice, y, cTotal, 16), sfR);
                    y += rowH + 2;

                    if (item.Modifiers != null && item.Modifiers.Any())
                    {
                        var modText = "+ " + string.Join(", ", item.Modifiers.Select(m => m.Quantity > 1 ? $"{m.Name} x{m.Quantity}" : m.Name));
                        g.DrawString(modText, fSmall, Brushes.Gray, x + 8, y);
                        y += 12;
                    }

                    if (!string.IsNullOrEmpty(item.Notes))
                    {
                        g.DrawString(item.Notes, fSmall, Brushes.Gray, x + 8, y);
                        y += 12;
                    }

                    if (item.DiscountPercent > 0)
                    {
                        g.DrawString($"Discount: -{item.DiscountPercent}%", fSmall, Brushes.Red, x + 8, y);
                        y += 12;
                    }

                    using var dotPen = new Pen(Color.LightGray, 0.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                    g.DrawLine(dotPen, x, y + 2, x + W, y + 2);
                    y += 6;
                }

                y = DrawDash(g, x, y, W);

                // Totals
                y = DrawTotalRow(g, x, y, W, fTotal, "Subtotal", Money(request.Subtotal), false);
                if (request.TotalLineDiscount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotal, "Line Discounts", "-" + Money(request.TotalLineDiscount), true);
                if (request.BillDiscountPercent > 0)
                    y = DrawTotalRow(g, x, y, W, fTotal, $"Bill Discount ({request.BillDiscountPercent}%)", "-" + Money(request.BillDiscountAmount), true);
                if (request.ServiceChargeAmount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotal, $"Service ({request.ServiceChargePercent}%)", Money(request.ServiceChargeAmount), false);
                if (request.VatAmount > 0)
                    y = DrawTotalRow(g, x, y, W, fTotal, $"VAT ({request.VatPercent}%)", Money(request.VatAmount), false);

                // Grand total with double border
                y += 4;
                using var thickPen = new Pen(Color.Black, 1.5f);
                using var thinPen = new Pen(Color.Black, 0.5f);
                g.DrawLine(thickPen, x, y, x + W, y);
                y += 3;
                g.DrawLine(thinPen, x, y, x + W, y);
                y += 8;

                g.DrawString("TOTAL", fGrand, Brushes.Black, new RectangleF(x, y, W / 2, 24), sfL);
                g.DrawString(Money(request.GrandTotal), fGrand, Brushes.Black, new RectangleF(x + W / 2, y, W / 2, 24), sfR);
                y += 24;

                g.DrawLine(thinPen, x, y, x + W, y);
                y += 3;
                g.DrawLine(thickPen, x, y, x + W, y);
                y += 14;

                // Footer
                g.DrawString("Thank you for your visit!", fFooter, Brushes.Black, new RectangleF(x, y, W, 16), sfC);
                y += 16;
                g.DrawString("Please come again", fFooterSm, Brushes.Gray, new RectangleF(x, y, W, 14), sfC);

                e.HasMorePages = false;
            };

            printDoc.Print();
            return Ok(new { message = "Receipt printed", printer = defaultName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Print failed: {ex.Message}" });
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawDash(Graphics g, float x, float y, float w)
    {
        y += 6;
        using var pen = new Pen(Color.Black, 0.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        g.DrawLine(pen, x, y, x + w, y);
        return y + 8;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawRow(Graphics g, float x, float y, float w, Font fL, Font fV, string label, string value)
    {
        g.DrawString(label, fL, Brushes.Black, x, y);
        g.DrawString(value, fV, Brushes.Black, new RectangleF(x, y, w, 16), new StringFormat { Alignment = StringAlignment.Far });
        return y + 16;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static float DrawTotalRow(Graphics g, float x, float y, float w, Font f, string label, string value, bool red)
    {
        var brush = red ? Brushes.Red : Brushes.Black;
        g.DrawString(label, f, brush, x, y);
        g.DrawString(value, f, brush, new RectangleF(x, y, w, 18), new StringFormat { Alignment = StringAlignment.Far });
        return y + 18;
    }

    private static string Money(decimal n) => $"${n:N2}";
}
