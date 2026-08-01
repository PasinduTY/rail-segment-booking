namespace RailBooking.Domain.Entities;

public class Coach
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public int CoachNumber { get; set; }
    public CoachType Type { get; set; }

    // Configured capacity. For Reserved coaches this drives how many Seat
    // rows get generated (Seat is the actual source of truth for booking).
    // For Unreserved coaches there are no Seat rows at all, so this is the
    // only capacity figure that exists for them (useful later for admin/
    // occupancy reporting, out of scope for booking itself).
    public int SeatCount { get; set; }

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
