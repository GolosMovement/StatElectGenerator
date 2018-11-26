using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ElectionStatistics.Model
{
    // TODO: tests, DRY, use transactions
    public class CalculateValuesRepository : RawSQLRepository
    {
        public CalculateValuesRepository(SqlConnection connection) : base(connection)
        {
        }

        public string BuildTable(int protocolSetId)
        {
            var lineDescriptionsIds = GetLineDescriptionsIds(protocolSetId);

            if (lineDescriptionsIds.Count == 0)
            {
                return null;
            }

            var tableName = GetTableName(protocolSetId);
            CreateCalcTable(tableName, lineDescriptionsIds);
            CreateProtocolIdIndex(tableName);

            return tableName;
        }

        public void RemoveTable(int protocolSetId)
        {
            var sql = "DROP TABLE " + GetTableName(protocolSetId);
            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();

            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        public bool Exists(int protocolSetId)
        {
            return ObjectExists(GetTableName(protocolSetId));
        }

        public string GetTableName(int protocolSetId)
        {
            return $"ZCalculateValues{protocolSetId}";
        }

        private List<int> GetLineDescriptionsIds(int protocolSetId)
        {
            var lineDescriptionsSql = @"SELECT LineDescriptions.Id
                FROM LineDescriptions
                WHERE LineDescriptions.ProtocolSetId = @protocolSetId AND
                    LineDescriptions.IsCalcResult = 1";

            SqlCommand command = new SqlCommand(lineDescriptionsSql, connection);
            command.Parameters.AddWithValue("@protocolSetId", protocolSetId);

            connection.Open();

            var result = new List<int>();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        private void CreateProtocolIdIndex(string tableName)
        {
            var indexName = $"IX_{tableName}_ProtocolId";
            var indexSql = $"CREATE NONCLUSTERED INDEX [{indexName}] ON [dbo].[{tableName}] " +
                "([ProtocolId] ASC)";

            SqlCommand command = new SqlCommand(indexSql, connection);
            connection.Open();

            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        private void CreateCalcTable(string tableName, List<int> lineDescriptionsIds)
        {
            var baseSql =
                @"SELECT ProtocolId,
                    %lineDescriptionsAsFields%
                INTO %tableName%
                FROM
                (
                    SELECT CAST(LineNumbers.Value AS DECIMAL) AS Value,
                        LineNumbers.LineDescriptionId,
                        LineNumbers.ProtocolId
                    FROM LineNumbers
                    WHERE LineNumbers.LineDescriptionId in (%lineDescriptionsIds%)
                    ) AS sourceQuery
                    PIVOT
                    (
                        MAX(Value) FOR LineDescriptionId IN (%lineDescriptionsAsFields%)
                ) AS pivotQuery";

            var ldsAsFields = "[" + string.Join("],[", lineDescriptionsIds) + "]";
            var lds = string.Join(",", lineDescriptionsIds);

            var sql = baseSql
                .Replace("%lineDescriptionsAsFields%", ldsAsFields)
                .Replace("%lineDescriptionsIds%", lds)
                .Replace("%tableName%", tableName);

            SqlCommand command = new SqlCommand(sql, connection);
            connection.Open();

            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
