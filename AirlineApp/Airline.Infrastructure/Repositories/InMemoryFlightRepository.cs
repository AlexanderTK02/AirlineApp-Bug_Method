using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Repositories;

public class InMemoryFlightRepository : IFlightRepository
{
    private readonly List<Flight> _flights = new();

    public Flight? GetById(int id) => _flights.FirstOrDefault(f => f.Id == id);

    public List<Flight> GetAll() => _flights.ToList();

    public List<Flight> GetByStatus(FlightStatus status) =>
        _flights.Where(f => f.Status == status).ToList();

    // BUG_TARGET: GetByRoute
    public List<Flight> GetByRoute(string departureAirport, string arrivalAirport) =>
        _flights.Where(f => f.DepartureAirport == departureAirport && f.ArrivalAirport == arrivalAirport).ToList();

    // BUG_TARGET: GetByDateRange
    public List<Flight> GetByDateRange(DateTime from, DateTime to) =>
        _flights.Where(f => f.DepartureTime >= from && f.DepartureTime <= to).ToList();

    public void Add(Flight flight) => _flights.Add(flight);

    public void Update(Flight flight)
    {
        var index = _flights.FindIndex(f => f.Id == flight.Id);
        if (index >= 0) _flights[index] = flight;
    }
}
