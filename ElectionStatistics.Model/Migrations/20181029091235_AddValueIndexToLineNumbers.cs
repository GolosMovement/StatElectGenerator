using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    // Index for LDA
    public partial class AddValueIndexToLineNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LineNumbers_LineDescriptionId_Value",
                table: "LineNumbers",
                columns: new[] { "LineDescriptionId", "Value" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LineNumbers_LineDescriptionId_Value",
                table: "LineNumbers"
            );
        }
    }
}
