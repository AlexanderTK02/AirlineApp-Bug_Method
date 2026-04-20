using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Repositories;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();

    public Booking? GetById(int id) => _bookings.FirstOrDefault(b => b.Id == id);

    public List<Booking> GetAll() => _bookings.ToList();

    // BUG_TARGET: GetByPassengerId
    public List<Booking> GetByPassengerId(int passengerId) =>
        _bookings.Where(b => b.PassengerId == passengerId).ToList();

    public List<Booking> GetByFlightId(int flightId) =>
        _bookings.Where(b => b.FlightId == flightId).ToList();

    public List<Booking> GetByStatus(BookingStatus status) =>
        _bookings.Where(b => b.Status == status).ToList();

    public void Add(Booking booking) => _bookings.Add(booking);

    public void Update(Booking booking)
    {
        var index = _bookings.FindIndex(b => b.Id == booking.Id);
        if (index >= 0) _bookings[index] = booking;
    }
}
