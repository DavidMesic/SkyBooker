namespace BookingService.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string FlightId { get; set; } = null!;
        public int PassengerId { get; set; }
        public string PassengerFirstname { get; set; } = null!;
        public string PassengerLastname { get; set; } = null!;
        public int TicketCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}