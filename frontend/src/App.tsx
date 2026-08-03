import { useEffect, useState } from "react";
import "./App.css";
import { JourneySelector } from "./components/JourneySelector";
import { SeatGrid } from "./components/SeatGrid";
import { BookingPanel } from "./components/BookingPanel";
import {
  ApiError,
  createBooking,
  getAvailability,
  getStations,
  getTripDepartures,
} from "./api/client";
import type { AvailabilityResponse, Booking, Station, TripDeparture } from "./api/types";

function todayIso(): string {
  const now = new Date();
  const yyyy = now.getFullYear();
  const mm = String(now.getMonth() + 1).padStart(2, "0");
  const dd = String(now.getDate()).padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

function describeError(err: unknown): string {
  if (err instanceof ApiError) return err.message;
  if (err instanceof Error) return err.message;
  return "Something went wrong.";
}

function App() {
  const [stations, setStations] = useState<Station[]>([]);
  const [stationsError, setStationsError] = useState<string | null>(null);

  const [date, setDate] = useState(todayIso());
  const [originId, setOriginId] = useState<number | "">("");
  const [destinationId, setDestinationId] = useState<number | "">("");

  const [tripDepartures, setTripDepartures] = useState<TripDeparture[]>([]);
  const [tripDepartureId, setTripDepartureId] = useState<number | "">("");
  const [tripError, setTripError] = useState<string | null>(null);

  const [availability, setAvailability] = useState<AvailabilityResponse | null>(null);
  const [availabilityLoading, setAvailabilityLoading] = useState(false);
  const [availabilityError, setAvailabilityError] = useState<string | null>(null);

  const [selectedSeatId, setSelectedSeatId] = useState<number | null>(null);
  const [passengerName, setPassengerName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [bookingError, setBookingError] = useState<string | null>(null);
  const [confirmedBooking, setConfirmedBooking] = useState<Booking | null>(null);

  function resetSelection() {
    setSelectedSeatId(null);
    setPassengerName("");
    setBookingError(null);
    setConfirmedBooking(null);
  }

  // The route's stations - loaded once, never changes at runtime.
  useEffect(() => {
    getStations()
      .then(setStations)
      .catch((err) => setStationsError(describeError(err)));
  }, []);

  // A different date means a different trip and different seats, so
  // everything downstream (trip selection, availability, in-progress
  // booking) resets.
  useEffect(() => {
    setTripDepartureId("");
    setAvailability(null);
    resetSelection();
    setTripError(null);

    let cancelled = false;
    getTripDepartures(date)
      .then((departures) => {
        if (cancelled) return;
        setTripDepartures(departures);
        if (departures.length === 1) {
          setTripDepartureId(departures[0].id);
        }
      })
      .catch((err) => {
        if (!cancelled) setTripError(describeError(err));
      });

    return () => {
      cancelled = true;
    };
  }, [date]);

  // Availability for the selected trip + leg. AbortController cancels a
  // stale in-flight request if the user changes the leg again before it
  // resolves, so a slow older response can never overwrite a newer one.
  useEffect(() => {
    resetSelection();

    if (tripDepartureId === "" || originId === "" || destinationId === "") {
      setAvailability(null);
      return;
    }

    const controller = new AbortController();
    setAvailabilityLoading(true);
    setAvailabilityError(null);

    getAvailability(tripDepartureId, originId, destinationId, controller.signal)
      .then(setAvailability)
      .catch((err) => {
        if (!controller.signal.aborted) setAvailabilityError(describeError(err));
      })
      .finally(() => {
        if (!controller.signal.aborted) setAvailabilityLoading(false);
      });

    return () => controller.abort();
  }, [tripDepartureId, originId, destinationId]);

  function refreshAvailability() {
    if (tripDepartureId !== "" && originId !== "" && destinationId !== "") {
      getAvailability(tripDepartureId, originId, destinationId).then(setAvailability);
    }
  }

  async function handleBook() {
    if (
      selectedSeatId === null ||
      tripDepartureId === "" ||
      originId === "" ||
      destinationId === ""
    ) {
      return;
    }

    setIsSubmitting(true);
    setBookingError(null);

    try {
      const booking = await createBooking({
        tripDepartureId,
        seatId: selectedSeatId,
        originStationId: originId,
        destinationStationId: destinationId,
        passengerName: passengerName.trim(),
      });
      setConfirmedBooking(booking);
      setSelectedSeatId(null);
    } catch (err) {
      if (err instanceof ApiError && err.status === 409) {
        // Someone else booked an overlapping leg on this seat between us
        // loading availability and submitting - refresh so the grid
        // reflects reality instead of leaving a stale "available" seat.
        setBookingError("That seat was just taken for this leg. Pick another seat below.");
        refreshAvailability();
      } else {
        setBookingError(describeError(err));
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  const selectedSeat = availability?.seats.find((s) => s.seatId === selectedSeatId) ?? null;

  return (
    <div className="app">
      <header>
        <h1>Colombo Fort &ndash; Badulla</h1>
        <p className="muted">Segment-based reserved seat booking</p>
      </header>

      {stationsError && <p className="error">{stationsError}</p>}

      <JourneySelector
        stations={stations}
        date={date}
        onDateChange={setDate}
        originId={originId}
        destinationId={destinationId}
        onOriginChange={setOriginId}
        onDestinationChange={setDestinationId}
      />

      {tripError && <p className="error">{tripError}</p>}

      {tripDepartures.length > 1 && (
        <label className="field">
          <span>Departure</span>
          <select
            value={tripDepartureId}
            onChange={(e) => setTripDepartureId(e.target.value ? Number(e.target.value) : "")}
          >
            <option value="">Select departure</option>
            {tripDepartures.map((td) => (
              <option key={td.id} value={td.id}>
                {td.trainName}
              </option>
            ))}
          </select>
        </label>
      )}

      {tripDepartures.length === 0 && !tripError && (
        <p className="muted">No departures scheduled for this date.</p>
      )}

      <div className="booking-layout">
        <section>
          {availabilityLoading && <p className="muted">Checking availability...</p>}
          {availabilityError && <p className="error">{availabilityError}</p>}
          {availability && !availabilityLoading && (
            <SeatGrid
              seats={availability.seats}
              selectedSeatId={selectedSeatId}
              onSelectSeat={setSelectedSeatId}
            />
          )}
          {!availability && !availabilityLoading && originId !== "" && destinationId !== "" && (
            <p className="muted">Select a departure to see seat availability.</p>
          )}
          {(originId === "" || destinationId === "") && (
            <p className="muted">Choose an origin and destination to see seat availability.</p>
          )}
        </section>

        <section>
          <BookingPanel
            selectedSeat={selectedSeat}
            passengerName={passengerName}
            onPassengerNameChange={setPassengerName}
            onSubmit={handleBook}
            isSubmitting={isSubmitting}
            errorMessage={bookingError}
            confirmedBooking={confirmedBooking}
            onBookAnother={() => {
              setConfirmedBooking(null);
              refreshAvailability();
            }}
          />
        </section>
      </div>
    </div>
  );
}

export default App;
