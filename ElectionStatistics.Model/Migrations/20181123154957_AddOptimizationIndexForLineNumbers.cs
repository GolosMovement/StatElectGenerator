using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class AddOptimizationIndexForLineNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE NONCLUSTERED INDEX
                [IX_LineNumbers_LineDescriptionId_ProtocolIdValue] ON
                    [dbo].[LineNumbers]([LineDescriptionId] ASC)
                INCLUDE ([ProtocolId],[Value])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,
                    SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF,
                    ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LineNumbers_LineDescriptionId_ProtocolIdValue",
                table: "LineNumbers"
            );
        }
    }
}
