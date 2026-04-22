using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Application.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IPassengerRepository _passengerRepository;
    private readonly ISeatPriceFactory _seatPriceFactory;
    private int _nextId = 1;

    public BookingService(
        IBookingRepository bookingRepository,
        IFlightRepository flightRepository,
        ISeatRepository seatRepository,
        IPassengerRepository passengerRepository,
        ISeatPriceFactory seatPriceFactory)
    {
        _bookingRepository = bookingRepository;
        _flightRepository = flightRepository;
        _seatRepository = seatRepository;
        _passengerRepository = passengerRepository;
        _seatPriceFactory = seatPriceFactory;
    }

    // BUG_TARGET: CreateBooking
    public Booking CreateBooking(int passengerId, int flightId, string seatNumber, SeatClass seatClass)
    {
        if (string.IsNullOrWhiteSpace(seatNumber))
            throw new ArgumentException("Seat number cannot be empty.");

        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");
        if (flight.Status == FlightStatus.Cancelled)
            throw new InvalidOperationException("Cannot book a cancelled flight.");

        var seat = _seatRepository.GetBySeatNumber(flightId, seatNumber);
        if (seat == null)
            throw new InvalidOperationException("Seat not found.");
        if (!seat.IsAvailable)
            throw new InvalidOperationException("Seat is not available.");

        var existingBooking = _bookingRepository.GetByPassengerId(passengerId)
            .Any(b => b.FlightId == flightId && b.Status == BookingStatus.Confirmed);
        if (existingBooking)
            throw new InvalidOperationException("Passenger already has a booking on this flight.");

        var price = _seatPriceFactory.CalculatePrice(seat.BasePrice, seatClass);

        seat.IsAvailable = false;
        _seatRepository.Update(seat);

        var booking = new Booking
        {
            Id = _nextId++,
            PassengerId = passengerId,
            FlightId = flightId,
            SeatNumber = seatNumber,
            BookingDate = DateTime.Now,
            Status = BookingStatus.Confirmed,
            Price = price,
            SeatClass = seatClass
        };

        _bookingRepository.Add(booking);
        return booking;
    }

    // BUG_TARGET: CancelBooking
    public void CancelBooking(int bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
            throw new InvalidOperationException("Booking not found.");
        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");

        booking.Status = BookingStatus.Cancelled;
        _bookingRepository.Update(booking);

        var seat = _seatRepository.GetBySeatNumber(booking.FlightId, booking.SeatNumber);
        if (seat != null)
        {
            seat.IsAvailable = true;
            _seatRepository.Update(seat);
        }
    }

    // MISSING_TARGET: GetBookingsByFlight
    public List<Booking> GetBookingsByFlight(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _bookingRepository.GetByFlightId(flightId)
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn)
            .ToList();
    }

    // BUG_TARGET: CalculateBookingRevenue
    public decimal CalculateBookingRevenue(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _bookingRepository.GetByFlightId(flightId)
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Sum(b => b.Price);
    }

    // MISSING_TARGET: CheckIn
    public void CheckIn(int bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
            throw new InvalidOperationException("Booking not found.");
        if (booking.Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be checked in.");

        booking.Status = BookingStatus.CheckedIn;
        _bookingRepository.Update(booking);
    }

    // BUG_TARGET: GetBookingsByPassenger
    public List<Booking> GetBookingsByPassenger(int passengerId)
    {
        var passenger = _passengerRepository.GetById(passengerId);
        if (passenger == null)
            throw new InvalidOperationException("Passenger not found.");

        return _bookingRepository.GetByPassengerId(passengerId);
    }

    // MISSING_TARGET: UpgradeSeat
    public decimal UpgradeSeat(int bookingId, SeatClass newClass)
    {
        throw new NotImplementedException();
    }

    // MISSING_TARGET: GetTotalBookingCount
    public int GetTotalBookingCount()
    {
        throw new NotImplementedException();
    }

    // BUG_TARGET: GetTotalRevenue
    public decimal GetTotalRevenue()
    {
        return _bookingRepository.GetAll()
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Sum(b => b.Price);
    }

    // MISSING_TARGET: GetFlightManifest
    public string GetFlightManifest(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        var bookings = _bookingRepository.GetByFlightId(flightId)
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn)
            .OrderBy(b => b.SeatNumber)
            .ToList();

        var lines = new List<string>
        {
            $"Manifest: {flight.FlightNumber} ({flight.DepartureAirport}→{flight.ArrivalAirport})"
        };

        foreach (var booking in bookings)
        {
            var passenger = _passengerRepository.GetById(booking.PassengerId);
            var name = passenger != null
                ? $"{passenger.FirstName} {passenger.LastName}"
                : "Okänd";
            lines.Add($"  {booking.SeatNumber} ({booking.SeatClass}): {name}");
        }

        return string.Join("\n", lines);
    }

    // MISSING_TARGET: ReschedulePassenger |||*FIXED*|||
    public Booking ReschedulePassenger(int bookingId, int newFlightId)
    {
        var oldBooking = _bookingRepository.GetById(bookingId);
        if (oldBooking == null)
            throw new InvalidOperationException("Booking not found.");
        if (oldBooking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot reschedule a cancelled booking.");

        var newFlight = _flightRepository.GetById(newFlightId);
        if (newFlight == null)
            throw new InvalidOperationException("New flight not found.");
        if (newFlight.Status == FlightStatus.Cancelled)
            throw new InvalidOperationException("Cannot rebook on a cancelled flight.");

        var availableSeats = _seatRepository.GetAvailableByFlight(newFlightId)
            .Where(s => s.SeatClass == oldBooking.SeatClass)
            .ToList();
        if (availableSeats.Count == 0)
            throw new InvalidOperationException("No available seats of the same class on the new flight.");

        // Cancel old booking and release old seat
        var oldSeat = _seatRepository.GetBySeatNumber(oldBooking.FlightId, oldBooking.SeatNumber);
        if (oldSeat != null)
        {
            oldSeat.IsAvailable = true; // <== Bug2 was right here //
            _seatRepository.Update(oldSeat);
        }
        oldBooking.Status = BookingStatus.Cancelled; // <== Bug was right here //
        _bookingRepository.Update(oldBooking);

        // Book first available seat on new flight
        var newSeat = availableSeats.First();
        newSeat.IsAvailable = false;
        _seatRepository.Update(newSeat);

        var newBooking = new Booking
        {
            Id = _nextId++,
            PassengerId = oldBooking.PassengerId,
            FlightId = newFlightId,
            SeatNumber = newSeat.SeatNumber,
            BookingDate = DateTime.Now,
            Status = BookingStatus.Confirmed,
            Price = oldBooking.Price,
            SeatClass = oldBooking.SeatClass
        };
        _bookingRepository.Add(newBooking);
        return newBooking;
    }

    // MISSING_TARGET: CalculateRouteRevenue |||*FIXED*||| [Forgot to add "!" at start and test still went through for some reason]
    public decimal CalculateRouteRevenue(string departureAirport, string arrivalAirport)
    {
        if (string.IsNullOrWhiteSpace(departureAirport) || string.IsNullOrWhiteSpace(arrivalAirport))
            throw new ArgumentException("Airport codes cannot be empty.");

        var flights = _flightRepository.GetByRoute(
            departureAirport.ToUpper(), arrivalAirport.ToUpper());

        decimal totalRevenue = 0;
        foreach (var flight in flights)
        {
            totalRevenue += _bookingRepository.GetByFlightId(flight.Id)
                .Where(b => !b.Status.Equals(BookingStatus.Cancelled)) // <== Bug was right here //
                .Sum(b => b.Price);
        }

        return Math.Round(totalRevenue, 2);
    }
}
