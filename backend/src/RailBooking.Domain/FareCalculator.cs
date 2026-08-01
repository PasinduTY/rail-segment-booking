namespace RailBooking.Domain;

// Pure distance x rate calculation. The rate is a business/config parameter
// (configurable per CoachType, not hardcoded here) - this class only knows
// how to turn a distance and a rate into a fare, nothing about where the
// rate comes from.
public static class FareCalculator
{
    public static decimal CalculateFare(decimal distanceKm, decimal ratePerKm)
    {
        if (distanceKm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceKm), "Distance must be positive.");
        }

        if (ratePerKm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ratePerKm), "Rate must be positive.");
        }

        return distanceKm * ratePerKm;
    }
}
