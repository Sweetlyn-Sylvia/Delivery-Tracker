using System.ComponentModel.DataAnnotations;

namespace ShipzoBackend.Models
{
    public class Parcel
    {
       
        public int Id { get; set; }

        public string? ParcelId { get; set; }

      
        public string? SenderName { get; set; }

        
        public string? ReceiverName { get; set; }

        
        public string? SenderAddress { get; set; }

        
        public string? ReceiverAddress { get; set; }

        
        public string? ReceiverContactNumber { get; set; }


        
        public decimal? Weight { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? Status { get; set; } = "Picked Up";

        public string? AgentId { get; set; }

        public DateTime? DeliveryTime { get; set; }
        public string? Remarks { get; set; }
        public double DeliveryAmount { get; set; }
        public bool FastDelivery { get; set; }
        public string PaymentMode { get; set; }
        public bool IsPaid { get; set; }
        public double? CurrentLat { get; set; }
        public double? CurrentLng { get; set; }
        public DateTime? LocationUpdatedAt { get; set; }
        public string? CurrentLocationText { get; set; }

    }
}
