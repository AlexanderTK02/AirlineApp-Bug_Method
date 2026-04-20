// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Application.Services;
using Airline.Domain.Enums;
using Airline.Infrastructure.Repositories;

namespace Airline.Tests;

public class FlightServiceTests
{
    private FlightService CreateService()
    {
        var flightRepo = new InMemoryFlightRepository();
        return new FlightService(flightRepo);
    }

    [Fact]
    public void CreateFlight_ValidData_ReturnsFlight()
    {
        var service = CreateService();
        var flight = service.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
            AircraftType.Narrow, 180);

        Assert.Equal("SK100", flight.FlightNumber);
        Assert.Equal("ARN", flight.DepartureAirport);
        Assert.Equal("CPH", flight.ArrivalAirport);
        Assert.Equal(FlightStatus.Scheduled, flight.Status);
        Assert.Equal(180, flight.TotalSeats);
    }

    [Fact]
    public void CreateFlight_EmptyFlightNumber_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() =>
            service.CreateFlight("", "ARN", "CPH",
                DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
                AircraftType.Narrow, 180));
    }

    [Fact]
    public void CreateFlight_SameAirports_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() =>
            service.CreateFlight("SK100", "ARN", "ARN",
                DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
                AircraftType.Narrow, 180));
    }

    [Fact]
    public void CreateFlight_DepartureAfterArrival_ThrowsArgumentException()
    {
        var service = CreateService();
        Assert.Throws<ArgumentException>(() =>
            service.CreateFlight("SK100", "ARN", "CPH",
                DateTime.Now.AddHours(4), DateTime.Now.AddHours(2),
                AircraftType.Narrow, 180));
    }

    [Fact]
    public void CalculateFlightDuration_ReturnsCorrectHours()
    {
        var service = CreateService();
        var flight = service.CreateFlight("SK100", "ARN", "CPH",
            new DateTime(2026, 1, 1, 10, 0, 0), new DateTime(2026, 1, 1, 12, 30, 0),
            AircraftType.Narrow, 180);

        var duration = service.CalculateFlightDuration(flight.Id);
        Assert.Equal(2.5m, duration);
    }

    [Fact]
    public void CalculateFlightDuration_InvalidFlight_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        Assert.Throws<InvalidOperationException>(() => service.CalculateFlightDuration(999));
    }

    [Fact]
    public void SearchFlights_MatchingRoute_ReturnsFlights()
    {
        var service = CreateService();
        var date = new DateTime(2026, 6, 15, 10, 0, 0);
        service.CreateFlight("SK100", "ARN", "CPH", date, date.AddHours(2), AircraftType.Narrow, 180);
        service.CreateFlight("SK200", "ARN", "LHR", date, date.AddHours(3), AircraftType.Wide, 300);

        var results = service.SearchFlights("ARN", "CPH", date.Date);
        Assert.Single(results);
    }

    [Fact]
    public void UpdateFlightStatus_ValidFlight_UpdatesStatus()
    {
        var service = CreateService();
        var flight = service.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
            AircraftType.Narrow, 180);

        service.UpdateFlightStatus(flight.Id, FlightStatus.Boarding);
        var updated = service.GetFlightById(flight.Id);
        Assert.Equal(FlightStatus.Boarding, updated!.Status);
    }

    [Fact]
    public void GetFlightsByRoute_ReturnsCorrectFlights()
    {
        var service = CreateService();
        service.CreateFlight("SK100", "ARN", "CPH", DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), AircraftType.Narrow, 180);
        service.CreateFlight("SK200", "ARN", "LHR", DateTime.Now.AddHours(2), DateTime.Now.AddHours(5), AircraftType.Wide, 300);

        var flights = service.GetFlightsByRoute("ARN", "CPH");
        Assert.Single(flights);
    }

    [Fact]
    public void GetDelayedFlights_ReturnsOnlyDelayed()
    {
        var service = CreateService();
        var f1 = service.CreateFlight("SK100", "ARN", "CPH", DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), AircraftType.Narrow, 180);
        service.CreateFlight("SK200", "ARN", "LHR", DateTime.Now.AddHours(2), DateTime.Now.AddHours(5), AircraftType.Wide, 300);
        service.UpdateFlightStatus(f1.Id, FlightStatus.Delayed);

        var delayed = service.GetDelayedFlights();
        Assert.Single(delayed);
    }

    [Fact]
    public void CancelFlight_ScheduledFlight_SetsCancelled()
    {
        var service = CreateService();
        var flight = service.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
            AircraftType.Narrow, 180);

        service.CancelFlight(flight.Id);
        var updated = service.GetFlightById(flight.Id);
        Assert.Equal(FlightStatus.Cancelled, updated!.Status);
    }

    [Fact]
    public void CancelFlight_DepartedFlight_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var flight = service.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
            AircraftType.Narrow, 180);
        service.UpdateFlightStatus(flight.Id, FlightStatus.Departed);

        Assert.Throws<InvalidOperationException>(() => service.CancelFlight(flight.Id));
    }

    [Fact]
    public void GetFlightsByDateRange_ReturnsCorrectFlights()
    {
        var service = CreateService();
        var date = new DateTime(2026, 6, 15, 10, 0, 0);
        service.CreateFlight("SK100", "ARN", "CPH", date, date.AddHours(2), AircraftType.Narrow, 180);
        service.CreateFlight("SK200", "ARN", "LHR", date.AddDays(5), date.AddDays(5).AddHours(3), AircraftType.Wide, 300);

        var flights = service.GetFlightsByDateRange(date.AddDays(-1), date.AddDays(1));
        Assert.Single(flights);
    }

    [Fact]
    public void GetFlightCount_ReturnsCorrectCount()
    {
        var service = CreateService();
        service.CreateFlight("SK100", "ARN", "CPH", DateTime.Now.AddHours(1), DateTime.Now.AddHours(3), AircraftType.Narrow, 180);
        service.CreateFlight("SK200", "ARN", "LHR", DateTime.Now.AddHours(2), DateTime.Now.AddHours(5), AircraftType.Wide, 300);

        Assert.Equal(2, service.GetFlightCount());
    }
}
