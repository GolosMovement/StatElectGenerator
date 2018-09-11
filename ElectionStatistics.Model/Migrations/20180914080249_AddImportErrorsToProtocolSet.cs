using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddImportErrorsToProtocolSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportErrors",
                table: "ProtocolSets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportErrors",
                table: "ProtocolSets");
        }
    }
}
