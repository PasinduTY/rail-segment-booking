namespace RailBooking.Api.Contracts;

public record TripDepartureDto(int Id, int TrainId, string TrainName, DateOnly ServiceDate);
