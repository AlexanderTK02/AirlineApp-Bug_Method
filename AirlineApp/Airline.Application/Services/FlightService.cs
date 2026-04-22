using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Application.Services;

public class FlightService
{
    private readonly IFlightRepository _flightRepository;
    private int _nextId = 1;

    public FlightService(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    // BUG_TARGET: CreateFlight |||*FIXED*|||
    public Flight CreateFlight(string flightNumber, string departureAirport, string arrivalAirport,
        DateTime departureTime, DateTime arrivalTime, AircraftType aircraftType, int totalSeats)
    {
        if (string.IsNullOrWhiteSpace(flightNumber))
            throw new ArgumentException("Flight number cannot be empty.");
        if (string.IsNullOrWhiteSpace(departureAirport) || string.IsNullOrWhiteSpace(arrivalAirport))
            throw new ArgumentException("Airport codes cannot be empty.");
        if (departureAirport == arrivalAirport)
            throw new ArgumentException("Departure and arrival airports must be different.");
        if (departureTime >= arrivalTime)
            throw new ArgumentException("Departure time must be before arrival time.");
        if (totalSeats <= 0)
            throw new ArgumentException("Total seats must be positive.");

        var flight = new Flight
        {
            Id = _nextId++,
            FlightNumber = flightNumber.ToUpper(),
            DepartureAirport = departureAirport.ToUpper(),
            ArrivalAirport = arrivalAirport.ToUpper(), // <== Right here was the bug //
            DepartureTime = departureTime,
            ArrivalTime = arrivalTime,
            Status = FlightStatus.Scheduled,
            AircraftType = aircraftType,
            TotalSeats = totalSeats
        };

        _flightRepository.Add(flight);
        return flight;
    }

    // BUG_TARGET: CalculateFlightDuration
    public decimal CalculateFlightDuration(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return Math.Round((decimal)(flight.ArrivalTime - flight.DepartureTime).TotalHours, 2);
    }

    // MISSING_TARGET: SearchFlights
    public List<Flight> SearchFlights(string departureAirport, string arrivalAirport, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(departureAirport) || string.IsNullOrWhiteSpace(arrivalAirport))
            throw new ArgumentException("Airport codes cannot be empty.");

        return _flightRepository.GetByRoute(departureAirport.ToUpper(), arrivalAirport.ToUpper())
            .Where(f => f.DepartureTime.Date == date.Date)
            .Where(f => f.Status != FlightStatus.Cancelled)
            .ToList();
    }

    // BUG_TARGET: UpdateFlightStatus |||*FIXED*|||
    public void UpdateFlightStatus(int flightId, FlightStatus newStatus)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        flight.Status = newStatus;
        _flightRepository.Update(flight); // <== Right here was the bug //
    }

    // MISSING_TARGET: GetFlightsByRoute
    public List<Flight> GetFlightsByRoute(string departureAirport, string arrivalAirport)
    {
        if (string.IsNullOrWhiteSpace(departureAirport) || string.IsNullOrWhiteSpace(arrivalAirport))
            throw new ArgumentException("Airport codes cannot be empty.");

        return _flightRepository.GetByRoute(departureAirport.ToUpper(), arrivalAirport.ToUpper());
    }

    // BUG_TARGET: GetDelayedFlights
    public List<Flight> GetDelayedFlights()
    {
        return _flightRepository.GetByStatus(FlightStatus.Delayed);
    }

    // MISSING_TARGET: GetFlightsByDateRange [IMPLEMENTED]
    public List<Flight> GetFlightsByDateRange(DateTime from, DateTime to)
    {
        return _flightRepository.GetByDateRange(from, to);
    }

    // BUG_TARGET: GetFlightCount
    public int GetFlightCount()
    {
        return _flightRepository.GetAll().Count;
    }

    // MISSING_TARGET: CancelFlight
    public void CancelFlight(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");
        if (flight.Status == FlightStatus.Departed)
            throw new InvalidOperationException("Flight has departed already");

        flight.Status = FlightStatus.Cancelled;
        _flightRepository.Update(flight);
    }

    public Flight? GetFlightById(int id)
    {
        return _flightRepository.GetById(id);
    }
}
