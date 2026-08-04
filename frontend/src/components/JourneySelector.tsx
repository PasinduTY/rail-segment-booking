import type { Station } from "../api/types";

interface JourneySelectorProps {
  stations: Station[];
  date: string;
  minDate: string;
  onDateChange: (date: string) => void;
  originId: number | "";
  destinationId: number | "";
  onOriginChange: (id: number | "") => void;
  onDestinationChange: (id: number | "") => void;
}

export function JourneySelector({
  stations,
  date,
  minDate,
  onDateChange,
  originId,
  destinationId,
  onOriginChange,
  onDestinationChange,
}: JourneySelectorProps) {
  const originStation = stations.find((s) => s.id === originId);

  // Destination choices are restricted to stations after the chosen origin.
  // This is a UX nicety only - the backend independently rejects an invalid
  // leg regardless of what the UI allows, so this can't be relied on for
  // correctness, only for guiding the user away from an error they'd
  // otherwise have to see after submitting.
  const destinationOptions = originStation
    ? stations.filter((s) => s.sequenceNumber > originStation.sequenceNumber)
    : stations;

  return (
    <div className="journey-selector">
      <label className="field">
        <span>Date</span>
        <input
          type="date"
          value={date}
          min={minDate}
          onChange={(e) => onDateChange(e.target.value)}
        />
      </label>

      <label className="field">
        <span>From</span>
        <select
          value={originId}
          onChange={(e) => {
            const value = e.target.value ? Number(e.target.value) : "";
            onOriginChange(value);
            // If the current destination is no longer downstream of the
            // new origin, clear it rather than leave an invalid pairing.
            if (
              value !== "" &&
              destinationId !== "" &&
              stations.find((s) => s.id === destinationId)!.sequenceNumber <=
                stations.find((s) => s.id === value)!.sequenceNumber
            ) {
              onDestinationChange("");
            }
          }}
        >
          <option value="">Select origin</option>
          {stations.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>
      </label>

      <label className="field">
        <span>To</span>
        <select
          value={destinationId}
          onChange={(e) =>
            onDestinationChange(e.target.value ? Number(e.target.value) : "")
          }
          disabled={originId === ""}
        >
          <option value="">Select destination</option>
          {destinationOptions.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>
      </label>
    </div>
  );
}
