using Airline.Domain.Enums;

namespace Airline.Domain.Entities;

public class CrewMember
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public CrewRole Role { get; set; }
    public int FlightId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
}
