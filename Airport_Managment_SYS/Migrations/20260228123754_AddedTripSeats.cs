using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class AddedTripSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Seats_SeatId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_SeatId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "Trips");

            migrationBuilder.CreateTable(
                name: "TripSeats",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripSeats", x => new { x.TripId, x.SeatId });
                    table.ForeignKey(
                        name: "FK_TripSeats_Seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripSeats_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripSeats_SeatId",
                table: "TripSeats",
                column: "SeatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TripSeats");

            migrationBuilder.AddColumn<int>(
                name: "SeatId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_SeatId",
                table: "Trips",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Seats_SeatId",
                table: "Trips",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id");
        }
    }
}
