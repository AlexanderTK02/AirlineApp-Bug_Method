namespace Airline.Domain.Enums;

public enum SeatClass
{
    Economy,
    Business,
    First
}

public static class SeatClassExtensions
{
    public static decimal GetPriceMultiplier(this SeatClass seatClass) => seatClass switch
    {
        SeatClass.Economy => 1.0m,
        SeatClass.Business => 2.5m,
        SeatClass.First => 5.0m,
        _ => throw new ArgumentOutOfRangeException(nameof(seatClass))
    };
}
