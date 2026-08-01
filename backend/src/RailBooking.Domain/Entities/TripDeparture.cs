namespace RailBooking.Domain.Entities;

// A specific calendar-date run of a Train. Booking/availability is always
// scoped to (Seat, TripDeparture), never to Seat alone - otherwise a seat
// booked Colombo Fort -> Kandy "today" would incorrectly stay blocked for
// every future day's train too.
public class TripDeparture
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public DateOnly ServiceDate { get; set; }
}
