// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Application.Services;
using Airline.Domain.Enums;
using Airline.Infrastructure.Repositories;

namespace Airline.Tests;

public class CrewServiceTests
{
    private (CrewService service, FlightService flightService) CreateService()
    {
        var crewRepo = new InMemoryCrewMemberRepository();
        var flightRepo = new InMemoryFlightRepository();
        return (new CrewService(crewRepo, flightRepo), new FlightService(flightRepo));
    }

    private (CrewService service, int flightId) CreateServiceWithFlight()
    {
        var (crewService, flightService) = CreateService();
        var flight = flightService.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4), AircraftType.Narrow, 180);
        return (crewService, flight.Id);
    }

    [Fact]
    public void AddCrewMember_ValidData_ReturnsCrewMember()
    {
        var (service, _) = CreateServiceWithFlight();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");

        Assert.Equal("Erik", crew.FirstName);
        Assert.Equal("Johansson", crew.LastName);
        Assert.Equal(CrewRole.Pilot, crew.Role);
    }

    [Fact]
    public void AddCrewMember_EmptyName_ThrowsArgumentException()
    {
        var (service, _) = CreateServiceWithFlight();
        Assert.Throws<ArgumentException>(() =>
            service.AddCrewMember("", "Johansson", CrewRole.Pilot, "LIC001"));
    }

    [Fact]
    public void AssignToFlight_ValidAssignment_AssignsCorrectly()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");

        service.AssignToFlight(crew.Id, flightId);
        var flightCrew = service.GetFlightCrew(flightId);
        Assert.Single(flightCrew);
    }

    [Fact]
    public void AssignToFlight_AlreadyAssigned_ThrowsInvalidOperationException()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        service.AssignToFlight(crew.Id, flightId);

        Assert.Throws<InvalidOperationException>(() =>
            service.AssignToFlight(crew.Id, flightId));
    }

    [Fact]
    public void GetFlightCrew_ReturnsCorrectCrew()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var pilot = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        var attendant = service.AddCrewMember("Anna", "Svensson", CrewRole.FlightAttendant, "LIC002");
        service.AssignToFlight(pilot.Id, flightId);
        service.AssignToFlight(attendant.Id, flightId);

        var crew = service.GetFlightCrew(flightId);
        Assert.Equal(2, crew.Count);
    }

    [Fact]
    public void GetAvailableCrew_ReturnsUnassigned()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var pilot = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        service.AddCrewMember("Anna", "Svensson", CrewRole.FlightAttendant, "LIC002");
        service.AssignToFlight(pilot.Id, flightId);

        var available = service.GetAvailableCrew();
        Assert.Single(available);
    }

    [Fact]
    public void UnassignFromFlight_AssignedCrew_Unassigns()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        service.AssignToFlight(crew.Id, flightId);

        service.UnassignFromFlight(crew.Id);
        var flightCrew = service.GetFlightCrew(flightId);
        Assert.Empty(flightCrew);
    }

    [Fact]
    public void UnassignFromFlight_NotAssigned_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithFlight();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");

        Assert.Throws<InvalidOperationException>(() =>
            service.UnassignFromFlight(crew.Id));
    }

    [Fact]
    public void GetPilotCount_ReturnsCorrectCount()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var pilot = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        var copilot = service.AddCrewMember("Anna", "Svensson", CrewRole.CoPilot, "LIC002");
        var attendant = service.AddCrewMember("Karl", "Larsson", CrewRole.FlightAttendant, "LIC003");
        service.AssignToFlight(pilot.Id, flightId);
        service.AssignToFlight(copilot.Id, flightId);
        service.AssignToFlight(attendant.Id, flightId);

        Assert.Equal(2, service.GetPilotCount(flightId));
    }

    [Fact]
    public void IsFlightFullyStaffed_WithAllRoles_ReturnsTrue()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var pilot = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        var copilot = service.AddCrewMember("Anna", "Svensson", CrewRole.CoPilot, "LIC002");
        var attendant = service.AddCrewMember("Karl", "Larsson", CrewRole.FlightAttendant, "LIC003");
        service.AssignToFlight(pilot.Id, flightId);
        service.AssignToFlight(copilot.Id, flightId);
        service.AssignToFlight(attendant.Id, flightId);

        Assert.True(service.IsFlightFullyStaffed(flightId));
    }

    [Fact]
    public void IsFlightFullyStaffed_MissingCoPilot_ReturnsFalse()
    {
        var (service, flightId) = CreateServiceWithFlight();
        var pilot = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        var attendant = service.AddCrewMember("Karl", "Larsson", CrewRole.FlightAttendant, "LIC003");
        service.AssignToFlight(pilot.Id, flightId);
        service.AssignToFlight(attendant.Id, flightId);

        Assert.False(service.IsFlightFullyStaffed(flightId));
    }

    [Fact]
    public void GetCrewMembersByRole_ReturnsCorrectMembers()
    {
        var (service, _) = CreateServiceWithFlight();
        service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        service.AddCrewMember("Anna", "Svensson", CrewRole.Pilot, "LIC002");
        service.AddCrewMember("Karl", "Larsson", CrewRole.FlightAttendant, "LIC003");

        var pilots = service.GetCrewMembersByRole(CrewRole.Pilot);
        Assert.Equal(2, pilots.Count);
    }

    [Fact]
    public void GetTotalCrewCount_ReturnsCorrectCount()
    {
        var (service, _) = CreateServiceWithFlight();
        service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        service.AddCrewMember("Anna", "Svensson", CrewRole.CoPilot, "LIC002");

        Assert.Equal(2, service.GetTotalCrewCount());
    }

    [Fact]
    public void GetCrewScheduleReport_AssignedFlight_ContainsCrewAndFlightInfo()
    {
        var (service, flightService) = CreateService();
        var crew = service.AddCrewMember("Erik", "Johansson", CrewRole.Pilot, "LIC001");
        var flight = flightService.CreateFlight("SK100", "ARN", "CPH",
            DateTime.Now.AddHours(2), DateTime.Now.AddHours(4),
            AircraftType.Narrow, 180);
        service.AssignToFlight(crew.Id, flight.Id);

        var from = DateTime.Now.AddDays(-1);
        var to = DateTime.Now.AddDays(2);
        var report = service.GetCrewScheduleReport(crew.Id, from, to);

        Assert.Contains("Erik Johansson", report);
        Assert.Contains("SK100", report);
    }

    [Fact]
    public void GetCrewScheduleReport_InvalidCrew_ThrowsInvalidOperationException()
    {
        var (service, _) = CreateServiceWithFlight();

        Assert.Throws<InvalidOperationException>(() =>
            service.GetCrewScheduleReport(999, DateTime.Now, DateTime.Now.AddDays(7)));
    }
}
