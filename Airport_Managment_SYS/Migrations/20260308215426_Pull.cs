using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class Pull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "NoOfSeats",
            //    table: "Reservations",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "NoOfSeats",
            //    table: "Reservations");
        }
    }
}
