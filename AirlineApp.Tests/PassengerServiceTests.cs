// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Application.Services;
using Airline.Domain.Enums;
using Airline.Infrastructure.Repositories;

namespace Airline.Tests;

public class PassengerServiceTests
{
    private PassengerService CreateService()
    {
        var passengerRepo = new InMemoryPassengerRepository();
        var bookingRepo = new InMemoryBookingRepository();
        return new PassengerService(passengerRepo, bookingRepo);
    }

    [Fact]
    public void RegisterPassenger_ValidData_ReturnsPassenger()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070-1234567");

        Assert.Equal("Anna", passenger.FirstName);
        Assert.Equal("Svensson", passenger.LastName);
        Assert.Equal("SE123456", passenger.PassportNumber);
    }

    [Fact]
    public void RegisterPassenger_EmptyFirstName_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() =>
            service.RegisterPassenger("", "Svensson", "SE123456", "anna@test.se", "070"));
    }

    [Fact]
    public void RegisterPassenger_InvalidEmail_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() =>
            service.RegisterPassenger("Anna", "Svensson", "SE123456", "invalid", "070"));
    }

    [Fact]
    public void RegisterPassenger_DuplicatePassport_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");
        Assert.Throws<InvalidOperationException>(() =>
            service.RegisterPassenger("Erik", "Johansson", "SE123456", "erik@test.se", "071"));
    }

    [Fact]
    public void GetPassengerFullName_ReturnsCorrectName()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");

        var fullName = service.GetPassengerFullName(passenger.Id);
        Assert.Equal("Anna Svensson", fullName);
    }

    [Fact]
    public void GetFrequentFlyers_ReturnsOnlyFrequentFlyers()
    {
        var service = CreateService();
        var p1 = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");
        service.RegisterPassenger("Erik", "Johansson", "SE789012", "erik@test.se", "071");
        service.SetFrequentFlyerNumber(p1.Id, "FF001");

        var flyers = service.GetFrequentFlyers();
        Assert.Single(flyers);
    }

    [Fact]
    public void GetPassengerBookings_NoBookings_ReturnsEmpty()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");

        var bookings = service.GetPassengerBookings(passenger.Id);
        Assert.Empty(bookings);
    }

    [Fact]
    public void UpdatePassengerInfo_ValidData_UpdatesFields()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");

        service.UpdatePassengerInfo(passenger.Id, "new@test.se", "071-9999999");
        var updated = service.GetPassengerById(passenger.Id);
        Assert.Equal("new@test.se", updated!.Email);
        Assert.Equal("071-9999999", updated.PhoneNumber);
    }

    [Fact]
    public void UpdatePassengerInfo_InvalidEmail_ThrowsArgumentException()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");

        Assert.Throws<ArgumentException>(() =>
            service.UpdatePassengerInfo(passenger.Id, "invalid", "071"));
    }

    [Fact]
    public void SetFrequentFlyerNumber_SetsCorrectly()
    {
        var service = CreateService();
        var passenger = service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");

        service.SetFrequentFlyerNumber(passenger.Id, "FF001");
        var updated = service.GetPassengerById(passenger.Id);
        Assert.Equal("FF001", updated!.FrequentFlyerNumber);
    }

    [Fact]
    public void SearchPassengers_MatchesLastName_ReturnsResults()
    {
        var service = CreateService();
        service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");
        service.RegisterPassenger("Erik", "Johansson", "SE789012", "erik@test.se", "071");

        var results = service.SearchPassengers("Svensson");
        Assert.Single(results);
    }

    [Fact]
    public void GetPassengerCount_ReturnsCorrectCount()
    {
        var service = CreateService();
        service.RegisterPassenger("Anna", "Svensson", "SE123456", "anna@test.se", "070");
        service.RegisterPassenger("Erik", "Johansson", "SE789012", "erik@test.se", "071");

        Assert.Equal(2, service.GetPassengerCount());
    }
}
