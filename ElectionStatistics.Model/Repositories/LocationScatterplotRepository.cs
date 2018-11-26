using System;
using System.Collections.Generic;
using System.Data.SqlClient;

// DEBUG
using System.Diagnostics;

namespace ElectionStatistics.Model
{
    public class LocationScatterplotRepository : RawSQLRepository
    {
        public class LocationScatterResult
        {
            public List<CalculatedProtocol> Protocols { get; set; }
            public decimal MaxY { get; set; }

        }
        public class CalculatedProtocol
        {
            public decimal Y { get; set; }
            public string TitleRus { get; set; }
            public int ParentId { get; set; }
            public int CommissionNumber { get; set; }
        }

        private CalculateValuesRepository cvr;

        public LocationScatterplotRepository(SqlConnection connection,
            CalculateValuesRepository cvr) : base(connection)
        {
            this.cvr = cvr;
        }

        public LocationScatterResult Query(int protocolSetId, int? protocolId,
            string presetExpression)
        {
            var sw = new Stopwatch();
            sw.Start();

            string sql;
            SqlCommand command = new SqlCommand();

            if (protocolId != null)
            {
                sql = @"WITH query AS
                (SELECT p1.Id, p1.ParentId, p1.TitleRus, p1.CommissionNumber, p1.IsLeaf
                    FROM protocols p1
                    WHERE p1.ParentId = @protocolId
                    UNION ALL
                    SELECT p2.Id, p2.ParentId, p2.TitleRus, p2.CommissionNumber, p2.IsLeaf
                    FROM protocols p2
                    JOIN query ON p2.ParentId = query.Id)

                SELECT %presetExpression% AS Y, TitleRus, ParentId, CommissionNumber
                FROM query
                JOIN %calculateTable% Z ON (Z.ProtocolId = query.Id)
                WHERE IsLeaf = 1";

                command.Parameters.AddWithValue("@protocolId", protocolId);
            }
            else
            {
                sql = @"SELECT %presetExpression% AS Y, P.TitleRus,
                    P.ParentId,
                    P.CommissionNumber
                FROM %calculateTable% Z
                JOIN Protocols P ON (P.Id = Z.ProtocolId)";
            }

            sql = sql.Replace("%presetExpression%", presetExpression)
                .Replace("%calculateTable%", cvr.GetTableName(protocolSetId));

            Console.WriteLine($"sql={sql}");

            LocationScatterResult results = new LocationScatterResult()
            {
                Protocols = new List<CalculatedProtocol>()
            };

            command.Connection = connection;
            command.CommandText = sql;

            connection.Open();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    results.MaxY = Decimal.MinValue;

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            continue;
                        }

                        CalculatedProtocol p = new CalculatedProtocol()
                        {
                            Y = reader.GetDecimal(0),
                            TitleRus = reader.GetString(1),
                            ParentId = reader.GetInt32(2),
                            CommissionNumber = reader.GetInt32(3)
                        };

                        if (p.Y > results.MaxY)
                        {
                            results.MaxY = p.Y;
                        }

                        results.Protocols.Add(p);
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            sw.Stop();
            Console.WriteLine($"Query time elapsed: {sw.Elapsed}");

            return results;
        }
    }
}
