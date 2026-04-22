using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Application.Services;

public class SeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly ISeatPriceFactory _seatPriceFactory;
    private int _nextId = 1;

    public SeatService(ISeatRepository seatRepository, IFlightRepository flightRepository, ISeatPriceFactory seatPriceFactory)
    {
        _seatRepository = seatRepository;
        _flightRepository = flightRepository;
        _seatPriceFactory = seatPriceFactory;
    }

    // BUG_TARGET: AddSeat
    public Seat AddSeat(int flightId, string seatNumber, SeatClass seatClass, decimal basePrice)
    {
        if (string.IsNullOrWhiteSpace(seatNumber))
            throw new ArgumentException("Seat number cannot be empty.");
        if (basePrice <= 0)
            throw new ArgumentException("Base price must be positive.");

        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        var existingSeat = _seatRepository.GetBySeatNumber(flightId, seatNumber);
        if (existingSeat != null)
            throw new InvalidOperationException("Seat already exists.");

        var seat = new Seat
        {
            Id = _nextId++,
            FlightId = flightId,
            SeatNumber = seatNumber,
            SeatClass = seatClass,
            IsAvailable = true,
            BasePrice = basePrice
        };

        _seatRepository.Add(seat);
        return seat;
    }

    // MISSING_TARGET: GetAvailableSeats
    public List<Seat> GetAvailableSeats(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _seatRepository.GetAvailableByFlight(flightId);
    }

    // BUG_TARGET: CalculateOccupancyRate |||*FIXED*|||
    public decimal CalculateOccupancyRate(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        var seats = _seatRepository.GetByFlightId(flightId);
        if (seats.Count == 0)
            return 0;

        var occupied = seats.Count(s => !s.IsAvailable); // <== Bug was right here //
        return Math.Round((decimal)occupied / seats.Count * 100, 2);
    }

    // MISSING_TARGET: GetSeatPrice
    public decimal GetSeatPrice(int flightId, string seatNumber)
    {
        var seat = _seatRepository.GetBySeatNumber(flightId, seatNumber);
        if (seat == null)
            throw new InvalidOperationException("Seat not found.");

        return _seatPriceFactory.CalculatePrice(seat.BasePrice, seat.SeatClass);
    }

    // BUG_TARGET: GetSeatsByClass
    public List<Seat> GetSeatsByClass(int flightId, SeatClass seatClass)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _seatRepository.GetByFlightAndClass(flightId, seatClass);
    }

    // MISSING_TARGET: GetAvailableSeatCount
    public int GetAvailableSeatCount(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _seatRepository.GetAvailableByFlight(flightId).Count;
    }

    // BUG_TARGET: GetTotalSeatCount
    public int GetTotalSeatCount(int flightId)
    {
        return _seatRepository.GetByFlightId(flightId).Count;
    }

    // MISSING_TARGET: GetSeatInfo
    public string GetSeatInfo(int flightId, string seatNumber)
    {
        var seat = _seatRepository.GetBySeatNumber(flightId, seatNumber);
        if (seat == null)
            throw new InvalidOperationException("Seat not found.");

        var price = _seatPriceFactory.CalculatePrice(seat.BasePrice, seat.SeatClass);
        var status = seat.IsAvailable ? "Ledig" : "Bokad";
        return $"Säte {seat.SeatNumber} | Klass: {seat.SeatClass} | Pris: {price:C} | Status: {status}";
    }
}
