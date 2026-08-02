namespace RailBooking.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailBooking.Api.Contracts;
using RailBooking.Api.Data;

[ApiController]
[Route("api/stations")]
public class StationsController(RailBookingDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StationDto>>> GetStations(CancellationToken cancellationToken)
    {
        var stations = await db.Stations
            .OrderBy(s => s.SequenceNumber)
            .Select(s => new StationDto(s.Id, s.Name, s.SequenceNumber, s.DistanceKm))
            .ToListAsync(cancellationToken);

        return Ok(stations);
    }
}
