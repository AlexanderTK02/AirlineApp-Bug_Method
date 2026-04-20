using Airline.Domain.Entities;

namespace Airline.Domain.Interfaces;

public interface IPassengerRepository
{
    Passenger? GetById(int id);
    List<Passenger> GetAll();
    Passenger? GetByPassport(string passportNumber);
    List<Passenger> Search(string keyword);
    void Add(Passenger passenger);
    void Update(Passenger passenger);
}
