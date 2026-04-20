// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Application.Services;
using Airline.Domain.Enums;
using Airline.Infrastructure.Repositories;
using Airline.Infrastructure.Factories;

namespace Airline.Tests;

public class SeatServiceTests
{
    private (SeatService service, FlightService flightService) CreateService()
    {
        var seatRepo = new InMemorySeatRepository();
        var flightRepo = new InMemoryFlightRepository();
        var factory = new SeatPriceFactory();
        return (new SeatService(seatRepo, flightRepo, factory), new FlightService(flightRepo));
    }

    private (SeatService service, int flightId) CreateServiceWithFlight()
    {
        var (seatService, flightService) = CreateService();
        var flight = flightService.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        return (seatService, flight.Id);
    }

    [Fact]
    public void AddSeat_ValidData_ReturnsSeat()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var seat = service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);

        Assert.Equal("1A", seat.SeatNumber);
        Assert.Equal(SeatClass.Economy, seat.SeatClass);
        Assert.True(seat.IsAvailable);
        Assert.Equal(1000m, seat.BasePrice);
    }

    [Fact]
    public void AddSeat_EmptySeatNumber_ThrowsArgumentException()
    {
        var (service, flightId) = CreateServiceWithFlight();
        Assert.Throws<ArgumentException>(() =>
            service.AddSeat(flightId, "", SeatClass.Economy, 1000m));
    }

    [Fact]
    public void AddSeat_InvalidFlight_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithFlight();
        Assert.Throws<InvalidOperationException>(() =>
            service.AddSeat(999, "1A", SeatClass.Economy, 1000m));
    }

    [Fact]
    public void AddSeat_DuplicateSeatNumber_ThrowsInvalidOperationException()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        Assert.Throws<InvalidOperationException>(() =>
            service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m));
    }

    [Fact]
    public void GetAvailableSeats_ReturnsOnlyAvailable()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "1B", SeatClass.Economy, 1000m);

        var available = service.GetAvailableSeats(flightId);
        Assert.Equal(2, available.Count);
    }

    [Fact]
    public void CalculateOccupancyRate_AllAvailable_ReturnsZero()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "1B", SeatClass.Economy, 1000m);

        Assert.Equal(0m, service.CalculateOccupancyRate(flightId));
    }

    [Fact]
    public void GetSeatPrice_Economy_ReturnsBasePrice()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);

        var price = service.GetSeatPrice(flightId, "1A");
        Assert.Equal(1000m, price); // Economy multiplier 1.0
    }

    [Fact]
    public void GetSeatPrice_Business_ReturnsMultipliedPrice()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Business, 1000m);

        var price = service.GetSeatPrice(flightId, "1A");
        Assert.Equal(2500m, price); // Business multiplier 2.5
    }

    [Fact]
    public void GetSeatsByClass_ReturnsCorrectSeats()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "2A", SeatClass.Business, 2000m);
        service.AddSeat(flightId, "3A", SeatClass.Economy, 1000m);

        var economy = service.GetSeatsByClass(flightId, SeatClass.Economy);
        Assert.Equal(2, economy.Count);
    }

    [Fact]
    public void GetAvailableSeatCount_ReturnsCorrectCount()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "1B", SeatClass.Economy, 1000m);

        Assert.Equal(2, service.GetAvailableSeatCount(flightId));
    }

    [Fact]
    public void GetTotalSeatCount_ReturnsCorrectCount()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "1B", SeatClass.Economy, 1000m);
        service.AddSeat(flightId, "2A", SeatClass.Business, 2000m);

        Assert.Equal(3, service.GetTotalSeatCount(flightId));
    }

    [Fact]
    public void GetSeatInfo_ReturnsFormattedString()
    {
        var (service, flightId) = CreateServiceWithFlight();
        service.AddSeat(flightId, "1A", SeatClass.Economy, 1000m);

        var info = service.GetSeatInfo(flightId, "1A");
        Assert.Contains("1A", info);
        Assert.Contains("Economy", info);
        Assert.Contains("Ledig", info);
    }
}
