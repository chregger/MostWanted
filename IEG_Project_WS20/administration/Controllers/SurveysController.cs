using Administration.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

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
        [HttpGet("{SurveyName}")]
        public IActionResult GetByName(string surveyName)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllSurveysByName(surveyName));
        }

        // GET: api/Surveys/1
        [HttpGet("id/{id}")]
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
            AddSurvey(value);
        }

        // PUT: api/Surveys/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            UpdateSurvey(id, value);
        }

        // DELETE: api/Surveys/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            DeleteSurvey(id);
        }

        private void AddSurvey(JObject survey)
        {
            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Surveys (SurveyName) 
                                                        VALUES (@name);", conn);

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@name",
                    DbType = DbType.String,
                    Value = survey["SurveyName"].Value<string>()
                });
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateSurvey(int id, JObject survey)
        {
            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE surveys SET SurveyName = @name WHERE id = @id;", conn);

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@name",
                    DbType = DbType.String,
                    Value = survey["SurveyName"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.String,
                    Value = id
                });
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
                var cmd = new SqlCommand(@"DELETE FROM surveys WHERE id = @id;", conn);
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
                            Id = Convert.ToInt16(reader["SurveyID"]),
                            Name = reader["SurveyName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<Survey> GetAllSurveysByName(string surveyname)
        {
            var list = new List<Survey>();

            using (var conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Surveys WHERE SurveyName = @SurveyName;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@SurveyName",
                    DbType = DbType.String,
                    Value = surveyname
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey()
                        {
                            Id = Convert.ToInt16(reader["SurveyID"]),
                            Name = reader["SurveyName"].ToString()
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
                var cmd = new SqlCommand(@"SELECT * FROM surveys WHERE id = @id;", conn);
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
                            Id = Convert.ToInt16(reader["SurveyID"]),
                            Name = reader["SurveyName"].ToString(),
                        });
                    }
                }
            }
            return list.FirstOrDefault();
        }
    }
}
