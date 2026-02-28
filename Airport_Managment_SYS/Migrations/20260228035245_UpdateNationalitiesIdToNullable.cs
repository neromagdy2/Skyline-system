using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNationalitiesIdToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "planeSeats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirplaneId = table.Column<int>(type: "int", nullable: false),
                    SeatClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planeSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_planeSeats_Airplanes_AirplaneId",
                        column: x => x.AirplaneId,
                        principalTable: "Airplanes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planeSeats_SeatClasses_SeatClassId",
                        column: x => x.SeatClassId,
                        principalTable: "SeatClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_planeSeats_AirplaneId",
                table: "planeSeats",
                column: "AirplaneId");

            migrationBuilder.CreateIndex(
                name: "IX_planeSeats_SeatClassId",
                table: "planeSeats",
                column: "SeatClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers",
                column: "NationalitiesId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "planeSeats");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers",
                column: "NationalitiesId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
