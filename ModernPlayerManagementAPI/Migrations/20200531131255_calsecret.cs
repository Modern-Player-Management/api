using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModernPlayerManagementAPI.Migrations
{
    public partial class calsecret : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CalendarSecret",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarSecret",
                table: "Users");
        }
    }
}
