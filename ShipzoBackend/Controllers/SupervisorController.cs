using Microsoft.AspNetCore.Mvc;
using ShipzoBackend.Models;
using ShipzoBackend.Services;

namespace ShipzoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupervisorController : ControllerBase
    {
        private readonly SupervisorService service;

        public SupervisorController(SupervisorService supervisorService)
        {
            service = supervisorService;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] SupervisorLogin login)
        {
            var result = service.Login(login.SupervisorId, login.Password);


            return Ok(new
            {
                Message = result
            });
        }


        [HttpGet("dashboard-summary")]
        public IActionResult DashboardSummary()
        {
            return Ok(service.GetDashboardSummary());
        }

       
        [HttpGet("{supervisorId}")]
        public IActionResult GetProfile(string supervisorId)
        {
            var result = service.GetSupervisorById(supervisorId);

            if (result == null)
                return NotFound("Supervisor not found");

            return Ok(result);
        }

     
        [HttpPut("update-profile")]
        public IActionResult UpdateProfile([FromBody] ChangePassword dto)
        {
            string result = service.UpdateSupervisorProfile(dto);
            return Ok(new { Message = result });
        }

        [HttpGet("notifications")]
        public IActionResult GetNotifications()
        {
            return Ok(service.GetNotifications());
        }

        [HttpPost("assign-agent")]
        public IActionResult AssignAgent([FromBody] AssignAgent request)
        {
            string result = service.AssignAgent(request);
            return Ok(new { Message = result });
        }
        [HttpPost("send-reminder")]
        public IActionResult SendReminder([FromBody] AssignAgent request)
        {
            string result = service.SendReminder(request);

            return Ok(new { message = result });
        }
    }
}