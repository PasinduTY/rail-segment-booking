import type { SeatAvailability } from "../api/types";

// 2 seats + aisle + 2 seats per row - a typical reserved-coach layout. The
// outer seat of each pair is the window seat, the inner one sits next to
// the aisle. This is derived purely from SeatNumber (row = ceil(n/4),
// position 0 or 3 within the row is a window) rather than stored as real
// per-seat layout data - there's no source of truth for the department's
// actual physical seat plan, so deriving a consistent, honest layout from
// the seat number avoids inventing precision the data doesn't have.
const SEATS_PER_ROW = 4;

function isWindowSeat(seatNumber: number): boolean {
  const positionInRow = (seatNumber - 1) % SEATS_PER_ROW;
  return positionInRow === 0 || positionInRow === SEATS_PER_ROW - 1;
}

interface SeatRow {
  rowNumber: number;
  leftPair: SeatAvailability[];
  rightPair: SeatAvailability[];
}

function groupIntoRows(seats: SeatAvailability[]): SeatRow[] {
  const rows = new Map<number, SeatAvailability[]>();
  for (const seat of seats) {
    const rowNumber = Math.ceil(seat.seatNumber / SEATS_PER_ROW);
    const row = rows.get(rowNumber);
    if (row) {
      row.push(seat);
    } else {
      rows.set(rowNumber, [seat]);
    }
  }

  return [...rows.entries()]
    .sort(([a], [b]) => a - b)
    .map(([rowNumber, rowSeats]) => ({
      rowNumber,
      leftPair: rowSeats.slice(0, 2),
      rightPair: rowSeats.slice(2, 4),
    }));
}

interface SeatButtonProps {
  seat: SeatAvailability;
  isSelected: boolean;
  onSelectSeat: (seatId: number) => void;
}

function SeatButton({ seat, isSelected, onSelectSeat }: SeatButtonProps) {
  const isWindow = isWindowSeat(seat.seatNumber);
  const title = seat.isAvailable
    ? `Seat ${seat.seatNumber}${isWindow ? " (window)" : ""} - Rs. ${seat.fare.toFixed(2)}`
    : `Seat ${seat.seatNumber} - already booked for this leg`;

  return (
    <button
      type="button"
      className={
        "seat" +
        (seat.isAvailable ? " available" : " taken") +
        (isSelected ? " selected" : "") +
        (isWindow ? " window" : "")
      }
      disabled={!seat.isAvailable}
      onClick={() => onSelectSeat(seat.seatId)}
      title={title}
    >
      {seat.seatNumber}
      {isWindow && (
        <span className="window-badge" aria-hidden="true">
          W
        </span>
      )}
    </button>
  );
}

interface SeatMapProps {
  seats: SeatAvailability[];
  selectedSeatId: number | null;
  onSelectSeat: (seatId: number) => void;
}

export function SeatMap({ seats, selectedSeatId, onSelectSeat }: SeatMapProps) {
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
    <div className="seat-map">
      <div className="coaches">
      {[...coaches.entries()].map(([coachNumber, coachSeats]) => (
        <div key={coachNumber} className="coach">
          <h3>Coach {coachNumber}</h3>
          <div className="coach-body">
            {groupIntoRows(coachSeats).map((row) => (
              <div key={row.rowNumber} className="seat-row">
                <div className="seat-pair">
                  {row.leftPair.map((seat) => (
                    <SeatButton
                      key={seat.seatId}
                      seat={seat}
                      isSelected={seat.seatId === selectedSeatId}
                      onSelectSeat={onSelectSeat}
                    />
                  ))}
                </div>
                <div className="aisle" aria-hidden="true" />
                <div className="seat-pair">
                  {row.rightPair.map((seat) => (
                    <SeatButton
                      key={seat.seatId}
                      seat={seat}
                      isSelected={seat.seatId === selectedSeatId}
                      onSelectSeat={onSelectSeat}
                    />
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
      </div>
      <div className="seat-legend">
        <span className="seat available" /> Available
        <span className="seat taken" /> Taken
        <span className="seat selected" /> Selected
        <span className="legend-window">W</span> Window seat
      </div>
    </div>
  );
}
