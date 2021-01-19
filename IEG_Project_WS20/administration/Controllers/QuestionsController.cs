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
        public IActionResult GetQuestionsFromSurvey(int SurveyID)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetQuestionsFromSurvey(SurveyID));
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
            AddQuestion(value);
        }

        // PUT: api/Questions/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            UpdateQuestion(id, value);
        }

        // DELETE: api/Questions/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            DeleteQuestion(id);
        }

        private void AddQuestion(JObject question)
        {
            var json = JsonConvert.SerializeObject(question, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Questions (SurveyID, Question) 
                                                        VALUES (@surveyid, @question);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyid",
                    DbType = DbType.Int32,
                    Value = question["SurveyID"].Value<int>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@question",
                    DbType = DbType.String,
                    Value = question["Question"].Value<string>()

                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateQuestion(int id, JObject question)
        {
            var json = JsonConvert.SerializeObject(question, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE Questions SET SurveyID = @surveyID, Question = @question WHERE QuestionID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyID",
                    DbType = DbType.Int32,
                    Value = question["SurveyID"].Value<int>()

                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@question",
                    DbType = DbType.String,
                    Value = question["Question"].Value<int>()

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

        private void DeleteQuestion(int id)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM Questions WHERE QuestionID = @id;", conn);
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
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            SurveyID = Convert.ToInt16(reader["SurveyID"]),
                            Questiontext = reader["Question"].ToString(),
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
                var cmd = new SqlCommand(@"SELECT * FROM questions WHERE SurveyID = @surveyid;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyid",
                    DbType = DbType.Int32,
                    Value = surveyID,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Question()
                        {
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            SurveyID = Convert.ToInt16(reader["SurveyID"]),
                            Questiontext = reader["Question"].ToString(),
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
                var cmd = new SqlCommand(@"SELECT * FROM questions WHERE QuestionID = @id;", conn);
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
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            SurveyID = Convert.ToInt16(reader["SurveyID"]),
                            Questiontext = reader["Question"].ToString(),

                        });
                    }
                }
            }
            return list.FirstOrDefault();
        }
    }
}
