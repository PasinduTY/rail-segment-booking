namespace RailBooking.Domain.Tests;

using RailBooking.Domain;
using Xunit;

public class FareCalculatorTests
{
    [Theory]
    [InlineData(100, 2.5, 250)]
    [InlineData(50.5, 1, 50.5)]
    public void CalculateFare_MultipliesDistanceByRate(decimal distanceKm, decimal ratePerKm, decimal expected)
    {
        Assert.Equal(expected, FareCalculator.CalculateFare(distanceKm, ratePerKm));
    }

    [Fact]
    public void CalculateFare_Throws_WhenDistanceNotPositive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FareCalculator.CalculateFare(0, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => FareCalculator.CalculateFare(-5, 2));
    }

    [Fact]
    public void CalculateFare_Throws_WhenRateNotPositive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FareCalculator.CalculateFare(10, 0));
    }
}
