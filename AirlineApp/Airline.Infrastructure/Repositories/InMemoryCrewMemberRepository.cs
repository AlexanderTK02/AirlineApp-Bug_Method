using Airline.Domain.Entities;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Repositories;

public class InMemoryCrewMemberRepository : ICrewMemberRepository
{
    private readonly List<CrewMember> _crewMembers = new();

    public CrewMember? GetById(int id) => _crewMembers.FirstOrDefault(c => c.Id == id);

    public List<CrewMember> GetAll() => _crewMembers.ToList();

    public List<CrewMember> GetByFlightId(int flightId) =>
        _crewMembers.Where(c => c.FlightId == flightId).ToList();

    // BUG_TARGET: GetUnassigned
    public List<CrewMember> GetUnassigned() =>
        _crewMembers.Where(c => c.FlightId == 0).ToList();

    public void Add(CrewMember crewMember) => _crewMembers.Add(crewMember);

    public void Update(CrewMember crewMember)
    {
        var index = _crewMembers.FindIndex(c => c.Id == crewMember.Id);
        if (index >= 0) _crewMembers[index] = crewMember;
    }
}
