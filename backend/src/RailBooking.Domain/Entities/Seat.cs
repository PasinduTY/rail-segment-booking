namespace RailBooking.Domain.Entities;

// Only ever created for a Coach where Type == CoachType.Reserved. Unreserved
// coaches are first-come-first-served with no seat assignment by definition,
// so there is nothing per-seat to model for them - this is enforced by
// simply never generating Seat rows for those coaches, not by a runtime check.
public class Seat
{
    public int Id { get; set; }
    public int CoachId { get; set; }
    public int SeatNumber { get; set; }
}
