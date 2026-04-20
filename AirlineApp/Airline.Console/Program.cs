using Airline.Application.Services;
using Airline.Domain.Enums;
using Airline.Infrastructure.Factories;
using Airline.Infrastructure.Repositories;

namespace Airline.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var flightRepo = new InMemoryFlightRepository();
        var passengerRepo = new InMemoryPassengerRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var seatRepo = new InMemorySeatRepository();
        var crewRepo = new InMemoryCrewMemberRepository();

        var seatPriceFactory = new SeatPriceFactory();

        var flightService = new FlightService(flightRepo);
        var passengerService = new PassengerService(passengerRepo, bookingRepo);
        var bookingService = new BookingService(bookingRepo, flightRepo, seatRepo, passengerRepo, seatPriceFactory);
        var seatService = new SeatService(seatRepo, flightRepo, seatPriceFactory);
        var crewService = new CrewService(crewRepo, flightRepo);

        bool running = true;

        while (running)
        {
            System.Console.WriteLine("\n=== Flygbolaget ===");
            System.Console.WriteLine("1. Registrera passagerare");
            System.Console.WriteLine("2. Skapa flyg");
            System.Console.WriteLine("3. Lägg till säten på flyg");
            System.Console.WriteLine("4. Boka biljett");
            System.Console.WriteLine("5. Checka in passagerare");
            System.Console.WriteLine("6. Visa tillgängliga säten");
            System.Console.WriteLine("7. Lägg till besättningsmedlem");
            System.Console.WriteLine("8. Tilldela besättning till flyg");
            System.Console.WriteLine("9. Visa flyginformation");
            System.Console.WriteLine("10. Avsluta");
            System.Console.Write("\nVälj alternativ: ");

            var choice = System.Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterPassenger(passengerService);
                    break;
                case "2":
                    CreateFlight(flightService);
                    break;
                case "3":
                    AddSeats(seatService, flightService);
                    break;
                case "4":
                    BookTicket(bookingService, flightService, passengerService, seatService);
                    break;
                case "5":
                    CheckIn(bookingService);
                    break;
                case "6":
                    ShowAvailableSeats(seatService, flightService);
                    break;
                case "7":
                    AddCrewMember(crewService);
                    break;
                case "8":
                    AssignCrew(crewService, flightService);
                    break;
                case "9":
                    ShowFlightInfo(flightService, seatService, crewService);
                    break;
                case "10":
                    running = false;
                    System.Console.WriteLine("Avslutar...");
                    break;
                default:
                    System.Console.WriteLine("Ogiltigt val.");
                    break;
            }
        }
    }

    static void RegisterPassenger(PassengerService passengerService)
    {
        System.Console.Write("Förnamn: ");
        var firstName = System.Console.ReadLine() ?? "";
        System.Console.Write("Efternamn: ");
        var lastName = System.Console.ReadLine() ?? "";
        System.Console.Write("Passnummer: ");
        var passport = System.Console.ReadLine() ?? "";
        System.Console.Write("E-post: ");
        var email = System.Console.ReadLine() ?? "";
        System.Console.Write("Telefon: ");
        var phone = System.Console.ReadLine() ?? "";

        try
        {
            var passenger = passengerService.RegisterPassenger(firstName, lastName, passport, email, phone);
            System.Console.WriteLine($"Passagerare registrerad: {passenger.FullName} (ID: {passenger.Id})");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void CreateFlight(FlightService flightService)
    {
        System.Console.Write("Flygnummer (t.ex. SK123): ");
        var flightNumber = System.Console.ReadLine() ?? "";
        System.Console.Write("Avgångsflygplats (IATA-kod): ");
        var departure = System.Console.ReadLine() ?? "";
        System.Console.Write("Ankomstflygplats (IATA-kod): ");
        var arrival = System.Console.ReadLine() ?? "";
        System.Console.Write("Avgångstid (yyyy-MM-dd HH:mm): ");
        var depTime = DateTime.Parse(System.Console.ReadLine() ?? "");
        System.Console.Write("Ankomsttid (yyyy-MM-dd HH:mm): ");
        var arrTime = DateTime.Parse(System.Console.ReadLine() ?? "");
        System.Console.Write("Flygplanstyp (0=Narrow, 1=Wide, 2=Regional): ");
        var aircraftType = (AircraftType)int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Antal platser totalt: ");
        var totalSeats = int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            var flight = flightService.CreateFlight(flightNumber, departure, arrival, depTime, arrTime, aircraftType, totalSeats);
            System.Console.WriteLine($"Flyg skapat: {flight.FlightNumber} {flight.DepartureAirport} → {flight.ArrivalAirport} (ID: {flight.Id})");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void AddSeats(SeatService seatService, FlightService flightService)
    {
        System.Console.Write("Flyg-ID: ");
        var flightId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Antal säten att lägga till: ");
        var count = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Klass (0=Economy, 1=Business, 2=First): ");
        var seatClass = (SeatClass)int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Baspris: ");
        var basePrice = decimal.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Radprefix (t.ex. A): ");
        var prefix = System.Console.ReadLine() ?? "A";

        try
        {
            for (int i = 1; i <= count; i++)
            {
                seatService.AddSeat(flightId, $"{prefix}{i}", seatClass, basePrice);
            }
            System.Console.WriteLine($"{count} säten tillagda på flyg {flightId}.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void BookTicket(BookingService bookingService, FlightService flightService,
        PassengerService passengerService, SeatService seatService)
    {
        System.Console.Write("Passagerar-ID: ");
        var passengerId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Flyg-ID: ");
        var flightId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Sätenummer: ");
        var seatNumber = System.Console.ReadLine() ?? "";
        System.Console.Write("Klass (0=Economy, 1=Business, 2=First): ");
        var seatClass = (SeatClass)int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            var booking = bookingService.CreateBooking(passengerId, flightId, seatNumber, seatClass);
            System.Console.WriteLine($"Bokning skapad: ID {booking.Id}, Pris: {booking.Price:C}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void CheckIn(BookingService bookingService)
    {
        System.Console.Write("Boknings-ID: ");
        var bookingId = int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            bookingService.CheckIn(bookingId);
            System.Console.WriteLine("Incheckning klar!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void ShowAvailableSeats(SeatService seatService, FlightService flightService)
    {
        System.Console.Write("Flyg-ID: ");
        var flightId = int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            var seats = seatService.GetAvailableSeats(flightId);
            System.Console.WriteLine($"Lediga säten på flyg {flightId}:");
            foreach (var seat in seats)
            {
                System.Console.WriteLine($"  {seat.SeatNumber} | {seat.SeatClass} | {seat.BasePrice:C}");
            }
            System.Console.WriteLine($"Totalt: {seats.Count} lediga säten");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void AddCrewMember(CrewService crewService)
    {
        System.Console.Write("Förnamn: ");
        var firstName = System.Console.ReadLine() ?? "";
        System.Console.Write("Efternamn: ");
        var lastName = System.Console.ReadLine() ?? "";
        System.Console.Write("Roll (0=Pilot, 1=CoPilot, 2=FlightAttendant, 3=FlightEngineer, 4=Purser): ");
        var role = (CrewRole)int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Licensnummer: ");
        var license = System.Console.ReadLine() ?? "";

        try
        {
            var crew = crewService.AddCrewMember(firstName, lastName, role, license);
            System.Console.WriteLine($"Besättningsmedlem tillagd: {crew.FirstName} {crew.LastName} (ID: {crew.Id})");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void AssignCrew(CrewService crewService, FlightService flightService)
    {
        System.Console.Write("Besättningsmedlem-ID: ");
        var crewId = int.Parse(System.Console.ReadLine() ?? "0");
        System.Console.Write("Flyg-ID: ");
        var flightId = int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            crewService.AssignToFlight(crewId, flightId);
            System.Console.WriteLine("Besättningsmedlem tilldelad flyget.");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }

    static void ShowFlightInfo(FlightService flightService, SeatService seatService, CrewService crewService)
    {
        System.Console.Write("Flyg-ID: ");
        var flightId = int.Parse(System.Console.ReadLine() ?? "0");

        try
        {
            var flight = flightService.GetFlightById(flightId);
            if (flight == null)
            {
                System.Console.WriteLine("Flyg hittades inte.");
                return;
            }

            System.Console.WriteLine($"\n--- Flyg {flight.FlightNumber} ---");
            System.Console.WriteLine($"Rutt: {flight.DepartureAirport} → {flight.ArrivalAirport}");
            System.Console.WriteLine($"Avgång: {flight.DepartureTime}");
            System.Console.WriteLine($"Ankomst: {flight.ArrivalTime}");
            System.Console.WriteLine($"Status: {flight.Status}");
            System.Console.WriteLine($"Flygplanstyp: {flight.AircraftType}");

            var duration = flightService.CalculateFlightDuration(flightId);
            System.Console.WriteLine($"Restid: {duration} timmar");

            var available = seatService.GetAvailableSeatCount(flightId);
            var total = seatService.GetTotalSeatCount(flightId);
            var occupancy = seatService.CalculateOccupancyRate(flightId);
            System.Console.WriteLine($"Platser: {available}/{total} lediga ({occupancy}% beläggning)");

            var fullyStaffed = crewService.IsFlightFullyStaffed(flightId);
            System.Console.WriteLine($"Besättning komplett: {(fullyStaffed ? "Ja" : "Nej")}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fel: {ex.Message}");
        }
    }
}
