using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddImportFieldsToProtocolSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportCurrentLine",
                table: "ProtocolSets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportFinishedAt",
                table: "ProtocolSets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportStartedAt",
                table: "ProtocolSets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportTotalLines",
                table: "ProtocolSets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportCurrentLine",
                table: "ProtocolSets");

            migrationBuilder.DropColumn(
                name: "ImportFinishedAt",
                table: "ProtocolSets");

            migrationBuilder.DropColumn(
                name: "ImportStartedAt",
                table: "ProtocolSets");

            migrationBuilder.DropColumn(
                name: "ImportTotalLines",
                table: "ProtocolSets");
        }
    }
}
