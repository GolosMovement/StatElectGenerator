using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ElectionStatistics.Model.Migrations
{
    public partial class NewDBSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionEng = table.Column<string>(nullable: true),
                    DescriptionRus = table.Column<string>(nullable: false),
                    TitleEng = table.Column<string>(nullable: true),
                    TitleRus = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MappingLines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColumnNumber = table.Column<int>(nullable: false),
                    DescriptionEng = table.Column<string>(nullable: true),
                    DescriptionNative = table.Column<string>(nullable: true),
                    DescriptionRus = table.Column<string>(nullable: true),
                    Hierarchy = table.Column<int>(nullable: false),
                    HierarchyLevel = table.Column<int>(nullable: false),
                    IsCalcResult = table.Column<bool>(nullable: false),
                    IsNumber = table.Column<bool>(nullable: false),
                    IsVoteResult = table.Column<bool>(nullable: false),
                    MappingId = table.Column<int>(nullable: false),
                    TitleEng = table.Column<string>(nullable: true),
                    TitleNative = table.Column<string>(nullable: true),
                    TitleRus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MappingLines_Mappings_MappingId",
                        column: x => x.MappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionEng = table.Column<string>(nullable: true),
                    DescriptionNative = table.Column<string>(nullable: true),
                    DescriptionRus = table.Column<string>(nullable: true),
                    IsCalcResult = table.Column<bool>(nullable: false),
                    IsVoteResult = table.Column<bool>(nullable: false),
                    ProtocolSetId = table.Column<int>(nullable: false),
                    TitleEng = table.Column<string>(nullable: true),
                    TitleNative = table.Column<string>(nullable: true),
                    TitleRus = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineDescriptions_ProtocolSets_ProtocolSetId",
                        column: x => x.ProtocolSetId,
                        principalTable: "ProtocolSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommissionNumber = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    ProtocolSetId = table.Column<int>(nullable: false),
                    TitleEng = table.Column<string>(nullable: true),
                    TitleNative = table.Column<string>(nullable: true),
                    TitleRus = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Protocols_Protocols_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Protocols_ProtocolSets_ProtocolSetId",
                        column: x => x.ProtocolSetId,
                        principalTable: "ProtocolSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineDescriptionId = table.Column<int>(nullable: false),
                    ProtocolId = table.Column<int>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineNumbers_LineDescriptions_LineDescriptionId",
                        column: x => x.LineDescriptionId,
                        principalTable: "LineDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineNumbers_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineStrings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LineDescriptionId = table.Column<int>(nullable: false),
                    ProtocolId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineStrings_LineDescriptions_LineDescriptionId",
                        column: x => x.LineDescriptionId,
                        principalTable: "LineDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineStrings_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineDescriptions_ProtocolSetId",
                table: "LineDescriptions",
                column: "ProtocolSetId");

            migrationBuilder.CreateIndex(
                name: "IX_LineNumbers_LineDescriptionId",
                table: "LineNumbers",
                column: "LineDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_LineNumbers_ProtocolId",
                table: "LineNumbers",
                column: "ProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_LineStrings_LineDescriptionId",
                table: "LineStrings",
                column: "LineDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_LineStrings_ProtocolId",
                table: "LineStrings",
                column: "ProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_MappingLines_MappingId",
                table: "MappingLines",
                column: "MappingId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_ParentId",
                table: "Protocols",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_ProtocolSetId",
                table: "Protocols",
                column: "ProtocolSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineNumbers");

            migrationBuilder.DropTable(
                name: "LineStrings");

            migrationBuilder.DropTable(
                name: "MappingLines");

            migrationBuilder.DropTable(
                name: "LineDescriptions");

            migrationBuilder.DropTable(
                name: "Protocols");

            migrationBuilder.DropTable(
                name: "Mappings");

            migrationBuilder.DropTable(
                name: "ProtocolSets");
        }
    }
}
