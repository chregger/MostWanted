using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Administration.Models;
using Logging;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Administration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private const string DbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=surveys; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";
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

        // GET: api/Questions/Random
        [HttpGet("{QuestionType}")]
        public IActionResult GetByType(string questionType)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllQuestionsByType(questionType));
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
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `questions` (`Type`,`Content`) 
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
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `questions` (`Type`,`Content`) 
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
                    Value = question
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

        private void DeleteQuestion(int id)
        {
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `questions` WHERE id = @id;", conn);
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

        public List<Question> GetAllQuestions()
        {
            var list = new List<Question>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM questions;", conn);

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

        public List<Question> GetAllQuestionsByType(string questionType)
        {
            var list = new List<Question>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM questions WHERE `type` = @QuestionType;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@QuestionType",
                    DbType = DbType.String,
                    Value = questionType,
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

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM questions WHERE `id` = @id;", conn);
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
