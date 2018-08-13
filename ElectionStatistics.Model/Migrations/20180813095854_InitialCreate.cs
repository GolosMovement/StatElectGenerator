using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ElectionStatistics.Model.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GenitiveName = table.Column<string>(maxLength: 500, nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    ShortName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElectoralDistricts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HierarchyPath = table.Column<string>(maxLength: 100, nullable: true),
                    HigherDistrictId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectoralDistricts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectoralDistricts_ElectoralDistricts_HigherDistrictId",
                        column: x => x.HigherDistrictId,
                        principalTable: "ElectoralDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataSourceUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ElectoralDistrictId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elections_ElectoralDistricts_ElectoralDistrictId",
                        column: x => x.ElectoralDistrictId,
                        principalTable: "ElectoralDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionCandidates",
                columns: table => new
                {
                    ElectionId = table.Column<int>(nullable: false),
                    CandidateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionCandidates", x => new { x.ElectionId, x.CandidateId });
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AbsenteeCertificateVotersCount = table.Column<int>(nullable: false),
                    CanceledAbsenteeCertificatesCount = table.Column<int>(nullable: false),
                    CanceledBallotsCount = table.Column<int>(nullable: false),
                    DataSourceUrl = table.Column<string>(maxLength: 1000, nullable: false),
                    EarlyIssuedBallotsCount = table.Column<int>(nullable: false),
                    ElectionId = table.Column<int>(nullable: false),
                    ElectoralDistrictId = table.Column<int>(nullable: false),
                    InsideBallotsCount = table.Column<int>(nullable: false),
                    InvalidBallotsCount = table.Column<int>(nullable: false),
                    IssuedAbsenteeCertificatesCount = table.Column<int>(nullable: false),
                    IssuedByHigherDistrictAbsenteeCertificatesCount = table.Column<int>(nullable: false),
                    IssuedInsideBallotsCount = table.Column<int>(nullable: false),
                    IssuedOutsideBallotsCount = table.Column<int>(nullable: false),
                    LostAbsenteeCertificatesCount = table.Column<int>(nullable: false),
                    LostBallotsCount = table.Column<int>(nullable: false),
                    OutsideBallotsCount = table.Column<int>(nullable: false),
                    ReceivedAbsenteeCertificatesCount = table.Column<int>(nullable: false),
                    ReceivedBallotsCount = table.Column<int>(nullable: false),
                    UnaccountedBallotsCount = table.Column<int>(nullable: false),
                    ValidBallotsCount = table.Column<int>(nullable: false),
                    VotersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionResults_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionResults_ElectoralDistricts_ElectoralDistrictId",
                        column: x => x.ElectoralDistrictId,
                        principalTable: "ElectoralDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectoralDistrictElections",
                columns: table => new
                {
                    ElectionId = table.Column<int>(nullable: false),
                    ElectoralDistrictId = table.Column<int>(nullable: false),
                    DataSourceUrl = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectoralDistrictElections", x => new { x.ElectionId, x.ElectoralDistrictId });
                    table.ForeignKey(
                        name: "FK_ElectoralDistrictElections_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectoralDistrictElections_ElectoralDistricts_ElectoralDistrictId",
                        column: x => x.ElectoralDistrictId,
                        principalTable: "ElectoralDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectionCandidateVotes",
                columns: table => new
                {
                    ElectionResultId = table.Column<int>(nullable: false),
                    CandidateId = table.Column<int>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionCandidateVotes", x => new { x.ElectionResultId, x.CandidateId });
                    table.ForeignKey(
                        name: "FK_ElectionCandidateVotes_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionCandidateVotes_ElectionResults_ElectionResultId",
                        column: x => x.ElectionResultId,
                        principalTable: "ElectionResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_CandidateId",
                table: "ElectionCandidates",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidateVotes_CandidateId",
                table: "ElectionCandidateVotes",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionResults_ElectionId",
                table: "ElectionResults",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionResults_ElectoralDistrictId",
                table: "ElectionResults",
                column: "ElectoralDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_ElectoralDistrictId",
                table: "Elections",
                column: "ElectoralDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectoralDistrictElections_ElectoralDistrictId",
                table: "ElectoralDistrictElections",
                column: "ElectoralDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectoralDistricts_HigherDistrictId",
                table: "ElectoralDistricts",
                column: "HigherDistrictId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectionCandidates");

            migrationBuilder.DropTable(
                name: "ElectionCandidateVotes");

            migrationBuilder.DropTable(
                name: "ElectoralDistrictElections");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "ElectionResults");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropTable(
                name: "ElectoralDistricts");
        }
    }
}
