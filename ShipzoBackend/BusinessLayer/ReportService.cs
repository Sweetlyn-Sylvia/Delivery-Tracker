using System.Data;
using Microsoft.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ShipzoBackend.BusinessLayer
{
    public class ReportService
    {
        private readonly Database db;

        public ReportService(Database database)
        {
            db = database;
        }

        public byte[] GetDailyReport()
        {
            DataTable table = db.ExecuteProcedure("sp_GetDailyReport", null);
            return GeneratePdf(table, "DAILY REPORT", "daily");
        }

        public byte[] GetDeliveredReport()
        {
            DataTable table = db.ExecuteProcedure("sp_GetDeliveredReport", null);
            return GeneratePdf(table, "DELIVERED REPORT", "delivered");
        }

        public byte[] GetPendingReport()
        {
            DataTable table = db.ExecuteProcedure("sp_GetPendingReport", null);
            return GeneratePdf(table, "PENDING REPORT", "pending");
        }

        public byte[] GetRangeReport(DateTime startDate, DateTime endDate)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            DataTable table = db.ExecuteProcedure("sp_GetRangeReport", parameters);

            return GeneratePdf(
                table,
                $"RANGE REPORT ({startDate:dd-MM-yyyy} to {endDate:dd-MM-yyyy})",
                "range-report"
            );
        }

        public byte[] GetAgentReport(string agentId)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@AgentId", agentId)
            };

            DataTable table = db.ExecuteProcedure("sp_GetAgentReport", parameters);

            return GeneratePdf(
                table,
                $"AGENT REPORT ({agentId})",
                "agent-report"
            );
        }

        private byte[] GeneratePdf(DataTable parcels, string reportTitle, string filenamePrefix)
        {
            int totalParcels = parcels.Rows.Count;

            int deliveredCount = parcels.AsEnumerable()
                .Count(r => r["Status"].ToString() == "Delivered");

            int inTransitCount = parcels.AsEnumerable()
                .Count(r => r["Status"].ToString() == "In Transit");

            int pickedUpCount = parcels.AsEnumerable()
                .Count(r => r["Status"].ToString() == "Picked Up");

            decimal totalDeliveryAmount = 0;

           
            if (parcels.Rows.Count > 0 && parcels.Columns.Contains("TotalDeliveryAmount"))
            {
                totalDeliveryAmount = Convert.ToDecimal(parcels.Rows[0]["TotalDeliveryAmount"]);
            }

            using MemoryStream memoryStream = new MemoryStream();

            Document document = new Document(PageSize.A4, 20, 20, 40, 40);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            writer.PageEvent = new PdfPageEvents();

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logo.jpg");

            if (File.Exists(logoPath))
            {
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(80, 80);
                logo.Alignment = Element.ALIGN_CENTER;
                document.Add(logo);
            }

            document.Add(new Paragraph("Shipzo Parcel Delivery Tracker", titleFont)
            { Alignment = Element.ALIGN_CENTER });

            document.Add(new Paragraph(reportTitle, headerFont)
            { Alignment = Element.ALIGN_CENTER });

            document.Add(new Paragraph($"Generated On: {DateTime.Now:dd-MM-yyyy hh:mm tt}\n\n", bodyFont));

           

            PdfPTable summaryTable = new PdfPTable(2)
            {
                WidthPercentage = 60,
                SpacingAfter = 15
            };

            summaryTable.AddCell("Summary");
            summaryTable.AddCell("Count");

            summaryTable.AddCell("Total Parcels");
            summaryTable.AddCell(totalParcels.ToString());

            summaryTable.AddCell("Delivered Parcels");
            summaryTable.AddCell(deliveredCount.ToString());

            summaryTable.AddCell("In Transit Parcels");
            summaryTable.AddCell(inTransitCount.ToString());

            summaryTable.AddCell("Picked Up Parcels");
            summaryTable.AddCell(pickedUpCount.ToString());

            document.Add(summaryTable);

          

            PdfPTable table = new PdfPTable(9) { WidthPercentage = 100 };

            table.AddCell("Parcel ID");
            table.AddCell("Sender");
            table.AddCell("Receiver");
            table.AddCell("Agent ID");
            table.AddCell("Status");
            table.AddCell("Contact");
            table.AddCell("Date");
            table.AddCell("Remarks");
            table.AddCell("Amount");

            foreach (DataRow row in parcels.Rows)
            {
                table.AddCell(row["ParcelId"].ToString());
                table.AddCell(row["SenderName"].ToString());
                table.AddCell(row["ReceiverName"].ToString());
                table.AddCell(row["AgentId"].ToString());
                table.AddCell(row["Status"].ToString());
                table.AddCell(row["ReceiverContactNumber"].ToString());
                table.AddCell(Convert.ToDateTime(row["Date"]).ToString("dd-MM-yyyy"));
                table.AddCell(row["Remarks"].ToString());
                table.AddCell(row["DeliveryAmount"].ToString());
            }

            document.Add(table);

            // Total Delivery Amount (Range Report)

            if (totalDeliveryAmount > 0)
            {
                document.Add(new Paragraph(
                    $"\nTotal Delivery Amount : Rs {totalDeliveryAmount}",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
            }

            document.Close();

            return memoryStream.ToArray();
        }

        public class PdfPageEvents : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                PdfPTable footer = new PdfPTable(1)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin
                };

                footer.DefaultCell.Border = 0;

                footer.AddCell(new Phrase(
                    $"Page {writer.PageNumber}",
                    FontFactory.GetFont(FontFactory.HELVETICA, 8)));

                footer.WriteSelectedRows(
                    0,
                    -1,
                    document.LeftMargin,
                    document.BottomMargin - 5,
                    writer.DirectContent);
            }
        }
    }
}