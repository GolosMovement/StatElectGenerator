using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddDataLineNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hierarchy",
                table: "MappingLines");

            migrationBuilder.AddColumn<int>(
                name: "DataLineNumber",
                table: "Mappings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHierarchy",
                table: "MappingLines",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataLineNumber",
                table: "Mappings");

            migrationBuilder.DropColumn(
                name: "IsHierarchy",
                table: "MappingLines");

            migrationBuilder.AddColumn<int>(
                name: "Hierarchy",
                table: "MappingLines",
                nullable: false,
                defaultValue: 0);
        }
    }
}
