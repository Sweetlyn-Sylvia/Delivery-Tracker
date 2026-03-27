namespace ShipzoBackend.Models
{
    public class DashboardSummary
    {
        public int TotalParcels { get; set; }
        public int DeliveredParcels { get; set; }
        public int InTransitParcels { get; set; }
        public int PickedUpParccels { get; set; }
        public int TotalAgents { get; set; }
        public decimal TodaysEarning { get; set; }

    }
}
