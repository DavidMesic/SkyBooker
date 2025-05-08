using BookingService.Data;
using BookingService.Models;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingContext _context;

        public BookingService(BookingContext context)
        {
            _context = context;
        }

        public IEnumerable<Booking> GetAll() => _context.Bookings.ToList();

        public Booking? Get(int id) => _context.Bookings.Find(id);

        public Booking Create(Booking newBooking)
        {
            newBooking.CreatedAt = DateTime.UtcNow;
            newBooking.UpdatedAt = DateTime.UtcNow;
            _context.Bookings.Add(newBooking);
            _context.SaveChanges();
            return newBooking;
        }
    }
}