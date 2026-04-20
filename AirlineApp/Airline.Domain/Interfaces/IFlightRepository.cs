using Airline.Domain.Entities;
using Airline.Domain.Enums;

namespace Airline.Domain.Interfaces;

public interface IFlightRepository
{
    Flight? GetById(int id);
    List<Flight> GetAll();
    List<Flight> GetByStatus(FlightStatus status);
    List<Flight> GetByRoute(string departure, string arrival);
    List<Flight> GetByDateRange(DateTime from, DateTime to);
    void Add(Flight flight);
    void Update(Flight flight);
}
