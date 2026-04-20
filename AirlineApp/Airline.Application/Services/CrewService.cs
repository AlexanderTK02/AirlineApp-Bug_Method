using Airline.Domain.Entities;
using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Application.Services;

public class CrewService
{
    private readonly ICrewMemberRepository _crewRepository;
    private readonly IFlightRepository _flightRepository;
    private int _nextId = 1;

    public CrewService(ICrewMemberRepository crewRepository, IFlightRepository flightRepository)
    {
        _crewRepository = crewRepository;
        _flightRepository = flightRepository;
    }

    // BUG_TARGET: AddCrewMember
    public CrewMember AddCrewMember(string firstName, string lastName, CrewRole role, string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.");
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number cannot be empty.");

        var crewMember = new CrewMember
        {
            Id = _nextId++,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            FlightId = 0,
            LicenseNumber = licenseNumber
        };

        _crewRepository.Add(crewMember);
        return crewMember;
    }

    // BUG_TARGET: AssignToFlight
    public void AssignToFlight(int crewMemberId, int flightId)
    {
        var crewMember = _crewRepository.GetById(crewMemberId);
        if (crewMember == null)
            throw new InvalidOperationException("Crew member not found.");

        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        if (crewMember.FlightId != 0)
            throw new InvalidOperationException("Crew member is already assigned to a flight.");

        crewMember.FlightId = flightId;
        _crewRepository.Update(crewMember);
    }

    // MISSING_TARGET: GetFlightCrew
    public List<CrewMember> GetFlightCrew(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        return _crewRepository.GetByFlightId(flightId);
    }

    // BUG_TARGET: GetAvailableCrew
    public List<CrewMember> GetAvailableCrew()
    {
        return _crewRepository.GetUnassigned();
    }

    // MISSING_TARGET: UnassignFromFlight
    public void UnassignFromFlight(int crewMemberId)
    {
        var crewMember = _crewRepository.GetById(crewMemberId);
        if (crewMember == null)
            throw new InvalidOperationException("Crew member not found.");
        if (crewMember.FlightId == 0)
            throw new InvalidOperationException("Crew member is not assigned to any flight.");

        crewMember.FlightId = 0;
        _crewRepository.Update(crewMember);
    }

    // BUG_TARGET: GetPilotCount
    public int GetPilotCount(int flightId)
    {
        return _crewRepository.GetByFlightId(flightId)
            .Count(c => c.Role == CrewRole.Pilot || c.Role == CrewRole.CoPilot);
    }

    // MISSING_TARGET: IsFlightFullyStaffed
    public bool IsFlightFullyStaffed(int flightId)
    {
        var flight = _flightRepository.GetById(flightId);
        if (flight == null)
            throw new InvalidOperationException("Flight not found.");

        var crew = _crewRepository.GetByFlightId(flightId);
        var hasPilot = crew.Any(c => c.Role == CrewRole.Pilot);
        var hasCoPilot = crew.Any(c => c.Role == CrewRole.CoPilot);
        var hasAttendant = crew.Any(c => c.Role == CrewRole.FlightAttendant);

        return hasPilot && hasCoPilot && hasAttendant;
    }

    // MISSING_TARGET: GetCrewMembersByRole
    public List<CrewMember> GetCrewMembersByRole(CrewRole role)
    {
        return _crewRepository.GetAll()
            .Where(c => c.Role == role)
            .ToList();
    }

    // BUG_TARGET: GetTotalCrewCount
    public int GetTotalCrewCount()
    {
        return _crewRepository.GetAll().Count;
    }

    // MISSING_TARGET: GetCrewScheduleReport
    public string GetCrewScheduleReport(int crewMemberId, DateTime from, DateTime to)
    {
        if (from > to)
            throw new ArgumentException("From date must be before to date.");

        var crewMember = _crewRepository.GetById(crewMemberId);
        if (crewMember == null)
            throw new InvalidOperationException("Crew member not found.");

        var flights = _flightRepository.GetByDateRange(from, to)
            .Where(f => _crewRepository.GetByFlightId(f.Id).Any(c => c.Id == crewMemberId))
            .OrderBy(f => f.DepartureTime)
            .ToList();

        var lines = new List<string>
        {
            $"{crewMember.FirstName} {crewMember.LastName} ({crewMember.Role}):"
        };

        foreach (var flight in flights)
            lines.Add($"  {flight.DepartureTime:yyyy-MM-dd HH:mm} {flight.DepartureAirport}→{flight.ArrivalAirport} ({flight.FlightNumber})");

        if (flights.Count == 0)
            lines.Add("  Inga tilldelade flygningar under perioden.");

        return string.Join("\n", lines);
    }
}
