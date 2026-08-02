namespace RailBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RailBooking.Api.Contracts;
using RailBooking.Api.Data;
using RailBooking.Api.Options;
using RailBooking.Domain;
using RailBooking.Domain.Entities;

[ApiController]
[Route("api/trip-departures")]
public class TripDeparturesController(RailBookingDbContext db, IOptions<FareRatesOptions> fareRates) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TripDepartureDto>>> GetTripDepartures(
        [FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var departures = await (
            from td in db.TripDepartures
            join train in db.Trains on td.TrainId equals train.Id
            where td.ServiceDate == targetDate
            select new TripDepartureDto(td.Id, train.Id, train.Name, td.ServiceDate)
        ).ToListAsync(cancellationToken);

        return Ok(departures);
    }

    // Availability is always for a specific (TripDeparture, origin, destination) leg - the
    // same seat can be available for one leg and taken for another on the same trip, so
    // "availability" only makes sense scoped to a requested journey, not to a seat alone.
    [HttpGet("{id:int}/availability")]
    public async Task<ActionResult<AvailabilityResponseDto>> GetAvailability(
        int id,
        [FromQuery] int originStationId,
        [FromQuery] int destinationStationId,
        CancellationToken cancellationToken)
    {
        var tripDeparture = await db.TripDepartures.FindAsync([id], cancellationToken);
        if (tripDeparture is null)
        {
            return NotFound($"Trip departure {id} not found.");
        }

        var originStation = await db.Stations.FindAsync([originStationId], cancellationToken);
        var destinationStation = await db.Stations.FindAsync([destinationStationId], cancellationToken);
        if (originStation is null || destinationStation is null)
        {
            return BadRequest("Origin or destination station not found.");
        }

        Segment requestedSegment;
        try
        {
            requestedSegment = new Segment(originStation.SequenceNumber, destinationStation.SequenceNumber);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        // Only Reserved coaches have per-seat booking (see Seat.cs) - Unreserved coaches
        // are intentionally not represented here.
        var reservedSeats = await (
            from seat in db.Seats
            join coach in db.Coaches on seat.CoachId equals coach.Id
            where coach.TrainId == tripDeparture.TrainId && coach.Type == CoachType.Reserved
            select new { seat.Id, seat.SeatNumber, coach.CoachNumber }
        ).ToListAsync(cancellationToken);

        var seatIds = reservedSeats.Select(s => s.Id).ToList();

        // This is a convenience read for the UI, not the correctness guarantee - the
        // Postgres exclusion constraint is what actually prevents a double-booking even if
        // two requests race past this check at the same time (see Booking POST below).
        var existingSegmentsBySeat = (await db.Bookings
                .Where(b => b.TripDepartureId == id
                    && b.Status == BookingStatus.Confirmed
                    && seatIds.Contains(b.SeatId))
                .Select(b => new { b.SeatId, b.OriginSequence, b.DestinationSequence })
                .ToListAsync(cancellationToken))
            .GroupBy(b => b.SeatId)
            .ToDictionary(g => g.Key, g => g.Select(b => new Segment(b.OriginSequence, b.DestinationSequence)).ToList());

        var fare = FareCalculator.CalculateFare(
            destinationStation.DistanceKm - originStation.DistanceKm,
            fareRates.Value.RateFor(CoachType.Reserved));

        var seatAvailability = reservedSeats
            .Select(seat =>
            {
                var isAvailable = !existingSegmentsBySeat.TryGetValue(seat.Id, out var bookedSegments)
                    || !bookedSegments.Any(requestedSegment.OverlapsWith);

                return new SeatAvailabilityDto(seat.Id, seat.CoachNumber, seat.SeatNumber, isAvailable, fare);
            })
            .OrderBy(s => s.CoachNumber)
            .ThenBy(s => s.SeatNumber)
            .ToList();

        return Ok(new AvailabilityResponseDto(id, originStationId, destinationStationId, seatAvailability));
    }
}
