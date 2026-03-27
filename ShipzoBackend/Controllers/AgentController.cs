using Microsoft.AspNetCore.Mvc;
using ShipzoBackend.BusinessLayer;
using ShipzoBackend.Models;

namespace ShipzoBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly AgentService service;

        public AgentController(AgentService agentService)
        {
            service = agentService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterAgent request)
        {
            Agent agent = new Agent
            {
                AgentName = request.AgentName,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password
            };
            var result = service.RegisterAgent(agent);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AgentLogin request)
        {
            var result = service.LoginAgent(request.Email, request.Password);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAllAgents());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            return Ok(service.GetAgentById(id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)

        {
            string result = service.DeleteAgent(id);

            return Ok(new { message = result });
        }
        [HttpGet("performance")]
        public IActionResult GetAgentPerformance()
        {
            return Ok(service.GetAgentPerformance());
        }
        [HttpGet("dashboard/{agentId}")]
        public IActionResult GetDashboardData(string agentId)
        {
            return Ok(service.GetDashboardData(agentId));
        }
    }
}