using Airline.Domain.Entities;

namespace Airline.Domain.Interfaces;

public interface ICrewMemberRepository
{
    CrewMember? GetById(int id);
    List<CrewMember> GetAll();
    List<CrewMember> GetByFlightId(int flightId);
    List<CrewMember> GetUnassigned();
    void Add(CrewMember crewMember);
    void Update(CrewMember crewMember);
}
