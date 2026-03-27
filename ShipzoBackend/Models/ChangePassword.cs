namespace ShipzoBackend.Models
{
    public class ChangePassword
    {
        public string SupervisorId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

    }
}
