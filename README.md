# Segment-Based Train Seat Booking System

A booking system for Sri Lanka's Colombo Fort–Badulla line, where a single
reserved seat can be sold independently to multiple passengers for different,
non-overlapping legs of the same journey — e.g. one passenger travels
Colombo Fort → Kandy, another travels Kandy → Badulla, on the same physical
seat, each charged only for the distance they actually travel.

## Quick start

Prerequisite: [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
cp .env.example .env      # fill in a real Postgres password
docker compose up --build
```

That's it — no separate migration step, no manual seeding. On first boot the
API applies pending EF Core migrations and seeds the route's stations, coach
layout, and a rolling window of upcoming trip departures automatically.

- Frontend: <http://localhost:5173>
- API: <http://localhost:5080/api/stations>

To stop: `docker compose down` (add `-v` to also wipe the database volume).

## Architecture

```
rail-segment-booking/
  backend/
    RailBooking.slnx
    src/
      RailBooking.Domain/   # entities + segment-overlap & fare logic - no EF Core/ASP.NET dependency
      RailBooking.Api/      # ASP.NET Core Web API, EF Core, controllers, migrations
    tests/
      RailBooking.Domain.Tests/   # xUnit tests for the overlap/fare logic
  frontend/                 # React + TypeScript (Vite)
  docker-compose.yml        # postgres + api + frontend
```

Three containers: `postgres`, `api` (ASP.NET Core), `frontend` (static React
build served by nginx). `docker-compose.yml` wires them together on an
internal Docker network; `api` waits for Postgres to report healthy (not
just "container started") before it starts, since it runs migrations
immediately on boot.

## Core design decisions

### Domain model

- **Station** — `Name`, `SequenceNumber` (0-based position along the route),
  `DistanceKm`. Ordered and data-driven — extending the route is a data
  change, not a code change, per the assignment's configurability
  requirement.
- **Train** — a named coach/seat layout template (e.g. "Podi Menike").
- **Coach** — belongs to a Train, has a `Type` (`Reserved` / `Unreserved`)
  and a configured `SeatCount`.
- **Seat** — only ever created for `Reserved` coaches. `Unreserved` coaches
  are first-come-first-served with no seat assignment *by definition*, so
  there is nothing per-seat to model for them — this is enforced structurally
  (no `Seat` rows are ever generated for them), not by a runtime check.
- **TripDeparture** — a specific calendar-date run of a Train. This exists
  even though the assignment doesn't explicitly mention dates: without it, a
  seat booked Colombo Fort → Kandy "today" would incorrectly stay blocked for
  every future day's train too. All booking/availability is scoped to
  `(Seat, TripDeparture)`, never to `Seat` alone.
- **Booking** — references a Seat, a TripDeparture, an origin/destination
  Station pair, and (critically) `OriginSequence`/`DestinationSequence` —
  copies of the two stations' `SequenceNumber` taken at booking time. This
  denormalization isn't a performance shortcut; see the concurrency section
  below for why it's required.

### Segment representation

A booked leg is modeled as a half-open range `[OriginSequence,
DestinationSequence)` over station sequence numbers (`Segment.cs`), not raw
distance. Two different representations for two different jobs: sequence
numbers (small integers, no gaps) are exact and simple for overlap math;
`DistanceKm` (a separate field on `Station`) is used only for fare
calculation. Half-open ranges are what make **adjacent** legs — one ending at
a station, the next starting at that same station — correctly *not* count as
overlapping, which is the entire mechanic the assignment's "resell the
vacated seat" requirement depends on.

### Concurrency correctness — the core design decision

The hardest requirement is guaranteeing that two people can never book
overlapping segments of the same seat, even when they submit at the exact
same instant. The chosen mechanism is a **PostgreSQL exclusion constraint**,
not application-level locking:

```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;

ALTER TABLE "Bookings" ADD COLUMN "Segment" int4range
    GENERATED ALWAYS AS (int4range("OriginSequence", "DestinationSequence", '[)')) STORED;

ALTER TABLE "Bookings" ADD CONSTRAINT "CK_Bookings_NoOverlappingSegments"
    EXCLUDE USING gist (
        "SeatId" WITH =,
        "TripDepartureId" WITH =,
        "Segment" WITH &&
    ) WHERE ("Status" = 'Confirmed');
```

- `Segment` is a **generated column**, computed by Postgres itself from
  `OriginSequence`/`DestinationSequence`. The application (and EF Core) never
  reads or writes it directly, so it can never drift out of sync with the two
  columns that back it.
- The `EXCLUDE` constraint is the actual guarantee: Postgres refuses any
  `INSERT` that would leave two `Confirmed` bookings on the same seat and
  trip with overlapping segments — enforced by the database engine itself,
  the same category of mechanism as a `UNIQUE` constraint, generalized from
  "no duplicate value" to "no overlapping range."
- The API (`BookingsController`) attempts a normal insert inside a
  transaction; if it collides, Postgres raises SQL state `23P01`
  (`exclusion_violation`), which is caught and translated into a clean
  `409 Conflict` instead of a raw database error leaking to the client.

**This was empirically verified, not just designed.** Two overlapping
`INSERT`s fired truly concurrently (in the same shell script, so there's no
gap for tool/process latency to hide in) showed exactly the documented
Postgres behavior: the second insert **blocks** until the first transaction
resolves, then rechecks —
- if the first **committed**, the second correctly **fails**;
- if the first **rolled back**, the second correctly **succeeds** (it isn't
  punished for a booking that never actually happened).

Two **adjacent** (non-overlapping) inserts on the same seat, fired the same
way, returned in under 5ms with no blocking at all — Postgres can tell the
ranges don't overlap regardless of the other transaction's eventual outcome,
so there's nothing to wait for. This is exactly the behavior the assignment
asks for: overlapping requests are safely serialized, adjacent ones are
never falsely contended.

**Alternatives considered and rejected:**
- *Application-level pessimistic locking* (`SELECT ... FOR UPDATE` + a manual
  overlap check + insert) — works, but the correctness guarantee then lives
  in application code that has to be gotten right on every code path,
  instead of one schema constraint the database enforces unconditionally.
- *`SERIALIZABLE` isolation + retry loop* — also correct, but adds
  retry-on-conflict complexity and coarser locking than a targeted GiST index
  on just the range column.
- *Optimistic concurrency (a version/row token)* — designed for single-row
  last-writer-wins conflicts, not naturally suited to "does my new range
  overlap any of N existing rows."

The `TripDeparturesController.GetAvailability` endpoint independently reuses
`Segment.OverlapsWith` (the exact same Domain logic) to compute "is this seat
free" for the UI. That check is a **read-side convenience**, not the
correctness guarantee — if two requests race past it at the same instant,
the exclusion constraint above is still the actual arbiter.

### No booking "hold" / reservation lock

There is no temporary reservation that locks a seat the moment a user
selects it in the UI. A seat is only actually reserved once a `POST
/api/bookings` successfully commits. If two users are looking at the same
seat and one submits first, the second gets a clear `409` and is prompted to
pick another — they are never silently double-booked, and never left
believing they have a seat when they don't.

A real airline/train-style "hold for 10 minutes while you check out" is a
legitimate feature, but was deliberately deferred: it needs a new booking
state with an expiry, an availability check that treats live holds as
blocking, and something to expire stale holds (a closed browser tab
shouldn't lock a seat forever). Given the assignment's explicit "core
requirements come first" guidance and the project timeline, this was scoped
out of the core in favor of a simple, honest optimistic-booking flow with
clear conflict recovery in the UI.

### Fare calculation

`Fare = DistanceKm(origin, destination) × RatePerKm[CoachType]`
(`FareCalculator.cs`), with the rate stored as configuration
(`FareRates:ReservedPerKm` / `UnreservedPerKm` in `appsettings.json`, bound
via the ASP.NET Core Options pattern) rather than hardcoded, since it's a
business parameter the department could reasonably want to change without a
code deployment.

Kept intentionally simple, per the assignment's "core first" guidance. Worth
noting: segment-based resale already structurally addresses the fairness
problem described in the assignment's background — a passenger's fare now
reflects only the distance they actually occupy the seat, and the remaining
segment is independently resellable, rather than the current system's "pay
double to cover the seat sitting empty" rule.

### Project structure: three projects, not one

`RailBooking.Domain` has zero dependency on EF Core or ASP.NET Core — plain
C# entities plus the segment-overlap and fare logic. `RailBooking.Domain.Tests`
references only `Domain`, so the correctness-critical logic (overlap
detection, fare math) can be unit-tested in under 100ms with no database, no
Docker, nothing but plain inputs and outputs. `RailBooking.Api` references
`Domain` and owns everything infrastructure-related (EF Core, controllers,
HTTP concerns). For a small CRUD app this separation wouldn't be worth the
ceremony — it's justified here specifically because the segment-overlap
logic is the thing being graded, and this structure makes "this logic has no
coupling to infrastructure" a structural fact, not just a claim.

### Database: PostgreSQL over SQL Server

Chosen specifically for the exclusion-constraint feature described above.
SQL Server has no first-class equivalent (only triggers or
`SERIALIZABLE`-plus-retry-logic emulations); Postgres's range types + GiST
exclusion constraints let the database itself refuse an invalid state,
which is the strongest and simplest guarantee available for this exact
problem.

### Secrets

No connection string or credential is ever committed. Local (non-Docker) IDE
development uses `dotnet user-secrets` (stored outside the repo, in the
user's profile). Docker Compose reads Postgres credentials from a git-ignored
`.env` file (`.env.example` documents the required keys with no real
values).

### Seeding: two different mechanisms, deliberately

Static reference data — stations, the train, coaches, seats — is seeded via
EF Core `HasData` inside a migration (`InitialCreate`), since it's
versioned, reviewable, and never goes stale. `TripDeparture` rows (specific
calendar dates) are **not** seeded this way: `HasData` values are baked into
the migration as literal C# at the moment `dotnet ef migrations add` runs, so
a hardcoded "seed August 5th" would quietly go stale the day after. Instead,
`DbSeeder` runs at application startup and seeds a rolling 14-day window of
upcoming departures, so the demo works correctly no matter when the app is
actually started.

## API surface

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/api/stations` | Ordered list of stations |
| `GET` | `/api/trip-departures?date=` | Departures on a given date (defaults to today) |
| `GET` | `/api/trip-departures/{id}/availability?originStationId=&destinationStationId=` | Per-seat availability + fare for a specific leg |
| `POST` | `/api/bookings` | Create a booking (`201`, or `409` on a genuine conflict) |
| `GET` | `/api/bookings/{id}` | Booking confirmation detail |

## Frontend notes

Single booking-flow page — no router or global state library, since the app
is one linear flow (pick date/origin/destination → see availability → pick a
seat → book) and either would be unjustified complexity for what it actually
does. Availability requests use an `AbortController` so a slow, stale
response can never overwrite a newer one if the user changes their selection
quickly.

## Extra credit implemented

**Clearer handling of booking conflicts in the UI** (loading states,
real-time-ish availability, graceful conflict recovery) — built as part of
the core booking flow, not bolted on separately, but worth calling out
explicitly since it's a listed extra-credit item:
- A visible "Checking availability..." state while the seat grid loads.
- On a `409` (seat taken between page load and submit), a specific,
  friendly message rather than a generic error — and availability is
  automatically re-fetched so the seat grid immediately reflects reality
  instead of continuing to show a stale "available" seat.
- Verified with a real simulated race: selected a seat in the browser,
  booked it out from under that session via a background request, then
  submitted — confirmed the UI showed the correct message and refreshed
  state, not just that the API returned the right status code.

**Seat map visualization.** The initial seat picker was a flat row of
numbered buttons — functionally correct, but it didn't read as an actual
seating chart and gave no sense of where a seat sits in the coach.
`SeatMap.tsx` renders each reserved coach as its own panel with seats
arranged in real rows: 2 seats, an aisle gap, 2 more seats (5 rows of 4 for
20 seats/coach), with the outer seat of each pair labeled as a window seat.
Coaches sit side by side so all three are visible in one view, scrolling
horizontally rather than squeezing if the viewport is too narrow.

Row and window-seat position are derived purely from `SeatNumber` on the
frontend (`row = ceil(n / 4)`; the outer position, 0 or 3, within each
group of 4 is a window seat) rather than stored as real per-seat layout
metadata in the database — there's no source of truth for the department's
actual physical seat plan, so deriving a consistent layout from the data
actually available avoids inventing precision that doesn't exist. Increasing
`SeatCount` from 10 to 20 per reserved coach (to get a proper 5-row layout)
required a real schema change: `SeedData.cs` changed, and EF Core correctly
diffed the existing seeded rows against the new data and generated the
necessary `UpdateData`/`InsertData` operations in a migration automatically
— no manual SQL needed. Verified beyond just checking the markup: computed
actual pixel positions (`getBoundingClientRect`) in the browser to confirm
seats land in the intended row/aisle grid, not just that the DOM claims to.


Other ideas considered but not attempted: waitlisting for fully booked
segments, and an admin/occupancy view — deprioritized given the project
timeline and the assignment's explicit guidance to prioritize a solid core
over a longer feature list. **Multi-seat (group) booking** — letting one
request book several seats atomically for a party traveling together — came
up during manual testing and was seriously considered: the underlying
correctness mechanism would essentially fall out of the existing design for
free, since EF Core already wraps multiple inserts in a single database
transaction, so the same exclusion constraint would guarantee all-or-nothing
atomicity with no new locking logic. What made it too large to take on this
close to the deadline was the surrounding surface area — a new endpoint and
request/response shape, per-seat passenger name input, a reworked
confirmation view, and re-verifying the atomicity claim with the same rigor
the single-seat concurrency behavior was verified with earlier.

## Challenges faced

- **A known vulnerability with no available fix.** The ASP.NET Core OpenAPI
  package transitively pulls in `Microsoft.OpenApi 2.0.0`, flagged by
  `NU1903`/`GHSA-v5pm-xwqc-g5wc` (a stack-overflow risk when *parsing*
  untrusted OpenAPI/YAML). The only version line with an actual fix (3.x)
  breaks compatibility with .NET 10's source generator — confirmed by trying
  it and getting a build error. Since this project only *generates* its own
  OpenAPI document and never parses external ones, the practical risk is
  negligible; documented here as a deliberate, informed tradeoff rather than
  silently ignored.
- **A Visual Studio-specific user-secrets glitch.** After setting a
  connection string via `dotnet user-secrets`, the app failed with
  "connection string not configured" *only* when launched from Visual
  Studio's debugger — not from the CLI. Diagnosed methodically: verified the
  secret existed on disk, verified no conflicting environment variables,
  verified the CLI could start the app cleanly with the exact same
  configuration, verified the correct `UserSecretsId` was compiled into the
  assembly. The `Manage User Secrets` command in Visual Studio was found to
  be showing a stale/empty view of a file that was actually correct on disk;
  explicitly rewriting the secret through Visual Studio's own tooling
  resolved it.
- **My own flawed concurrency test.** My first attempt at proving concurrent
  overlapping requests behave correctly used two separate commands run
  moments apart — one to hold a transaction open, another to fire a
  conflicting request. The results looked plausible, but were meaningless:
  the delay between issuing the two commands turned out to exceed 13 seconds
  in one case — meaning the "concurrent" request was actually firing after
  the first transaction had already finished. Caught by adding real
  timestamps on both the database and shell side and noticing they didn't
  add up; fixed by running the entire test inside a single shell script so
  there's no gap for that delay to hide in.
- **A local dev port mismatch.** The backend's default `launchSettings.json`
  ports (auto-generated by the project template) didn't match the port
  already documented for the frontend to call — a plain configuration
  oversight, fixed by aligning both to one canonical local-dev port.
- **A stale process silently serving old state.** After stopping and
  restarting the local API multiple times during manual testing, a build
  once failed with a file-lock error — a previous `RailBooking.Api` process
  hadn't actually exited and was still holding its own executable open. Worse,
  that same orphaned process was still answering requests on the expected
  port the whole time, so an earlier symptom (trip departures appearing
  empty after a database reset) was actually caused by talking to a stale
  process that had seeded itself before the reset and never got the chance
  to re-seed. Found by explicitly listing OS processes by name rather than
  trusting that "the API responds on this port" means "the API I just
  started is the one responding."

## Verification

- `dotnet test` on `RailBooking.Domain.Tests` — 13 tests covering segment
  overlap (adjacent legs don't conflict, overlapping legs do, one leg fully
  containing another, identical legs) and fare calculation, with zero
  infrastructure required.
- Manual concurrency verification against a real Postgres instance (see
  "Concurrency correctness" above) — overlapping inserts correctly
  serialize and resolve based on the first transaction's actual outcome;
  adjacent inserts never block.
- Full booking flow exercised through a real browser against the actual
  running API and database — including a genuine simulated race condition,
  not just the golden path.
- `docker compose down -v && docker compose up -d` — the whole stack
  verified working from a completely empty database volume, with zero
  manual steps, confirming the one-shot clean-machine requirement for real.

## Local development without Docker

Backend:
```bash
cd backend/src/RailBooking.Api
dotnet user-secrets set "ConnectionStrings:RailBookingDb" "Host=localhost;Port=5432;Database=railbooking;Username=postgres;Password=..."
dotnet run
```

Frontend:
```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```
