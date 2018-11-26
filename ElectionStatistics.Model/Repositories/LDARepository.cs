using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ElectionStatistics.Model
{
    public class LDANumbers
    {
        public int Total { get; set; }
        public List<int> Numbers { get; set; }
    }

    // TODO: tests, DRY
    public class LDARepository : RawSQLRepository
    {
        public LDARepository(SqlConnection connection) : base(connection)
        {
        }

        public LDANumbers CountNumbers(int protocolSetId,
            int [] lineDescriptionsIds,
            int? protocolId,
            int? minValue)
        {
            var baseSelect =
                @"SELECT
                    count(*) as total,
                    count(case when LineNumbers.Value % 10 = 0 then 1 end) as total0,
                    count(case when LineNumbers.Value % 10 = 1 then 1 end) as total1,
                    count(case when LineNumbers.Value % 10 = 2 then 1 end) as total2,
                    count(case when LineNumbers.Value % 10 = 3 then 1 end) as total3,
                    count(case when LineNumbers.Value % 10 = 4 then 1 end) as total4,
                    count(case when LineNumbers.Value % 10 = 5 then 1 end) as total5,
                    count(case when LineNumbers.Value % 10 = 6 then 1 end) as total6,
                    count(case when LineNumbers.Value % 10 = 7 then 1 end) as total7,
                    count(case when LineNumbers.Value % 10 = 8 then 1 end) as total8,
                    count(case when LineNumbers.Value % 10 = 9 then 1 end) as total9
                ";

            var hierarchyQuery =
                @"WITH hierarchyQuery AS
                    (SELECT Id
                    FROM Protocols p1
                    WHERE p1.id = @protocolId
                    UNION ALL
                    SELECT p2.Id
                    FROM Protocols p2
                    JOIN hierarchyQuery ON p2.ParentId = hierarchyQuery.Id)
                ";

            var hierarchyFrom =
                @"FROM
                    (SELECT LineNumbers.Value
                    FROM hierarchyQuery
                    JOIN LineNumbers ON (LineNumbers.ProtocolId = hierarchyQuery.Id)
                    WHERE
                        LineNumbers.Value IS NOT NULL
                        %minValueFilter%
                        %lineDescFilter%
                ) as LineNumbers
                ";

            var protocolSetFrom =
                @"FROM LineNumbers
                JOIN LineDescriptions ON (LineDescriptions.Id = LineNumbers.LineDescriptionId)
                WHERE
                    LineDescriptions.ProtocolSetId = @protocolSetId
                    AND LineNumbers.Value IS NOT NULL
                    %minValueFilter%
                    %lineDescFilter%
                ";

            var minValueFilter = "AND LineNumbers.Value >= @minValue";
            var lineDescFilter = "AND LineNumbers.LineDescriptionId IN (%lineDescriptionsIds%)";

            var sql = "";
            SqlCommand command = new SqlCommand();

            if (protocolId != null)
            {
                sql = hierarchyQuery + baseSelect + hierarchyFrom;

                command.Parameters.AddWithValue("@protocolId", protocolId);
            }
            else
            {
                sql = baseSelect + protocolSetFrom;

                command.Parameters.AddWithValue("@protocolSetId", protocolSetId);
            }

            if (minValue != null) {
                sql = sql.Replace("%minValueFilter%", minValueFilter);

                command.Parameters.AddWithValue("@minValue", minValue);
            }
            else
            {
                sql = sql.Replace("%minValueFilter%", "");
            }

            if (lineDescriptionsIds.Length > 0)
            {
                // FIXME: possible SQL injection
                sql = sql.Replace("%lineDescFilter%", lineDescFilter).Replace(
                    "%lineDescriptionsIds%", string.Join(",", lineDescriptionsIds));
            }
            else
            {
                sql = sql.Replace("%lineDescFilter%", "");
            }

            command.Connection = connection;
            command.CommandText = sql;

            connection.Open();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LDANumbers result = new LDANumbers();
                        result.Total = reader.GetInt32(0);
                        result.Numbers = new List<int>();

                        for (int i = 1; i < reader.FieldCount; i++)
                        {
                            result.Numbers.Add(reader.GetInt32(i));
                        }

                        return result;
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return null;
        }
    }
}
