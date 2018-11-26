using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectionStatistics.Model.Migrations
{
    public partial class CreateProtocolRootItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO Protocols
                (ProtocolSetId, TitleRus, TitleEng, CommissionNumber)
                SELECT PS.Id, PS.TitleRus, PS.TitleEng, 0 FROM ProtocolSets PS

                DECLARE @protocolSetId INT
                DECLARE @currentRoot INT

                DECLARE SetsCursor CURSOR FOR
                SELECT Id
                FROM ProtocolSets

                OPEN SetsCursor

                FETCH NEXT FROM SetsCursor INTO @protocolSetId
                WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SELECT TOP 1 @currentRoot = Id
                        FROM Protocols
                        WHERE Protocols.ProtocolSetId = @protocolSetId
                            AND Protocols.ParentId IS NULL
                        ORDER BY Id DESC

                        UPDATE Protocols
                        SET ParentId = @currentRoot
                        WHERE
                            Protocols.Id IN (
                                SELECT Id
                                FROM Protocols
                                WHERE Protocols.ProtocolSetId = @protocolSetId
                                    AND Protocols.ParentId IS NULL
                                    AND Protocols.Id <> @currentRoot
                            )

                        FETCH NEXT FROM SetsCursor INTO @protocolSetId
                    END

                CLOSE SetsCursor
                DEALLOCATE SetsCursor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DECLARE @protocolSetId INT
                DECLARE @currentRoot INT

                DECLARE SetsCursor CURSOR FOR
                SELECT Id
                FROM ProtocolSets

                OPEN SetsCursor

                FETCH NEXT FROM SetsCursor INTO @protocolSetId
                WHILE @@FETCH_STATUS = 0
                    BEGIN
                        SELECT TOP 1 @currentRoot = Id
                        FROM Protocols
                        WHERE Protocols.ProtocolSetId = @protocolSetId
                            AND Protocols.ParentId IS NULL
                        ORDER BY Id DESC

                        UPDATE Protocols
                        SET ParentId = NULL
                        WHERE
                        Protocols.ParentId = @currentRoot

                        DELETE FROM Protocols WHERE Id = @currentRoot

                        FETCH NEXT FROM SetsCursor INTO @protocolSetId
                    END

                CLOSE SetsCursor
                DEALLOCATE SetsCursor");
        }
    }
}
