using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class FixedPricingArangment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Seats");

            migrationBuilder.AddColumn<double>(
                name: "PriceMult",
                table: "SeatClasses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceMult",
                table: "SeatClasses");

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "Seats",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
