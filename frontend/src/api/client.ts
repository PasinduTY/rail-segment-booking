import type {
  AvailabilityResponse,
  Booking,
  CreateBookingRequest,
  Station,
  TripDeparture,
} from "./types";

// The API's origin is configuration, not a hardcoded constant, so the same
// build works against local dev (`dotnet run`) and the docker-compose
// service without a code change - see .env.example.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080";

// Carries the HTTP status code through so callers can distinguish, e.g.,
// a 409 (seat just taken - recoverable, show a friendly message and
// refresh availability) from a 400/500 (something actually wrong).
export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
  });

  if (!response.ok) {
    const message = (await response.text()) || response.statusText;
    throw new ApiError(response.status, message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const getStations = () => request<Station[]>("/api/stations");

export const getTripDepartures = (date: string) =>
  request<TripDeparture[]>(`/api/trip-departures?date=${encodeURIComponent(date)}`);

export const getAvailability = (
  tripDepartureId: number,
  originStationId: number,
  destinationStationId: number,
  signal?: AbortSignal,
) =>
  request<AvailabilityResponse>(
    `/api/trip-departures/${tripDepartureId}/availability` +
      `?originStationId=${originStationId}&destinationStationId=${destinationStationId}`,
    { signal },
  );

export const createBooking = (payload: CreateBookingRequest) =>
  request<Booking>("/api/bookings", {
    method: "POST",
    body: JSON.stringify(payload),
  });
