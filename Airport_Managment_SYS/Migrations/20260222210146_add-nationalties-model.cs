using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airport_Managment_SYS.Migrations
{
    /// <inheritdoc />
    public partial class addnationaltiesmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "NationalitiesId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NationalitiesId",
                table: "AspNetUsers",
                column: "NationalitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers",
                column: "NationalitiesId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Nationalities_NationalitiesId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NationalitiesId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalitiesId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
