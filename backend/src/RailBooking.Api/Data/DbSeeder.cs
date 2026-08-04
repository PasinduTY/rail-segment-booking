namespace RailBooking.Api.Data;

using Microsoft.EntityFrameworkCore;
using RailBooking.Domain.Entities;

// Seeds TripDeparture rows for a rolling window of upcoming dates. Unlike
// the static reference data in SeedData (route/coach/seat layout), calendar
// dates can't live in a HasData migration - those values get baked into the
// migration as literal C# at the moment `dotnet ef migrations add` runs, so
// a hardcoded "seed August 1st" would just go stale. Running this at
// startup instead keeps the demo working no matter when the app is started.
public static class DbSeeder
{
    // Mirrored on the frontend as BOOKING_HORIZON_DAYS (frontend/src/App.tsx)
    // so the date picker's max bound matches what's actually seeded, instead
    // of letting a user pick a date this far out and just see an
    // unexplained "no departures" message. If one changes, check the other.
    private const int RollingWindowDays = 14;

    public static async Task SeedUpcomingTripDeparturesAsync(
        RailBookingDbContext db, CancellationToken cancellationToken = default)
    {
        var trainIds = await db.Trains.Select(t => t.Id).ToListAsync(cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var trainId in trainIds)
        {
            var existingDates = (await db.TripDepartures
                .Where(td => td.TrainId == trainId)
                .Select(td => td.ServiceDate)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            for (var offset = 0; offset < RollingWindowDays; offset++)
            {
                var date = today.AddDays(offset);
                if (!existingDates.Contains(date))
                {
                    db.TripDepartures.Add(new TripDeparture { TrainId = trainId, ServiceDate = date });
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
