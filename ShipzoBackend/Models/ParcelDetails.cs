namespace ShipzoBackend.Models
{
    public class ParcelDetails
    {
        public string ParcelId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverContactNumber { get; set; }
        public decimal? Weight { get; set; }
        public string Status { get; set; }
        public string AgentId { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
        public double DeliveryAmount { get; set; }
        public string DeliveryType { get; set; }
    }
}
