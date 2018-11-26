using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class SetIsLeafForProtocols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Protocols
                SET IsLeaf = 1
                WHERE Id IN (
                    SELECT Id
                    FROM Protocols
                    WHERE Id NOT IN (
                        SELECT ParentId
                        FROM protocols
                        WHERE parentid IS NOT NULL
                    )
                );");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Protocols SET IsLeaf = 0;");
        }
    }
}
