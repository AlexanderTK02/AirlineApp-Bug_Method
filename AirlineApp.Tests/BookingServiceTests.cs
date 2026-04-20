// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Application.Services;
using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Infrastructure.Repositories;
using Airline.Infrastructure.Factories;

namespace Airline.Tests;

public class BookingServiceTests
{
    private (BookingService service, FlightService flightService, PassengerService passengerService, SeatService seatService)
        CreateServices()
    {
        var bookingRepo = new InMemoryBookingRepository();
        var flightRepo = new InMemoryFlightRepository();
        var seatRepo = new InMemorySeatRepository();
        var passengerRepo = new InMemoryPassengerRepository();
        var factory = new SeatPriceFactory();

        var bookingService = new BookingService(bookingRepo, flightRepo, seatRepo, passengerRepo, factory);
        var flightService = new FlightService(flightRepo);
        var passengerService = new PassengerService(passengerRepo, bookingRepo);
        var seatService = new SeatService(seatRepo, flightRepo, factory);

        return (bookingService, flightService, passengerService, seatService);
    }

    private (BookingService service, int passengerId, int flightId, string seatNumber)
        CreateServicesWithData()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();

        var passenger = passengerService.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070-1234567");
        var flight = flightService.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        seatService.AddSeat(flight.Id, "1A", SeatClass.Economy, 1000m);
        seatService.AddSeat(flight.Id, "1B", SeatClass.Business, 1000m);

        return (bookingService, passenger.Id, flight.Id, "1A");
    }

    [Fact]
    public void CreateBooking_ValidData_ReturnsBooking()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        Assert.NotNull(booking);
        Assert.Equal(passengerId, booking.PassengerId);
        Assert.Equal(flightId, booking.FlightId);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.Equal(1000m, booking.Price); // Economy multiplier = 1.0
    }

    [Fact]
    public void CreateBooking_InvalidPassenger_ThrowsInvalidOperationException()
    {
        var (service, _, flightId, seatNumber) = CreateServicesWithData();
        Assert.Throws<InvalidOperationException>(() =>
            service.CreateBooking(999, flightId, seatNumber, SeatClass.Economy));
    }

    [Fact]
    public void CreateBooking_InvalidFlight_ThrowsInvalidOperationException()
    {
        var (service, passengerId, _, seatNumber) = CreateServicesWithData();
        Assert.Throws<InvalidOperationException>(() =>
            service.CreateBooking(passengerId, 999, seatNumber, SeatClass.Economy));
    }

    [Fact]
    public void CancelBooking_ValidBooking_SetsCancelled()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        service.CancelBooking(booking.Id);
        var bookings = service.GetBookingsByPassenger(passengerId);
        Assert.Equal(BookingStatus.Cancelled, bookings.First().Status);
    }

    [Fact]
    public void CancelBooking_AlreadyCancelled_ThrowsInvalidOperationException()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);
        service.CancelBooking(booking.Id);

        Assert.Throws<InvalidOperationException>(() => service.CancelBooking(booking.Id));
    }

    [Fact]
    public void GetBookingsByFlight_ReturnsCorrectBookings()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        var bookings = service.GetBookingsByFlight(flightId);
        Assert.Single(bookings);
    }

    [Fact]
    public void CalculateBookingRevenue_ReturnsCorrectRevenue()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        var revenue = service.CalculateBookingRevenue(flightId);
        Assert.Equal(1000m, revenue);
    }

    [Fact]
    public void CheckIn_ConfirmedBooking_SetsCheckedIn()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        service.CheckIn(booking.Id);
        var bookings = service.GetBookingsByPassenger(passengerId);
        Assert.Equal(BookingStatus.CheckedIn, bookings.First().Status);
    }

    [Fact]
    public void CheckIn_CancelledBooking_ThrowsInvalidOperationException()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);
        service.CancelBooking(booking.Id);

        Assert.Throws<InvalidOperationException>(() => service.CheckIn(booking.Id));
    }

    [Fact]
    public void GetBookingsByPassenger_ReturnsCorrectBookings()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        var bookings = service.GetBookingsByPassenger(passengerId);
        Assert.Single(bookings);
    }

    [Fact]
    public void UpgradeSeat_EconomyToBusiness_ReturnsPriceDiff()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        var diff = service.UpgradeSeat(booking.Id, SeatClass.Business);
        Assert.True(diff > 0);
    }

    [Fact]
    public void GetTotalBookingCount_ReturnsCorrectCount()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);

        Assert.Equal(1, service.GetTotalBookingCount());
    }

    [Fact]
    public void GetTotalRevenue_CancelledNotCounted()
    {
        var (service, passengerId, flightId, seatNumber) = CreateServicesWithData();
        var booking = service.CreateBooking(passengerId, flightId, seatNumber, SeatClass.Economy);
        service.CancelBooking(booking.Id);

        var revenue = service.GetTotalRevenue();
        Assert.Equal(0m, revenue);
    }

    [Fact]
    public void GetFlightManifest_WithConfirmedBooking_ContainsPassengerName()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var passenger = passengerService.RegisterPassenger("Maria", "Lindqvist", "SE999", "m@test.se", "070-999");
        var flight = flightService.CreateFlight("SK200", "ARN", "GOT",
            DateTime.Now.AddHours(3), DateTime.Now.AddHours(5), AircraftType.Narrow, 180);
        seatService.AddSeat(flight.Id, "2A", SeatClass.Economy, 900m);
        bookingService.CreateBooking(passenger.Id, flight.Id, "2A", SeatClass.Economy);

        var manifest = bookingService.GetFlightManifest(flight.Id);

        Assert.Contains("Maria Lindqvist", manifest);
        Assert.Contains("SK200", manifest);
    }

    [Fact]
    public void GetFlightManifest_CancelledBookingExcluded_NotInManifest()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var p1 = passengerService.RegisterPassenger("Jonas", "Berg", "SE111", "j@test.se", "070-111");
        var p2 = passengerService.RegisterPassenger("Lisa", "Gran", "SE222", "l@test.se", "070-222");
        var flight = flightService.CreateFlight("SK300", "ARN", "CPH",
            DateTime.Now.AddHours(3), DateTime.Now.AddHours(5), AircraftType.Narrow, 180);
        seatService.AddSeat(flight.Id, "3A", SeatClass.Economy, 800m);
        seatService.AddSeat(flight.Id, "3B", SeatClass.Economy, 800m);
        var b1 = bookingService.CreateBooking(p1.Id, flight.Id, "3A", SeatClass.Economy);
        bookingService.CreateBooking(p2.Id, flight.Id, "3B", SeatClass.Economy);
        bookingService.CancelBooking(b1.Id);

        var manifest = bookingService.GetFlightManifest(flight.Id);

        Assert.DoesNotContain("Jonas Berg", manifest);
        Assert.Contains("Lisa Gran", manifest);
    }

    [Fact]
    public void GetFlightManifest_InvalidFlight_ThrowsInvalidOperationException()
    {
        var (service, _, _, _) = CreateServices();

        Assert.Throws<InvalidOperationException>(() => service.GetFlightManifest(999));
    }

    [Fact]
    public void ReschedulePassenger_ValidData_CreatesNewBooking()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var passenger = passengerService.RegisterPassenger("Karl", "Åberg", "SE333", "k@test.se", "070-333");
        var flight1 = flightService.CreateFlight("SK400", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        var flight2 = flightService.CreateFlight("SK401", "ARN", "CPH",
            DateTime.Now.AddHours(6), DateTime.Now.AddHours(8), AircraftType.Narrow, 180);
        seatService.AddSeat(flight1.Id, "4A", SeatClass.Economy, 1000m);
        seatService.AddSeat(flight2.Id, "4B", SeatClass.Economy, 1000m);
        var oldBooking = bookingService.CreateBooking(passenger.Id, flight1.Id, "4A", SeatClass.Economy);

        var newBooking = bookingService.ReschedulePassenger(oldBooking.Id, flight2.Id);

        Assert.NotNull(newBooking);
        Assert.Equal(flight2.Id, newBooking.FlightId);
        Assert.Equal(passenger.Id, newBooking.PassengerId);
    }

    [Fact]
    public void ReschedulePassenger_ValidData_CancelsOldBooking()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var passenger = passengerService.RegisterPassenger("Sara", "Holm", "SE444", "s@test.se", "070-444");
        var flight1 = flightService.CreateFlight("SK500", "ARN", "GOT",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        var flight2 = flightService.CreateFlight("SK501", "ARN", "GOT",
            DateTime.Now.AddHours(6), DateTime.Now.AddHours(8), AircraftType.Narrow, 180);
        seatService.AddSeat(flight1.Id, "5A", SeatClass.Economy, 1100m);
        seatService.AddSeat(flight2.Id, "5B", SeatClass.Economy, 1100m);
        var oldBooking = bookingService.CreateBooking(passenger.Id, flight1.Id, "5A", SeatClass.Economy);

        bookingService.ReschedulePassenger(oldBooking.Id, flight2.Id);

        var passengerBookings = bookingService.GetBookingsByPassenger(passenger.Id);
        var cancelledBooking = passengerBookings.First(b => b.FlightId == flight1.Id);
        Assert.Equal(BookingStatus.Cancelled, cancelledBooking.Status);
    }

    [Fact]
    public void ReschedulePassenger_NoAvailableSeats_ThrowsInvalidOperationException()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var p1 = passengerService.RegisterPassenger("Per", "Nilsson", "SE555", "p@test.se", "070-555");
        var p2 = passengerService.RegisterPassenger("Eva", "Larsson", "SE556", "e@test.se", "070-556");
        var flight1 = flightService.CreateFlight("SK600", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        var flight2 = flightService.CreateFlight("SK601", "ARN", "CPH",
            DateTime.Now.AddHours(6), DateTime.Now.AddHours(8), AircraftType.Narrow, 180);
        seatService.AddSeat(flight1.Id, "6A", SeatClass.Economy, 1200m);
        seatService.AddSeat(flight2.Id, "6B", SeatClass.Economy, 1200m);
        var b1 = bookingService.CreateBooking(p1.Id, flight1.Id, "6A", SeatClass.Economy);
        bookingService.CreateBooking(p2.Id, flight2.Id, "6B", SeatClass.Economy);

        Assert.Throws<InvalidOperationException>(() =>
            bookingService.ReschedulePassenger(b1.Id, flight2.Id));
    }

    [Fact]
    public void CalculateRouteRevenue_ValidRoute_ReturnsSumOfNonCancelledBookings()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var p1 = passengerService.RegisterPassenger("Nils", "Ek", "SE777", "n@test.se", "070-777");
        var p2 = passengerService.RegisterPassenger("Karin", "Mo", "SE778", "k2@test.se", "070-778");
        var flight = flightService.CreateFlight("SK700", "ARN", "LHR",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(6), AircraftType.Wide, 300);
        seatService.AddSeat(flight.Id, "7A", SeatClass.Economy, 2000m);
        seatService.AddSeat(flight.Id, "7B", SeatClass.Economy, 2000m);
        bookingService.CreateBooking(p1.Id, flight.Id, "7A", SeatClass.Economy);
        bookingService.CreateBooking(p2.Id, flight.Id, "7B", SeatClass.Economy);

        var revenue = bookingService.CalculateRouteRevenue("ARN", "LHR");

        Assert.Equal(4000m, revenue);
    }

    [Fact]
    public void CalculateRouteRevenue_ExcludesCancelledBookings()
    {
        var (bookingService, flightService, passengerService, seatService) = CreateServices();
        var p1 = passengerService.RegisterPassenger("Bo", "Sun", "SE888", "b@test.se", "070-888");
        var p2 = passengerService.RegisterPassenger("Maj", "Lund", "SE889", "m2@test.se", "070-889");
        var flight = flightService.CreateFlight("SK800", "GOT", "LHR",
            DateTime.Now.AddHours(3), DateTime.Now.AddHours(7), AircraftType.Wide, 300);
        seatService.AddSeat(flight.Id, "8A", SeatClass.Economy, 1500m);
        seatService.AddSeat(flight.Id, "8B", SeatClass.Economy, 1500m);
        var b1 = bookingService.CreateBooking(p1.Id, flight.Id, "8A", SeatClass.Economy);
        bookingService.CreateBooking(p2.Id, flight.Id, "8B", SeatClass.Economy);
        bookingService.CancelBooking(b1.Id);

        var revenue = bookingService.CalculateRouteRevenue("GOT", "LHR");

        Assert.Equal(1500m, revenue);
    }
}
