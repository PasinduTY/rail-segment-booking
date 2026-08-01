using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RailBooking.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "numeric(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainId = table.Column<int>(type: "integer", nullable: false),
                    CoachNumber = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SeatCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coaches_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripDepartures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainId = table.Column<int>(type: "integer", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDepartures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDepartures_Trains_TrainId",
                        column: x => x.TrainId,
                        principalTable: "Trains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoachId = table.Column<int>(type: "integer", nullable: false),
                    SeatNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeatId = table.Column<int>(type: "integer", nullable: false),
                    TripDepartureId = table.Column<int>(type: "integer", nullable: false),
                    OriginStationId = table.Column<int>(type: "integer", nullable: false),
                    DestinationStationId = table.Column<int>(type: "integer", nullable: false),
                    OriginSequence = table.Column<int>(type: "integer", nullable: false),
                    DestinationSequence = table.Column<int>(type: "integer", nullable: false),
                    PassengerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FareAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Stations_DestinationStationId",
                        column: x => x.DestinationStationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Stations_OriginStationId",
                        column: x => x.OriginStationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_TripDepartures_TripDepartureId",
                        column: x => x.TripDepartureId,
                        principalTable: "TripDepartures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "DistanceKm", "Name", "SequenceNumber" },
                values: new object[,]
                {
                    { 1, 0m, "Colombo Fort", 0 },
                    { 2, 66m, "Rambukkana", 1 },
                    { 3, 121m, "Kandy", 2 },
                    { 4, 127m, "Peradeniya", 3 },
                    { 5, 168m, "Hatton", 4 },
                    { 6, 186m, "Nanu Oya", 5 },
                    { 7, 194m, "Haputale", 6 },
                    { 8, 204m, "Bandarawela", 7 },
                    { 9, 216m, "Ella", 8 },
                    { 10, 230m, "Badulla", 9 }
                });

            migrationBuilder.InsertData(
                table: "Trains",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Podi Menike" });

            migrationBuilder.InsertData(
                table: "Coaches",
                columns: new[] { "Id", "CoachNumber", "SeatCount", "TrainId", "Type" },
                values: new object[,]
                {
                    { 1, 1, 10, 1, "Reserved" },
                    { 2, 2, 10, 1, "Reserved" },
                    { 3, 3, 10, 1, "Reserved" },
                    { 4, 4, 80, 1, "Unreserved" },
                    { 5, 5, 80, 1, "Unreserved" },
                    { 6, 6, 80, 1, "Unreserved" },
                    { 7, 7, 80, 1, "Unreserved" },
                    { 8, 8, 80, 1, "Unreserved" }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "CoachId", "SeatNumber" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 2 },
                    { 3, 1, 3 },
                    { 4, 1, 4 },
                    { 5, 1, 5 },
                    { 6, 1, 6 },
                    { 7, 1, 7 },
                    { 8, 1, 8 },
                    { 9, 1, 9 },
                    { 10, 1, 10 },
                    { 11, 2, 1 },
                    { 12, 2, 2 },
                    { 13, 2, 3 },
                    { 14, 2, 4 },
                    { 15, 2, 5 },
                    { 16, 2, 6 },
                    { 17, 2, 7 },
                    { 18, 2, 8 },
                    { 19, 2, 9 },
                    { 20, 2, 10 },
                    { 21, 3, 1 },
                    { 22, 3, 2 },
                    { 23, 3, 3 },
                    { 24, 3, 4 },
                    { 25, 3, 5 },
                    { 26, 3, 6 },
                    { 27, 3, 7 },
                    { 28, 3, 8 },
                    { 29, 3, 9 },
                    { 30, 3, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DestinationStationId",
                table: "Bookings",
                column: "DestinationStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_OriginStationId",
                table: "Bookings",
                column: "OriginStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SeatId_TripDepartureId",
                table: "Bookings",
                columns: new[] { "SeatId", "TripDepartureId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TripDepartureId",
                table: "Bookings",
                column: "TripDepartureId");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_TrainId_CoachNumber",
                table: "Coaches",
                columns: new[] { "TrainId", "CoachNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_CoachId_SeatNumber",
                table: "Seats",
                columns: new[] { "CoachId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_SequenceNumber",
                table: "Stations",
                column: "SequenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripDepartures_TrainId_ServiceDate",
                table: "TripDepartures",
                columns: new[] { "TrainId", "ServiceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "TripDepartures");

            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "Trains");
        }
    }
}
