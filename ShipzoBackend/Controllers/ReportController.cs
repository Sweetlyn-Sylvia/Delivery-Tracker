using Microsoft.AspNetCore.Mvc;
using ShipzoBackend.BusinessLayer;

namespace ShipzoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService service;

        public ReportsController(ReportService reportService)
        {
            service = reportService;
        }

        [HttpGet("daily")]
        public IActionResult DailyReport()
        {
            var pdf = service.GetDailyReport();
            return File(pdf, "application/pdf", "daily-report.pdf");
        }

        [HttpGet("delivered")]
        public IActionResult DeliveredReport()
        {
            var pdf = service.GetDeliveredReport();
            return File(pdf, "application/pdf", "delivered-report.pdf");
        }

        [HttpGet("pending")]
        public IActionResult PendingReport()
        {
            var pdf = service.GetPendingReport();
            return File(pdf, "application/pdf", "pending-report.pdf");
        }

        [HttpGet("range")]
        public IActionResult RangeReport(DateTime startDate, DateTime endDate)
        {
            var pdf = service.GetRangeReport(startDate, endDate);
            return File(pdf, "application/pdf", "range-report.pdf");
        }

        [HttpGet("agent")]
        public IActionResult AgentReport(string agentId)
        {
            var pdf = service.GetAgentReport(agentId);
            return File(pdf, "application/pdf", "agent-report.pdf");
        }
    }
}