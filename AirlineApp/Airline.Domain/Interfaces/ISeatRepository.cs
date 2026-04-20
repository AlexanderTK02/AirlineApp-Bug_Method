using Airline.Domain.Entities;
using Airline.Domain.Enums;

namespace Airline.Domain.Interfaces;

public interface ISeatRepository
{
    Seat? GetById(int id);
    List<Seat> GetAll();
    List<Seat> GetByFlightId(int flightId);
    List<Seat> GetAvailableByFlight(int flightId);
    List<Seat> GetByFlightAndClass(int flightId, SeatClass seatClass);
    Seat? GetBySeatNumber(int flightId, string seatNumber);
    void Add(Seat seat);
    void Update(Seat seat);
}
