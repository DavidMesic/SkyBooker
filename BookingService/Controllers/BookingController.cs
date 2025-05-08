using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using BookingService.Services;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Booking>> Get() => Ok(_bookingService.GetAll());

        [HttpGet("{id}")]
        public ActionResult<Booking> Get(int id)
        {
            var booking = _bookingService.Get(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public ActionResult<Booking> Create(Booking booking)
        {
            var created = _bookingService.Create(booking);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}