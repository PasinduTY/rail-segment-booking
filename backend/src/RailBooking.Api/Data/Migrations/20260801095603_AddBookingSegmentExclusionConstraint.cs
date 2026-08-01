using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailBooking.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSegmentExclusionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Exclusion constraints on plain equality columns (SeatId,
            // TripDepartureId) inside a GiST index require btree_gist -
            // GiST doesn't support integer equality out of the box.
            migrationBuilder.Sql(
                """
                CREATE EXTENSION IF NOT EXISTS btree_gist;
                """);

            // "Segment" is derived by Postgres itself from OriginSequence/
            // DestinationSequence - EF Core never reads or writes it, so it
            // can never drift out of sync with the two columns that back it.
            // '[)' makes it half-open, so adjacent legs (one ending where
            // the next starts) do not count as overlapping.
            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings"
                    ADD COLUMN "Segment" int4range
                    GENERATED ALWAYS AS (int4range("OriginSequence", "DestinationSequence", '[)')) STORED;
                """);

            // The actual correctness guarantee: Postgres refuses any insert
            // or update that would leave two Confirmed bookings on the same
            // seat, same trip, with overlapping segments - enforced by the
            // database itself, not by application code.
            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings"
                    ADD CONSTRAINT "CK_Bookings_NoOverlappingSegments"
                    EXCLUDE USING gist (
                        "SeatId" WITH =,
                        "TripDepartureId" WITH =,
                        "Segment" WITH &&
                    ) WHERE ("Status" = 'Confirmed');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings" DROP CONSTRAINT "CK_Bookings_NoOverlappingSegments";
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings" DROP COLUMN "Segment";
                """);
        }
    }
}
