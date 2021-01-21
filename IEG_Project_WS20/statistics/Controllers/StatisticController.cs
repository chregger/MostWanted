using Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Statistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private const string DbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Results;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly Logger _logger;

        public StatisticController()
        {
            _logger = new Logger(typeof(StatisticController).FullName);
        }

        // GET: api/statistics
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(GetAllResults());
        }

        // GET: api/statistics/Random
        [HttpGet("survey/{statisticType}")]
        public IActionResult Get(string surveyName)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return Ok(GetAllResultsFromSurvey(surveyName));
        }

        // GET: api/statistics/1
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(GetResultsById(id));
        }

        // POST: api/statistics
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            AddResult(value);
        }

        // PUT: api/statistics/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Newtonsoft.Json.Linq.JObject value)
        {
            UpdateResult(id, value);
        }

        // DELETE: api/statistics/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DeleteResult(id);
        }

        private static void AddResult(JObject result)
        {
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Results (SurveyName, Question, Answer) 
                                                        VALUES (@surveyname, @question, @answer);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyname",
                    DbType = DbType.String,
                    Value = result["SurveyName"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@question",
                    DbType = DbType.String,
                    Value = result["Question"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@answer",
                    DbType = DbType.String,
                    Value = result["Answer"].Value<string>()
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private static void UpdateResult(int id, JObject result)
        {
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE Results SET SurveyName = @surveyname, Question = @question, Answer = @answer
                                                        WHERE ResultID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyname",
                    DbType = DbType.String,
                    Value = result["SurveyName"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@question",
                    DbType = DbType.String,
                    Value = result["Question"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@answer",
                    DbType = DbType.String,
                    Value = result["Answer"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = id
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }

        }

        private static void DeleteResult(int id)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM Results WHERE ResultID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
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

        public List<Statistic> GetAllResults()
        {
            var list = new List<Statistic>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Results;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic
                        {
                            ResultId = Convert.ToInt16(reader["ResultID"]),
                            SurveyName = reader["SurveyName"].ToString(),
                            Question = reader["Question"].ToString(),
                            Answer = reader["Answer"].ToString(),
                        });
                    }
                }
            }

            return list;
        }

        public static List<Statistic> GetAllResultsFromSurvey(string surveyname)
        {
            var list = new List<Statistic>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Results WHERE SurveyName = @surveyname;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyname",
                    DbType = DbType.String,
                    Value = surveyname,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic
                        {
                            ResultId = Convert.ToInt16(reader["ResultID"]),
                            SurveyName = reader["SurveyName"].ToString(),
                            Question = reader["Question"].ToString(),
                            Answer = reader["Answer"].ToString(),
                        });
                    }
                }
            }

            return list;
        }

        public Statistic GetResultsById(int id)
        {
            var list = new List<Statistic>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Results WHERE ResultID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = id,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Statistic
                        {
                            ResultId = Convert.ToInt16(reader["ResultID"]),
                            SurveyName = reader["SurveyName"].ToString(),
                            Question = reader["Question"].ToString(),
                            Answer = reader["Answer"].ToString(),
                        });
                    }
                }
            }

            return list.FirstOrDefault();
        }
    }
}
