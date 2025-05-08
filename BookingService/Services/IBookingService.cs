using BookingService.Models;

namespace BookingService.Services
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAll();
        Booking? Get(int id);
        Booking Create(Booking newBooking);
    }
}