using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Administration.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Administration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private const string DbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly Logger _logger;

        public QuestionsController()
        {
            _logger = new Logger(typeof(QuestionsController).FullName);
        }

        // GET: api/Questions
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllQuestions());
        }

        // GET: api/Survey/Random
        [HttpGet("Survey/{SurveyID}")]
        public IActionResult GetBySurvey(int SurveyID)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllQuestionsBySurvey(SurveyID));
        }

        // GET: api/Questions/1
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetQuestionById(id));
        }

        // POST: api/Questions
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            AddQuestion(value, value["QuestionType"].Value<string>());
        }

        // PUT: api/Questions/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            UpdateQuestion(value["ID"].Value<string>(), value, value["QuestionType"].Value<string>());
        }

        // DELETE: api/Questions/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            DeleteQuestion(id);
        }

        private void AddQuestion(JObject question, string type)
        {
            var json = JsonConvert.SerializeObject(question, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO `questions` (`Type`,`Content`) 
                                                        VALUES (@type, @content);", conn);
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
                    Value = question
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateQuestion(string id, JObject question, string type)
        {
            var json = JsonConvert.SerializeObject(question, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE `questions` (`Type`,`Content`) 
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
                    Value = question
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

        private void DeleteQuestion(int id)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM `questions` WHERE id = @id;", conn);
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

        public List<Question> GetAllQuestions()
        {
            var list = new List<Question>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM questions;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Question()
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

        public List<Question> GetAllQuestionsBySurvey(int surveyID)
        {
            var list = new List<Question>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM questions WHERE `type` = @QuestionType;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@QuestionType",
                    DbType = DbType.String,
                    Value = surveyID,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Question()
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

        public Question GetQuestionById(int id)
        {
            var list = new List<Question>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM questions WHERE `id` = @id;", conn);
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
                        list.Add(new Question()
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
