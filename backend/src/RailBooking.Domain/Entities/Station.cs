namespace RailBooking.Domain.Entities;

// Ordered, data-driven stop on the route. Adding a station to extend the
// route (or splice one into the middle) is a data change, not a code change.
public class Station
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Position along the route, 0-based from the first station. Used for
    // segment overlap math (see Segment) - never for fares.
    public int SequenceNumber { get; set; }

    // Cumulative distance from the first station, used only for fare
    // calculation. Kept separate from SequenceNumber because overlap
    // detection and fare calculation are different jobs with different
    // units - one cares about ordering, the other about real-world distance.
    public decimal DistanceKm { get; set; }
}
