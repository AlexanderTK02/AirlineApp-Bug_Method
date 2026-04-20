using Airline.Domain.Entities;
using Airline.Domain.Enums;

namespace Airline.Domain.Interfaces;

public interface ISeatPriceFactory
{
    decimal CalculatePrice(decimal basePrice, SeatClass seatClass);
}
