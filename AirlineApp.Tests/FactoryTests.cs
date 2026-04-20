// Examination: Alexander Tamo Khasho
// Generated: 2026-04-02
// Domain: Airline

using Airline.Domain.Enums;
using Airline.Infrastructure.Factories;

namespace Airline.Tests;

public class FactoryTests
{
    [Fact]
    public void CalculatePrice_Economy_ReturnsBasePrice()
    {
        var factory = new SeatPriceFactory();
        var price = factory.CalculatePrice(1000m, SeatClass.Economy);
        Assert.Equal(1000m, price);
    }

    [Fact]
    public void CalculatePrice_Business_Returns2Point5xBase()
    {
        var factory = new SeatPriceFactory();
        var price = factory.CalculatePrice(1000m, SeatClass.Business);
        Assert.Equal(2500m, price);
    }

    [Fact]
    public void CalculatePrice_First_Returns5xBase()
    {
        var factory = new SeatPriceFactory();
        var price = factory.CalculatePrice(1000m, SeatClass.First);
        Assert.Equal(5000m, price);
    }

    [Fact]
    public void CalculatePrice_RoundsToTwoDecimals()
    {
        var factory = new SeatPriceFactory();
        var price = factory.CalculatePrice(333.33m, SeatClass.Business);
        Assert.Equal(833.32m, price); // 333.33 * 2.5 = 833.325, banker's rounding to 833.32
    }
}
