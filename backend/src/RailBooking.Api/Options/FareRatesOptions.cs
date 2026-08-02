namespace RailBooking.Api.Options;

using RailBooking.Domain.Entities;

public class FareRatesOptions
{
    public const string SectionName = "FareRates";

    public decimal ReservedPerKm { get; set; }
    public decimal UnreservedPerKm { get; set; }

    public decimal RateFor(CoachType type) => type switch
    {
        CoachType.Reserved => ReservedPerKm,
        CoachType.Unreserved => UnreservedPerKm,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown coach type."),
    };
}
