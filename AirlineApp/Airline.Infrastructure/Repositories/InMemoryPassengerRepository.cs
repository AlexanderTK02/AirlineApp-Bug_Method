using Airline.Domain.Entities;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Repositories;

public class InMemoryPassengerRepository : IPassengerRepository
{
    private readonly List<Passenger> _passengers = new();

    public Passenger? GetById(int id) => _passengers.FirstOrDefault(p => p.Id == id);

    public List<Passenger> GetAll() => _passengers.ToList();

    public Passenger? GetByPassport(string passportNumber) =>
        _passengers.FirstOrDefault(p => p.PassportNumber == passportNumber);

    // BUG_TARGET: Search
    public List<Passenger> Search(string keyword) =>
        _passengers.Where(p =>
            p.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            p.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            p.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        .ToList();

    public void Add(Passenger passenger) => _passengers.Add(passenger);

    public void Update(Passenger passenger)
    {
        var index = _passengers.FindIndex(p => p.Id == passenger.Id);
        if (index >= 0) _passengers[index] = passenger;
    }
}
