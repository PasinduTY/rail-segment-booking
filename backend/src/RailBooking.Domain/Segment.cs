namespace RailBooking.Domain;

// A half-open [OriginSequence, DestinationSequence) range over station
// SequenceNumbers. Half-open so that adjacent legs - one ending at a station,
// the next starting at that same station - correctly do NOT count as
// overlapping. This overlap rule is written to mirror exactly the range
// semantics of the Postgres "&&" operator used by the EXCLUDE USING gist
// constraint enforced on the Booking table, so the application-level
// availability check and the database-level guarantee can never disagree.
public readonly record struct Segment
{
    public int OriginSequence { get; }
    public int DestinationSequence { get; }

    public Segment(int originSequence, int destinationSequence)
    {
        if (destinationSequence <= originSequence)
        {
            throw new ArgumentException(
                "Destination sequence must come after origin sequence.",
                nameof(destinationSequence));
        }

        OriginSequence = originSequence;
        DestinationSequence = destinationSequence;
    }

    public bool OverlapsWith(Segment other) =>
        OriginSequence < other.DestinationSequence && other.OriginSequence < DestinationSequence;
}
