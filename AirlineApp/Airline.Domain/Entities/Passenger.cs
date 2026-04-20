namespace Airline.Domain.Entities;

public class Passenger
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? FrequentFlyerNumber { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
