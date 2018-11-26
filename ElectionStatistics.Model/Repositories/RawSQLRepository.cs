using System;
using System.Data.SqlClient;

namespace ElectionStatistics.Model
{
    public abstract class RawSQLRepository
    {
        protected SqlConnection connection;

        public RawSQLRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public bool ObjectExists(string objectName)
        {
            var sql = $"SELECT OBJECT_ID('{objectName}')";
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        if (!reader.Read())
                        {
                            return false;
                        }

                        return !reader.IsDBNull(0);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
