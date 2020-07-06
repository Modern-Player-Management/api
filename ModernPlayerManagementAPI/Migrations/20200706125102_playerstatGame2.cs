using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ModernPlayerManagementAPI.Migrations
{
    public partial class playerstatGame2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStats_Game_GameId",
                table: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "PlayerStats");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "PlayerStats",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStats_Game_GameId",
                table: "PlayerStats",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStats_Game_GameId",
                table: "PlayerStats");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "PlayerStats",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "PlayerStats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStats_Game_GameId",
                table: "PlayerStats",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
