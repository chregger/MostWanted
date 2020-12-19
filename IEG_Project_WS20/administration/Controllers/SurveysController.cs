using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Administration.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Administration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController : ControllerBase
    {
        private const string SurveyDbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=Surveys; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";
        private readonly Logger _logger;

        public SurveysController()
        {
            _logger = new Logger(typeof(SurveysController).FullName);
        }
        
        // GET: api/Surveys
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllSurveys());
        }

        // GET: api/Surveys/random
        [HttpGet("{SurveyType}", Name = "Get")]
        public IActionResult GetByType(string surveyType)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllSurveysByType(surveyType));
        }

        // GET: api/Surveys/1
        [HttpGet("id/{id}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetSurveyById(id));
        }

        // POST: api/Surveys
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            AddSurvey(value["ID"].Value<string>(), value, value["SurveyType"].Value<string>());
        }

        // PUT: api/Surveys/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            UpdateSurvey(value["ID"].Value<string>(), value, value["SurveyType"].Value<string>());
        }

        // DELETE: api/Surveys/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            DeleteSurvey(id);
        }

        private void AddSurvey(string id, JObject survey, string type)
        {
            var json = JsonConvert.SerializeObject(survey, Formatting.Indented);
            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `surveys` (`id`,`Type`,`Content`) 
                                                        VALUES (@id, @type, @content);", conn);
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
                    Value = survey
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

        private void UpdateSurvey(string id, JObject survey, string type)
        {
            var json = JsonConvert.SerializeObject(survey, Formatting.Indented);
            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `surveys` (`Type`,`Content`) 
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
                    Value = survey
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

        private void DeleteSurvey(int id)
        {
            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `surveys` WHERE id = @id;", conn);
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

        public List<Survey> GetAllSurveys()
        {
            var list = new List<Survey>();

            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM surveys;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey()
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

        public List<Survey> GetAllSurveysByType(string surveyType)
        {
            var list = new List<Survey>();

            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM surveys WHERE `type` = @SurveyType;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@SurveyType",
                    DbType = DbType.String,
                    Value = surveyType,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey()
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

        public Survey GetSurveyById(int id)
        {
            var list = new List<Survey>();

            using (var conn = new MySqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM surveys WHERE `id` = @id;", conn);
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
                        list.Add(new Survey()
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
