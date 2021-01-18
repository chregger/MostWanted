using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Administration.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Administration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController : ControllerBase
    {
        private const string SurveyDbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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
            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO `surveys` (`id`,`Type`,`Content`) 
                                                        VALUES (@id, @type, @content);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = type
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@content",
                    DbType = DbType.String,
                    Value = survey
                });
                cmd.Parameters.Add(new SqlParameter
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
            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE `surveys` (`Type`,`Content`) 
                                                        VALUES (@type, @content) WHERE id = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = type
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@content",
                    DbType = DbType.String,
                    Value = survey
                });
                cmd.Parameters.Add(new SqlParameter
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
            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM `surveys` WHERE id = @id;", conn);
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

        public List<Survey> GetAllSurveys()
        {
            var list = new List<Survey>();

            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM surveys;", conn);

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

            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM surveys WHERE `type` = @SurveyType;", conn);
                cmd.Parameters.Add(new SqlParameter
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

            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM surveys WHERE `id` = @id;", conn);
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
