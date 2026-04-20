using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Repositories;

public class InMemorySeatRepository : ISeatRepository
{
    private readonly List<Seat> _seats = new();

    public Seat? GetById(int id) => _seats.FirstOrDefault(s => s.Id == id);

    public List<Seat> GetAll() => _seats.ToList();

    public List<Seat> GetByFlightId(int flightId) =>
        _seats.Where(s => s.FlightId == flightId).ToList();

    // BUG_TARGET: GetAvailableByFlight
    public List<Seat> GetAvailableByFlight(int flightId) =>
        _seats.Where(s => s.FlightId == flightId && s.IsAvailable).ToList();

    public List<Seat> GetByFlightAndClass(int flightId, SeatClass seatClass) =>
        _seats.Where(s => s.FlightId == flightId && s.SeatClass == seatClass).ToList();

    public Seat? GetBySeatNumber(int flightId, string seatNumber) =>
        _seats.FirstOrDefault(s => s.FlightId == flightId && s.SeatNumber == seatNumber);

    public void Add(Seat seat) => _seats.Add(seat);

    public void Update(Seat seat)
    {
        var index = _seats.FindIndex(s => s.Id == seat.Id);
        if (index >= 0) _seats[index] = seat;
    }
}
