﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddHiddenToProtocolSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "ProtocolSets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "ProtocolSets");
        }
    }
}
