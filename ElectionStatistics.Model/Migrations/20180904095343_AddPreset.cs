using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddPreset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Presets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Expression = table.Column<string>(nullable: false),
                    TitleRus = table.Column<string>(nullable: false),
                    TitleEng = table.Column<string>(nullable: true),
                    DescriptionRus = table.Column<string>(nullable: true),
                    DescriptionEng = table.Column<string>(nullable: true),
                    ProtocolSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presets_ProtocolSets_ProtocolSetId",
                        column: x => x.ProtocolSetId,
                        principalTable: "ProtocolSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Presets_ProtocolSetId",
                table: "Presets",
                column: "ProtocolSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Presets");
        }
    }
}
