using Microsoft.EntityFrameworkCore.Migrations;

namespace SimSolverV2.Migrations
{
    public partial class migV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vehicleCount",
                table: "Results");

            migrationBuilder.AddColumn<string>(
                name: "vehicleData",
                table: "Results",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vehicleData",
                table: "Results");

            migrationBuilder.AddColumn<int>(
                name: "vehicleCount",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
