using Microsoft.EntityFrameworkCore.Migrations;

namespace ModernPlayerManagementAPI.Migrations
{
    public partial class events3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelayLength",
                table: "Discrepancy",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayLength",
                table: "Discrepancy");
        }
    }
}