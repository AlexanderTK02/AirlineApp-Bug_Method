using Airline.Domain.Enums;

namespace Airline.Domain.Entities;

public class Seat
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatClass SeatClass { get; set; }
    public bool IsAvailable { get; set; }
    public decimal BasePrice { get; set; }
}
