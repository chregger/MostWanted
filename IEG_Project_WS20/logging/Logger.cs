using System;
using System.Data;
using System.Data.SqlClient;

namespace Logging
{
    public class Logger
    {
        private const string DbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Logging;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly string _context;

        public Logger(string context)
        {
            _context = context;
        }

        public void Log(string log)
        {
            AddLog(log);
        }

        private void AddLog(string log)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO [dbo].[Logs] (Message,CreatedTime,Context) 
                                                        VALUES (@message, @createdtime, @context);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@message",
                    DbType = DbType.String,
                    Value = log
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@createdtime",
                    DbType = DbType.DateTime2,
                    Value = DateTime.Now
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@context",
                    DbType = DbType.String,
                    Value = _context
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }
    }
}
