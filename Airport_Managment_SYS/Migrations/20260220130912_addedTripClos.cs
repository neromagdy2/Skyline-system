using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class addedTripClos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AirplaneId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_AirplaneId",
                table: "Trips",
                column: "AirplaneId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_SeatId",
                table: "Trips",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Airplanes_AirplaneId",
                table: "Trips",
                column: "AirplaneId",
                principalTable: "Airplanes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Seats_SeatId",
                table: "Trips",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Airplanes_AirplaneId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Seats_SeatId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_AirplaneId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_SeatId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "AirplaneId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "Trips");
        }
    }
}
