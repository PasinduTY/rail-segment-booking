namespace RailBooking.Api.Data;

using RailBooking.Domain.Entities;

// Static reference data only - the route topology and coach/seat layout.
// This does not change with time, so it is safe to bake into a versioned
// EF Core migration via HasData. Calendar-specific data (TripDeparture)
// is deliberately NOT here - see DbSeeder for why.
public static class SeedData
{
    // Approximate distances (km) from Colombo Fort along the upcountry line.
    public static Station[] GetStations() =>
    [
        new() { Id = 1, Name = "Colombo Fort", SequenceNumber = 0, DistanceKm = 0m },
        new() { Id = 2, Name = "Rambukkana", SequenceNumber = 1, DistanceKm = 66m },
        new() { Id = 3, Name = "Kandy", SequenceNumber = 2, DistanceKm = 121m },
        new() { Id = 4, Name = "Peradeniya", SequenceNumber = 3, DistanceKm = 127m },
        new() { Id = 5, Name = "Hatton", SequenceNumber = 4, DistanceKm = 168m },
        new() { Id = 6, Name = "Nanu Oya", SequenceNumber = 5, DistanceKm = 186m },
        new() { Id = 7, Name = "Haputale", SequenceNumber = 6, DistanceKm = 194m },
        new() { Id = 8, Name = "Bandarawela", SequenceNumber = 7, DistanceKm = 204m },
        new() { Id = 9, Name = "Ella", SequenceNumber = 8, DistanceKm = 216m },
        new() { Id = 10, Name = "Badulla", SequenceNumber = 9, DistanceKm = 230m },
    ];

    public static Train[] GetTrains() =>
    [
        new() { Id = 1, Name = "Podi Menike" },
    ];

    // 3 reserved + 5 unreserved, per the assignment's description of the
    // line. Reserved coaches get a modest seat count (10) to keep the demo
    // seat grid easy to look at; Unreserved coaches carry only a capacity
    // figure since there is no per-seat booking for them (see Seat.cs).
    public static Coach[] GetCoaches() =>
    [
        new() { Id = 1, TrainId = 1, CoachNumber = 1, Type = CoachType.Reserved, SeatCount = 10 },
        new() { Id = 2, TrainId = 1, CoachNumber = 2, Type = CoachType.Reserved, SeatCount = 10 },
        new() { Id = 3, TrainId = 1, CoachNumber = 3, Type = CoachType.Reserved, SeatCount = 10 },
        new() { Id = 4, TrainId = 1, CoachNumber = 4, Type = CoachType.Unreserved, SeatCount = 80 },
        new() { Id = 5, TrainId = 1, CoachNumber = 5, Type = CoachType.Unreserved, SeatCount = 80 },
        new() { Id = 6, TrainId = 1, CoachNumber = 6, Type = CoachType.Unreserved, SeatCount = 80 },
        new() { Id = 7, TrainId = 1, CoachNumber = 7, Type = CoachType.Unreserved, SeatCount = 80 },
        new() { Id = 8, TrainId = 1, CoachNumber = 8, Type = CoachType.Unreserved, SeatCount = 80 },
    ];

    // 10 seats in each of the 3 reserved coaches (ids 1-3) - nothing for the
    // 5 unreserved coaches.
    public static Seat[] GetSeats()
    {
        var seats = new List<Seat>();
        var nextId = 1;

        for (var coachId = 1; coachId <= 3; coachId++)
        {
            for (var seatNumber = 1; seatNumber <= 10; seatNumber++)
            {
                seats.Add(new Seat { Id = nextId++, CoachId = coachId, SeatNumber = seatNumber });
            }
        }

        return [.. seats];
    }
}
