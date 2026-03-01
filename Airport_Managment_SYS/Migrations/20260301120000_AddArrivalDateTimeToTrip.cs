using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class AddArrivalDateTimeToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalDateTime",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            // Backfill existing rows: arrival = departure + 3 hours
            migrationBuilder.Sql("UPDATE Trips SET ArrivalDateTime = DATEADD(hour, 3, DateTime)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalDateTime",
                table: "Trips");
        }
    }
}
