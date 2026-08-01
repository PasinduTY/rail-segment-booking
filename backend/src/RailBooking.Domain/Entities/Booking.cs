namespace RailBooking.Domain.Entities;

using RailBooking.Domain;

public class Booking
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public int TripDepartureId { get; set; }
    public int OriginStationId { get; set; }
    public int DestinationStationId { get; set; }

    // Denormalized copies of Station.SequenceNumber at booking time. The
    // Postgres exclusion constraint that guarantees no two confirmed
    // bookings on the same seat+trip overlap works on a range column
    // physically present on this row - it cannot reach through a join to
    // Station - so these two ints (mapped as a Postgres int4range) are what
    // the database actually enforces the "no overlap" guarantee on.
    public int OriginSequence { get; set; }
    public int DestinationSequence { get; set; }

    public string PassengerName { get; set; } = string.Empty;
    public decimal FareAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public DateTime CreatedAtUtc { get; set; }

    public Segment Segment => new(OriginSequence, DestinationSequence);
}
