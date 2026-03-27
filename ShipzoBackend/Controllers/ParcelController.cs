using Microsoft.AspNetCore.Mvc;
using ShipzoBackend.BusinessLayer;
using ShipzoBackend.Models;

namespace ShipzoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParcelController : ControllerBase
    {
        private readonly ParcelService parcelService;

        public ParcelController(ParcelService service)
        {
            parcelService = service;
        }


        [HttpPost("create")]
        public IActionResult CreateParcel([FromBody] Parcel parcel)
        {
            var result = parcelService.CreateParcel(parcel);

            var message = result.GetType()
                                .GetProperty("Message")?
                                .GetValue(result)?.ToString();

            if (message != "Parcel created successfully")
            {
                return BadRequest(message);
            }

            return Ok(result);
        }


        [HttpGet("all")]
        public IActionResult GetAllParcels()
        {
            List<ParcelDetails> parcels = parcelService.GetAllParcels();
            return Ok(parcels);
        }

        [HttpGet("{parcelId}")]
        public IActionResult GetParcelById(string parcelId)
        {
            ParcelDetails parcel = parcelService.GetParcelById(parcelId);

            if (parcel == null)
                return NotFound("Parcel not found");

            return Ok(parcel);
        }

 
        public class StatusUpdateRequest
        {
            public string Status { get; set; }
            public string Remarks { get; set; }
        }

       
        [HttpPut("update-status/{parcelId}")]
        public IActionResult UpdateParcelStatus(string parcelId, [FromBody] StatusUpdateRequest request)
        {
            string result = parcelService.UpdateParcelStatus(
                parcelId,
                request.Status,
                request.Remarks
            );

            return Ok(new
            {
                Message = result
            });
        }

     
        [HttpGet("pending")]
        public IActionResult GetPendingParcels()
        {
            List<ParcelDetails> parcels = parcelService.GetPendingParcels();
            return Ok(parcels);
        }

   
        [HttpGet("filter")]
        public IActionResult FilterParcels(string? status, string? agentId, DateTime? date)
        {
            List<ParcelDetails> parcels = parcelService.FilterParcels(status, agentId, date);
            return Ok(parcels);
        }

        public class LocationUpdateRequest
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
            public string? LocationText { get; set; }
        }

       
        [HttpPut("update-location/{parcelId}")]
        public IActionResult UpdateParcelLocation(string parcelId, [FromBody] LocationUpdateRequest request)
        {
            string result = parcelService.UpdateParcelLocation(
                parcelId,
                request.Lat,
                request.Lng,
                request.LocationText
            );

            return Ok(new
            {
                Message = result
            });
        }

        
        [HttpGet("track/{parcelId}")]
        public IActionResult TrackParcel(string parcelId)
        {
            TrackParcel parcel = parcelService.TrackParcel(parcelId);

            if (parcel == null)
                return NotFound("Parcel not found");

            return Ok(parcel);
        }
        [HttpGet("parcel-agent/{agentId}")]
        public IActionResult GetParcelsByAgent(string agentId)
        {
            var parcels = parcelService.GetParcelsByAgentId(agentId);
            return Ok(parcels);
        }
    }
}