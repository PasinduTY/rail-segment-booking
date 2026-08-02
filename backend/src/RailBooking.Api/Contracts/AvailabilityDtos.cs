namespace RailBooking.Api.Contracts;

public record SeatAvailabilityDto(int SeatId, int CoachNumber, int SeatNumber, bool IsAvailable, decimal Fare);

public record AvailabilityResponseDto(
    int TripDepartureId,
    int OriginStationId,
    int DestinationStationId,
    IReadOnlyList<SeatAvailabilityDto> Seats);
