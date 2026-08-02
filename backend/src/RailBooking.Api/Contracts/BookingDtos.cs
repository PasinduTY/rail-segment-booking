namespace RailBooking.Api.Contracts;

using System.ComponentModel.DataAnnotations;

public record CreateBookingRequest(
    int TripDepartureId,
    int SeatId,
    int OriginStationId,
    int DestinationStationId,
    [Required, MaxLength(200)] string PassengerName);

public record BookingDto(
    int Id,
    int SeatId,
    int CoachNumber,
    int SeatNumber,
    int TripDepartureId,
    DateOnly ServiceDate,
    string OriginStationName,
    string DestinationStationName,
    string PassengerName,
    decimal FareAmount,
    string Status,
    DateTime CreatedAtUtc);
