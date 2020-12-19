using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Logging;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Models;

namespace Statistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private const string DbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=results; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";

        private readonly Logger _logger;

        public StatisticController()
        {
            _logger = new Logger(typeof(StatisticController).FullName);
        }

        //// GET: api/statistics
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    return Ok(GetAllstatistics());
        //}

        // GET: api/statistics/Random
        [HttpGet("{statisticType}")]
        public IActionResult Get(string statisticType)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllStatisticsByType(statisticType));
        }

        //// GET: api/statistics/1
        //[HttpGet("id/{id}")]
        //public IActionResult GetById(int id)
        //{
        //    return Ok(GetstatisticById(id));
        //}

        //// POST: api/statistics
        //[HttpPost]
        //public void Post([FromBody] JObject value)
        //{
        //    AddStatistic(value, value["statisticType"].Value<string>());
        //}

        //// PUT: api/statistics/1
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] Newtonsoft.Json.Linq.JObject value)
        //{
        //    Updatestatistic(value["ID"].Value<string>(), value, value["statisticType"].Value<string>());
        //}

        //// DELETE: api/statistics/1
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //    Deletestatistic(id);
        //}

        private void AddStatistic(JObject statistic, string type)
        {
            var json = JsonConvert.SerializeObject(statistic, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `results` (`Type`,`Content`) 
                                                        VALUES (@type, @content);", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = type
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@content",
                    DbType = DbType.String,
                    Value = statistic
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateStatistic(string id, JObject statistic, string type)
        {
            var json = JsonConvert.SerializeObject(statistic, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `results` (`Type`,`Content`) 
                                                        VALUES (@type, @content) WHERE id = @id;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = type
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@content",
                    DbType = DbType.String,
                    Value = statistic
                });
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.String,
                    Value = id
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }

        }

        private void DeleteStatistic(int id)
        {
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `results` WHERE id = @id;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int64,
                    Value = id
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }

        }

        public List<Statistic> GetAllStatistics()
        {
            var list = new List<Statistic>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM results;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic()
                        {
                            Id = Convert.ToInt16(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public List<Statistic> GetAllStatisticsByType(string statisticType)
        {
            var list = new List<Statistic>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM results WHERE `type` = @statisticType;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@statisticType",
                    DbType = DbType.String,
                    Value = statisticType,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic()
                        {
                            Id = Convert.ToInt16(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public Statistic GetStatisticById(int id)
        {
            var list = new List<Statistic>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM results WHERE `id` = @id;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = id,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic()
                        {
                            Id = Convert.ToInt16(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list.FirstOrDefault();
        }
    }
}
