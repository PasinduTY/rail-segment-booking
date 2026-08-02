namespace RailBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using RailBooking.Api.Contracts;
using RailBooking.Api.Data;
using RailBooking.Api.Options;
using RailBooking.Domain;
using RailBooking.Domain.Entities;

[ApiController]
[Route("api/bookings")]
public class BookingsController(RailBookingDbContext db, IOptions<FareRatesOptions> fareRates) : ControllerBase
{
    // Postgres error code for "exclusion_violation" - raised by the
    // CK_Bookings_NoOverlappingSegments constraint (see the
    // AddBookingSegmentExclusionConstraint migration). This is the actual
    // correctness guarantee; everything above it in this method is
    // validation and a best-effort UX check, not the source of truth.
    private const string ExclusionViolationSqlState = "23P01";

    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(
        [FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PassengerName))
        {
            return BadRequest("Passenger name is required.");
        }

        var tripDeparture = await db.TripDepartures.FindAsync([request.TripDepartureId], cancellationToken);
        if (tripDeparture is null)
        {
            return NotFound($"Trip departure {request.TripDepartureId} not found.");
        }

        var seatInfo = await (
            from seat in db.Seats
            join coach in db.Coaches on seat.CoachId equals coach.Id
            where seat.Id == request.SeatId
            select new { seat.Id, seat.SeatNumber, coach.CoachNumber, coach.Type, coach.TrainId }
        ).SingleOrDefaultAsync(cancellationToken);

        if (seatInfo is null)
        {
            return BadRequest($"Seat {request.SeatId} not found.");
        }

        if (seatInfo.Type != CoachType.Reserved)
        {
            return BadRequest("Only seats in reserved coaches can be booked.");
        }

        if (seatInfo.TrainId != tripDeparture.TrainId)
        {
            return BadRequest("Seat does not belong to the train operating this trip departure.");
        }

        var originStation = await db.Stations.FindAsync([request.OriginStationId], cancellationToken);
        var destinationStation = await db.Stations.FindAsync([request.DestinationStationId], cancellationToken);
        if (originStation is null || destinationStation is null)
        {
            return BadRequest("Origin or destination station not found.");
        }

        Segment segment;
        try
        {
            segment = new Segment(originStation.SequenceNumber, destinationStation.SequenceNumber);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var fare = FareCalculator.CalculateFare(
            destinationStation.DistanceKm - originStation.DistanceKm,
            fareRates.Value.RateFor(seatInfo.Type));

        var booking = new Booking
        {
            SeatId = request.SeatId,
            TripDepartureId = request.TripDepartureId,
            OriginStationId = request.OriginStationId,
            DestinationStationId = request.DestinationStationId,
            OriginSequence = segment.OriginSequence,
            DestinationSequence = segment.DestinationSequence,
            PassengerName = request.PassengerName.Trim(),
            FareAmount = fare,
            Status = BookingStatus.Confirmed,
            CreatedAtUtc = DateTime.UtcNow,
        };

        db.Bookings.Add(booking);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: ExclusionViolationSqlState })
        {
            return Conflict("This seat is no longer available for the requested leg. Please choose another seat.");
        }

        var dto = new BookingDto(
            booking.Id, seatInfo.Id, seatInfo.CoachNumber, seatInfo.SeatNumber,
            booking.TripDepartureId, tripDeparture.ServiceDate,
            originStation.Name, destinationStation.Name,
            booking.PassengerName, booking.FareAmount, booking.Status.ToString(), booking.CreatedAtUtc);

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id, CancellationToken cancellationToken)
    {
        // Project scalars only (no .ToString() on the enum here) - whether a provider
        // can translate an enum-to-string call inside the SQL projection is provider-
        // and version-dependent, so the status is formatted after materialization instead.
        var row = await (
            from b in db.Bookings
            join seat in db.Seats on b.SeatId equals seat.Id
            join coach in db.Coaches on seat.CoachId equals coach.Id
            join origin in db.Stations on b.OriginStationId equals origin.Id
            join destination in db.Stations on b.DestinationStationId equals destination.Id
            join trip in db.TripDepartures on b.TripDepartureId equals trip.Id
            where b.Id == id
            select new
            {
                b.Id,
                SeatId = seat.Id,
                coach.CoachNumber,
                seat.SeatNumber,
                b.TripDepartureId,
                trip.ServiceDate,
                OriginStationName = origin.Name,
                DestinationStationName = destination.Name,
                b.PassengerName,
                b.FareAmount,
                b.Status,
                b.CreatedAtUtc,
            }
        ).SingleOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return NotFound();
        }

        var dto = new BookingDto(
            row.Id, row.SeatId, row.CoachNumber, row.SeatNumber,
            row.TripDepartureId, row.ServiceDate,
            row.OriginStationName, row.DestinationStationName,
            row.PassengerName, row.FareAmount, row.Status.ToString(), row.CreatedAtUtc);

        return Ok(dto);
    }
}
