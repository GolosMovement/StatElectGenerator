using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class RenameProtocolSetImportError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImportErrors",
                table: "ProtocolSets",
                newName: "ImportErrorCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImportErrorCount",
                table: "ProtocolSets",
                newName: "ImportErrors");
        }
    }
}
