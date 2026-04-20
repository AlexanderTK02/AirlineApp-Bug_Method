using Airline.Domain.Entities;
using Airline.Domain.Interfaces;

namespace Airline.Application.Services;

public class PassengerService
{
    private readonly IPassengerRepository _passengerRepository;
    private readonly IBookingRepository _bookingRepository;
    private int _nextId = 1;

    public PassengerService(IPassengerRepository passengerRepository, IBookingRepository bookingRepository)
    {
        _passengerRepository = passengerRepository;
        _bookingRepository = bookingRepository;
    }

    // BUG_TARGET: RegisterPassenger
    public Passenger RegisterPassenger(string firstName, string lastName, string passportNumber, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");
        if (string.IsNullOrWhiteSpace(passportNumber))
            throw new ArgumentException("Passport number cannot be empty.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Invalid email address.");

        var existingPassenger = _passengerRepository.GetByPassport(passportNumber);
        if (existingPassenger != null)
            throw new InvalidOperationException("Passenger with this passport already exists.");

        var passenger = new Passenger
        {
            Id = _nextId++,
            FirstName = firstName,
            LastName = lastName,
            PassportNumber = passportNumber,
            Email = email,
            PhoneNumber = phoneNumber
        };

        _passengerRepository.Add(passenger);
        return passenger;
    }

    // MISSING_TARGET: GetPassengerFullName
    public string GetPassengerFullName(int passengerId)
    {
        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        return passenger.FullName;
    }

    // BUG_TARGET: GetFrequentFlyers
    public List<Passenger> GetFrequentFlyers()
    {
        return _passengerRepository.GetAll()
            .Where(p => !string.IsNullOrEmpty(p.FrequentFlyerNumber))
            .ToList();
    }

    // MISSING_TARGET: GetPassengerBookings
    public List<Booking> GetPassengerBookings(int passengerId)
    {
        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        return _bookingRepository.GetByPassengerId(passengerId);
    }

    // BUG_TARGET: UpdatePassengerInfo
    public void UpdatePassengerInfo(int passengerId, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Invalid email address.");

        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        passenger.Email = email;
        passenger.PhoneNumber = phoneNumber;
        _passengerRepository.Update(passenger);
    }

    // MISSING_TARGET: SetFrequentFlyerNumber
    public void SetFrequentFlyerNumber(int passengerId, string frequentFlyerNumber)
    {
        if (string.IsNullOrWhiteSpace(frequentFlyerNumber))
            throw new ArgumentException("Frequent flyer number cannot be empty.");

        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        passenger.FrequentFlyerNumber = frequentFlyerNumber;
        _passengerRepository.Update(passenger);
    }

    // BUG_TARGET: SearchPassengers
    public List<Passenger> SearchPassengers(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Search keyword cannot be empty.");

        return _passengerRepository.Search(keyword);
    }

    // MISSING_TARGET: GetPassengerCount
    public int GetPassengerCount()
    {
        return _passengerRepository.GetAll().Count;
    }

    public Passenger? GetPassengerById(int id)
    {
        return _passengerRepository.GetById(id);
    }
}
