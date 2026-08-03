import type { SeatAvailability } from "../api/types";

interface SeatGridProps {
  seats: SeatAvailability[];
  selectedSeatId: number | null;
  onSelectSeat: (seatId: number) => void;
}

export function SeatGrid({ seats, selectedSeatId, onSelectSeat }: SeatGridProps) {
  if (seats.length === 0) {
    return <p className="muted">No reserved seats found for this train.</p>;
  }

  // The API already returns seats ordered by coach then seat number - group
  // by coach here purely for display, not to change ordering.
  const coaches = new Map<number, SeatAvailability[]>();
  for (const seat of seats) {
    const group = coaches.get(seat.coachNumber);
    if (group) {
      group.push(seat);
    } else {
      coaches.set(seat.coachNumber, [seat]);
    }
  }

  return (
    <div className="seat-grid">
      {[...coaches.entries()].map(([coachNumber, coachSeats]) => (
        <div key={coachNumber} className="coach">
          <h3>Coach {coachNumber}</h3>
          <div className="coach-seats">
            {coachSeats.map((seat) => {
              const isSelected = seat.seatId === selectedSeatId;
              return (
                <button
                  key={seat.seatId}
                  type="button"
                  className={
                    "seat" +
                    (seat.isAvailable ? " available" : " taken") +
                    (isSelected ? " selected" : "")
                  }
                  disabled={!seat.isAvailable}
                  onClick={() => onSelectSeat(seat.seatId)}
                  title={
                    seat.isAvailable
                      ? `Seat ${seat.seatNumber} - Rs. ${seat.fare.toFixed(2)}`
                      : `Seat ${seat.seatNumber} - already booked for this leg`
                  }
                >
                  {seat.seatNumber}
                </button>
              );
            })}
          </div>
        </div>
      ))}
      <div className="seat-legend">
        <span className="seat available" /> Available
        <span className="seat taken" /> Taken
        <span className="seat selected" /> Selected
      </div>
    </div>
  );
}
