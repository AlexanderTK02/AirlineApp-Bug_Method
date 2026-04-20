using Airline.Domain.Entities;
using Airline.Domain.Enums;

namespace Airline.Domain.Interfaces;

public interface IBookingRepository
{
    Booking? GetById(int id);
    List<Booking> GetAll();
    List<Booking> GetByPassengerId(int passengerId);
    List<Booking> GetByFlightId(int flightId);
    List<Booking> GetByStatus(BookingStatus status);
    void Add(Booking booking);
    void Update(Booking booking);
}
