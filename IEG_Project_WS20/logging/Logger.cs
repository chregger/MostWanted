using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Logging
{
    public class Logger
    {
        private const string DbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=logs; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";
        private readonly string _type;

        public Logger(string type)
        {
            _type = type;
        }

        public void Log(string log)
        {
            AddLog(log);
        }

        private void AddLog(string log)
        {
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `logs` (`type`,`content`,`timestamp`) 
                                                        VALUES (@type, @content, @timestamp);", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = _type
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@content",
                    DbType = DbType.String,
                    Value = log
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@timestamp",
                    DbType = DbType.DateTime,
                    Value = DateTime.Now
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }
    }
}
