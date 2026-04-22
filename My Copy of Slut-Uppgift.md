My Copy of Slut-Uppgift



\# Examination - Airline Application



\*\*Elev:\*\* Alexander Tamo Khasho

\*\*Datum:\*\* 2026-04-20

\*\*Domän:\*\* Airline



\## Instruktioner



Du har fått en Console-applikation i \*\*Onion Architecture\*\* som innehåller buggar och

saknade metodimplementationer. Din uppgift är att:



1\. \*\*Felsöka och fixa 8 buggar\*\* i Application- och Infrastructure-lagren

2\. \*\*Implementera 4 saknade metoder\*\* (markerade med `NotImplementedException`)



\## Regler



\- Du får \*\*INTE\*\* ändra namn på klasser, metoder, namespaces eller interfaces

\- Du får \*\*INTE\*\* lägga till nya projekt eller ändra projektstrukturen

\- Du får \*\*INTE\*\* ändra i testprojektet

\- Alla metodsignaturer måste vara exakt som de är (parametrar, returtyper)

\- Följ Onion Architecture-principerna (beroenden pekar inåt)



\## Projektstruktur



```

AirlineApp/

├── Airline.Domain/       # Entiteter, enums, interfaces (ÄNDRA INTE)

├── Airline.Application/  # Services - HÄR FIXAR DU BUGGAR \\\& IMPLEMENTERAR METODER

├── Airline.Infrastructure/ # Repositories \\\& factories - KAN INNEHÅLLA BUGGAR

└── Airline.Console/      # Consoleappen (valfritt att ändra för felsökning)



AirlineApp.Tests/          # Testprojektet - ÄNDRA INTE

```



\## Att fixa: Buggar \[8/8 bugs fixed]



1\. \*\*Bugg i **(FIXED**) \*\*`\*\*UpdateFlightStatus`\*\* - UpdateFlightStatus does not persist update (EF Core-like: forgot to call SaveChanges)



2\. \*\*Bugg i **(FIXED**) `CalculateRouteRevenue`\*\* - CalculateRouteRevenue includes cancelled bookings in the revenue total



3\. \*\*Bugg i (**FIXED**) `RegisterPassenger`\*\* - RegisterPassenger does not validate email format (missing @ check)



4\. \*\*Bugg i **(FIXED**) **`**ReschedulePassenger`\*\* - ReschedulePassenger does not cancel the old booking (sets Confirmed instead of Cancelled)



5\. \*\*Bugg i **(FIXED**) **`**CalculateOccupancyRate`\*\* - CalculateOccupancyRate counts available seats instead of occupied ones



6\. \*\*Bugg i **(FIXED**) **`**CalculatePrice`\*\* \[Infrastructure] - SeatPriceFactory divides by multiplier instead of multiplying



7\. \*\*Bugg i **(FIXED** **(DIDNT HAVE A TEST)) `**ReschedulePassenger`\*\* - ReschedulePassenger does not release the old seat (marks as unavailable instead of available)



8\. \*\*Bugg i (**FIXED**) `CreateFlight`\*\* - CreateFlight does not convert arrival airport to uppercase



==============================================================================================================



\## Att implementera: Saknade metoder \[4/4 methods implemented]



1\. **(IMPLEMENTED)** \*\***`**GetFlightsByDateRange`\*\* - Implement GetFlightsByDateRange that returns flights within a date range



2\. **(IMPLEMENTED)** \*\***`**UpgradeSeat`\*\* - Implement UpgradeSeat that upgrades a booking to a higher class and returns price difference



3\. (**IMPLEMENTED**) \*\*`GetTotalBookingCount`\*\* - Implement GetTotalBookingCount that returns the total number of bookings



4\. (**IMPLEMENTED**) \*\*`CancelFlight`\*\* - Implement CancelFlight that cancels a scheduled or delayed flight



\## Verifiering



När du är klar, kör testerna:



```bash

dotnet test AirlineApp.Tests/Airline.Tests.csproj

```



\*\*Alla tester gröna = Godkänd uppgift!\*\*

