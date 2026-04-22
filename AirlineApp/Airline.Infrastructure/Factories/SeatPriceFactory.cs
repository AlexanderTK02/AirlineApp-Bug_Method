using Airline.Domain.Enums;
using Airline.Domain.Interfaces;

namespace Airline.Infrastructure.Factories;

public class SeatPriceFactory : ISeatPriceFactory
{
    // BUG_TARGET: CalculatePrice |||*FIXED*|||
    public decimal CalculatePrice(decimal basePrice, SeatClass seatClass)
    {
        return Math.Round(basePrice * seatClass.GetPriceMultiplier(), 2); // <== Bug was right here //
    }
}
