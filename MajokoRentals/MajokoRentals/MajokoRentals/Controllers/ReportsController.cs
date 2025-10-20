using Microsoft.AspNetCore.Mvc;
using MajokoRentals.Models;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace MajokoRentals.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IConfiguration _config;

        public ReportsController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Reports()
        {
            var report = GetDashboardData();
            return View(report);
        }

        [HttpGet]
        public IActionResult DownloadReport()
        {
            var report = GetDashboardData();

            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var blackColor = new BaseColor(0, 0, 0);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, blackColor);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, blackColor);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, blackColor);

                doc.Add(new Paragraph("📊 MajokoRentals – Reports & Analysis", titleFont));
                doc.Add(new Paragraph($"Generated on: {DateTime.Now}", smallFont));
                doc.Add(new Paragraph("\n"));

                PdfPTable summaryTable = new PdfPTable(2) { WidthPercentage = 100 };

                void AddRow(string label, string value)
                {
                    summaryTable.AddCell(new PdfPCell(new Phrase(label, normalFont)) { BackgroundColor = new BaseColor(240, 240, 240) });
                    summaryTable.AddCell(new PdfPCell(new Phrase(value, normalFont)));
                }

                AddRow("Total Revenue", $"R {report.TotalRevenue:N2}");
                AddRow("Total Arrears", $"R {report.TotalArrears:N2}");
                AddRow("Total Properties", report.TotalProperties.ToString());
                AddRow("Occupied Units", report.OccupiedUnits.ToString());
                AddRow("Maintenance (Pending)", report.MaintenancePending.ToString());
                AddRow("Maintenance (Resolved)", report.MaintenanceResolved.ToString());

                doc.Add(summaryTable);
                doc.Add(new Paragraph("\n"));

                doc.Add(new Paragraph("This report summarizes financial, occupancy, and maintenance performance metrics for the MajokoRentals system.", normalFont));
                doc.Add(new Paragraph("\n\nSignature: ________________________", normalFont));

                doc.Close();

                return File(stream.ToArray(), "application/pdf", $"MajokoRentals_Report_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        private DashboardReport GetDashboardData()
        {
            var report = new DashboardReport();
            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();

                // ✅ Safe queries with TRY/CATCH
                report.TotalRevenue = ExecuteScalarSafe(conn, "SELECT ISNULL(SUM(Amount), 0) FROM Payments");
                report.TotalArrears = ExecuteScalarSafe(conn, "SELECT ISNULL(SUM(AmountDue), 0) FROM Rent WHERE Status='Unpaid'");
                report.TotalProperties = ExecuteScalarSafe(conn, "SELECT COUNT(*) FROM Listings");
                report.OccupiedUnits = ExecuteScalarSafe(conn, "SELECT COUNT(*) FROM Listings WHERE Status='Occupied'");
                report.MaintenancePending = ExecuteScalarSafe(conn, "SELECT COUNT(*) FROM MaintenanceRequests WHERE Status='Pending'");
                report.MaintenanceResolved = ExecuteScalarSafe(conn, "SELECT COUNT(*) FROM MaintenanceRequests WHERE Status='Resolved' OR Status='Complete'");

            }

            return report;
        }

        // ✅ Helper: safely return integer from SQL, default 0 if table/column missing
        private int ExecuteScalarSafe(SqlConnection conn, string sql)
        {
            try
            {
                using (SqlCommand cmd = new(sql, conn))
                {
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
