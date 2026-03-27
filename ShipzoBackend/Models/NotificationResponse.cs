namespace ShipzoBackend.Models
{
    public class NotificationResponse
    {
        public string ParcelId { get; set; }

        public string Status { get; set; }

        public string? AgentID { get; set; }

        public string AgentName { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }
    }
}
