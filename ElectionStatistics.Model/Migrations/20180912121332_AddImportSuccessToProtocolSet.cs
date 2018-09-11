using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddImportSuccessToProtocolSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ImportSuccess",
                table: "ProtocolSets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportSuccess",
                table: "ProtocolSets");
        }
    }
}
