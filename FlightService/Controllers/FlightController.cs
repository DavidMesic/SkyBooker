using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightService.Models;
using FlightService.Services;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlightController : ControllerBase
    {
        private readonly FlightManager _flightService;

        public FlightController(FlightManager flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        public ActionResult<List<Flight>> Get() => _flightService.Get();

        [HttpGet("{id}")]
        public ActionResult<Flight> Get(string id)
        {
            var flight = _flightService.Get(id);
            if (flight == null)
            {
                return NotFound();
            }
            return flight;
        }

        [HttpPost]
        public ActionResult<Flight> Create(Flight flight)
        {
            _flightService.Create(flight);
            return CreatedAtAction(nameof(Get), new { id = flight.Id }, flight);
        }
    }
}