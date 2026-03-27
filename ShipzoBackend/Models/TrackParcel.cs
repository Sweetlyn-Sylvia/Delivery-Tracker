namespace ShipzoBackend.Models
{
    public class TrackParcel
    {
        public string ParcelId { get; set; }
        public string Status { get; set; }
        public string ReceiverAddress { get; set; }

        public double? CurrentLat { get; set; }
        public double? CurrentLng { get; set; }

        public string? CurrentLocationText { get; set; }

        public DateTime? LocationUpdatedAt { get; set; }
    }
}
