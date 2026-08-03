import type { Booking, SeatAvailability } from "../api/types";

interface BookingPanelProps {
  selectedSeat: SeatAvailability | null;
  passengerName: string;
  onPassengerNameChange: (name: string) => void;
  onSubmit: () => void;
  isSubmitting: boolean;
  errorMessage: string | null;
  confirmedBooking: Booking | null;
  onBookAnother: () => void;
}

export function BookingPanel({
  selectedSeat,
  passengerName,
  onPassengerNameChange,
  onSubmit,
  isSubmitting,
  errorMessage,
  confirmedBooking,
  onBookAnother,
}: BookingPanelProps) {
  if (confirmedBooking) {
    return (
      <div className="booking-panel confirmation">
        <h3>Booking confirmed</h3>
        <dl>
          <dt>Passenger</dt>
          <dd>{confirmedBooking.passengerName}</dd>
          <dt>Journey</dt>
          <dd>
            {confirmedBooking.originStationName} &rarr;{" "}
            {confirmedBooking.destinationStationName}
          </dd>
          <dt>Seat</dt>
          <dd>
            Coach {confirmedBooking.coachNumber}, Seat {confirmedBooking.seatNumber}
          </dd>
          <dt>Fare</dt>
          <dd>Rs. {confirmedBooking.fareAmount.toFixed(2)}</dd>
          <dt>Booking ID</dt>
          <dd>#{confirmedBooking.id}</dd>
        </dl>
        <button type="button" onClick={onBookAnother}>
          Book another seat
        </button>
      </div>
    );
  }

  return (
    <div className="booking-panel">
      <h3>Passenger details</h3>
      {!selectedSeat && <p className="muted">Select an available seat to continue.</p>}
      {selectedSeat && (
        <p>
          Coach {selectedSeat.coachNumber}, Seat {selectedSeat.seatNumber} &mdash; Rs.{" "}
          {selectedSeat.fare.toFixed(2)}
        </p>
      )}

      <label className="field">
        <span>Passenger name</span>
        <input
          type="text"
          value={passengerName}
          onChange={(e) => onPassengerNameChange(e.target.value)}
          disabled={!selectedSeat}
          placeholder="Full name"
        />
      </label>

      {errorMessage && <p className="error">{errorMessage}</p>}

      <button
        type="button"
        onClick={onSubmit}
        disabled={!selectedSeat || !passengerName.trim() || isSubmitting}
      >
        {isSubmitting ? "Booking..." : "Book seat"}
      </button>
    </div>
  );
}
