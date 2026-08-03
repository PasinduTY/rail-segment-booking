// Mirrors the backend's Contracts DTOs (backend/src/RailBooking.Api/Contracts).
// Kept as plain interfaces, hand-written rather than generated, since the
// API surface is small and stable enough that a codegen step would be
// more ceremony than it's worth here.

export interface Station {
  id: number;
  name: string;
  sequenceNumber: number;
  distanceKm: number;
}

export interface TripDeparture {
  id: number;
  trainId: number;
  trainName: string;
  serviceDate: string; // ISO date, e.g. "2026-08-05"
}

export interface SeatAvailability {
  seatId: number;
  coachNumber: number;
  seatNumber: number;
  isAvailable: boolean;
  fare: number;
}

export interface AvailabilityResponse {
  tripDepartureId: number;
  originStationId: number;
  destinationStationId: number;
  seats: SeatAvailability[];
}

export interface CreateBookingRequest {
  tripDepartureId: number;
  seatId: number;
  originStationId: number;
  destinationStationId: number;
  passengerName: string;
}

export interface Booking {
  id: number;
  seatId: number;
  coachNumber: number;
  seatNumber: number;
  tripDepartureId: number;
  serviceDate: string;
  originStationName: string;
  destinationStationName: string;
  passengerName: string;
  fareAmount: number;
  status: string;
  createdAtUtc: string;
}
