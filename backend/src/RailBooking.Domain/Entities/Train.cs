namespace RailBooking.Domain.Entities;

// A named service (e.g. "Podi Menike") that defines a coach/seat layout
// template. Kept separate from TripDeparture so the same layout can run on
// many different calendar dates.
public class Train
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Coach> Coaches { get; set; } = new List<Coach>();
}
