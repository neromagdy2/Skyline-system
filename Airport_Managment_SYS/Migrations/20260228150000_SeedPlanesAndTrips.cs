using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    [DbContext(typeof(Airport_Managment_SYS.DataAccess.ApplicationDbcontext))]
    [Migration("20260228150000_SeedPlanesAndTrips")]
    public partial class SeedPlanesAndTrips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // insert test airplanes
            migrationBuilder.InsertData(
                table: "Airplanes",
                columns: new[] { "Id", "Name", "Model" },
                values: new object[,]
                {
                    { 1, "TestPlaneA", "Model-A" },
                    { 2, "TestPlaneB", "Model-B" }
                });

            // add seats for each airplane
            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "SeatNumber", "Available", "Price", "seatClassId", "AirplaneId" },
                values: new object[,]
                {
                    // two seats on plane A
                    { 1, 1, true, 20f, 1, 1 },
                    { 2, 2, true, 20f, 1, 1 },
                    // two seats on plane B
                    { 3, 1, true, 25f, 1, 2 },
                    { 4, 2, true, 25f, 1, 2 }
                });

            // insert test trips using those airplanes
            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "Price", "DateTime", "AirplaneId", "Airport_ToId", "Airport_FromId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 199.99f, new DateTime(2026,3,1,9,0,0), 1, 1, 2, false },
                    { 2, 299.99f, new DateTime(2026,3,2,15,30,0), 2, 2, 1, false }
                });

            // link plane seats to trips via TripSeats junction
            migrationBuilder.InsertData(
                table: "TripSeats",
                columns: new[] { "TripId", "SeatId", "IsBooked" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 1, 2, false },
                    { 2, 3, false },
                    { 2, 4, false }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TripSeats",
                keyColumns: new[] { "TripId", "SeatId" },
                keyValues: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 }
                });

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Seats",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 });

            migrationBuilder.DeleteData(
                table: "Airplanes",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 });
        }
    }
}