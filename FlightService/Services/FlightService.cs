using MongoDB.Driver;
using FlightService.Models;

namespace FlightService.Services
{
    public class FlightManager
    {
        private readonly IMongoCollection<Flight> _flights;

        public FlightManager(IFlightDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _flights = database.GetCollection<Flight>(settings.FlightsCollectionName);
        }

        public List<Flight> Get() => _flights.Find(f => true).ToList();

        public Flight Get(string id) => _flights.Find<Flight>(f => f.Id == id).FirstOrDefault();

        public Flight Create(Flight newFlight)
        {
            newFlight.CreatedAt = DateTime.UtcNow;
            newFlight.UpdatedAt = DateTime.UtcNow;
            _flights.InsertOne(newFlight);
            return newFlight;
        }

        // Update und Delete könnten ergänzt werden, sind aber Level 1 nicht erforderlich.
    }
}