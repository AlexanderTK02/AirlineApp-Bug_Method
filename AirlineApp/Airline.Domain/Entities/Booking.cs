using Airline.Domain.Enums;

namespace Airline.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public BookingStatus Status { get; set; }
    public decimal Price { get; set; }
    public SeatClass SeatClass { get; set; }
}
