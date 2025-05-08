namespace FlightService.Models
{
    public class FlightDatabaseSettings : IFlightDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string FlightsCollectionName { get; set; } = null!;
    }
    public interface IFlightDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string FlightsCollectionName { get; set; }
    }
}