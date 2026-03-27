namespace ShipzoBackend.Models
{
    public class ReportCreation
    {
        public string ParcelId { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public string AgentId { get; set; }

        public string Status { get; set; }

        public string ReceiverContactNumber { get; set; }

        public DateTime Date { get; set; }

        public string Remarks { get; set; }

        public decimal DeliveryAmount { get; set; }
    }
}
