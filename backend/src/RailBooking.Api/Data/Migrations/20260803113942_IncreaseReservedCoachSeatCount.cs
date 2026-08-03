using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RailBooking.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseReservedCoachSeatCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 1,
                column: "SeatCount",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 2,
                column: "SeatCount",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 3,
                column: "SeatCount",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 11 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 12 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 13 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 14 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 15 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 16 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 17 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 18 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 19 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 1, 20 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 21,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 22,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 23,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 24,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 25,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 26,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 27,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 28,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 29,
                column: "CoachId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 30,
                column: "CoachId",
                value: 2);

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "CoachId", "SeatNumber" },
                values: new object[,]
                {
                    { 31, 2, 11 },
                    { 32, 2, 12 },
                    { 33, 2, 13 },
                    { 34, 2, 14 },
                    { 35, 2, 15 },
                    { 36, 2, 16 },
                    { 37, 2, 17 },
                    { 38, 2, 18 },
                    { 39, 2, 19 },
                    { 40, 2, 20 },
                    { 41, 3, 1 },
                    { 42, 3, 2 },
                    { 43, 3, 3 },
                    { 44, 3, 4 },
                    { 45, 3, 5 },
                    { 46, 3, 6 },
                    { 47, 3, 7 },
                    { 48, 3, 8 },
                    { 49, 3, 9 },
                    { 50, 3, 10 },
                    { 51, 3, 11 },
                    { 52, 3, 12 },
                    { 53, 3, 13 },
                    { 54, 3, 14 },
                    { 55, 3, 15 },
                    { 56, 3, 16 },
                    { 57, 3, 17 },
                    { 58, 3, 18 },
                    { 59, 3, 19 },
                    { 60, 3, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 1,
                column: "SeatCount",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 2,
                column: "SeatCount",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Coaches",
                keyColumn: "Id",
                keyValue: 3,
                column: "SeatCount",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 2 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 5 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 6 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 7 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 8 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 9 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CoachId", "SeatNumber" },
                values: new object[] { 2, 10 });

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 21,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 22,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 23,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 24,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 25,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 26,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 27,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 28,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 29,
                column: "CoachId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Seats",
                keyColumn: "Id",
                keyValue: 30,
                column: "CoachId",
                value: 3);
        }
    }
}
