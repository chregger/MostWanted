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
    public class AnswersController : ControllerBase
    {
        private const string DbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=surveys; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";
        private readonly Logger _logger;

        public AnswersController()
        {
            _logger = new Logger(typeof(AnswersController).FullName);
        }

        // GET: api/Answers
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllAnswers());
        }

        // GET: api/Answers/Random
        [HttpGet("{AnswerType}")]
        public IActionResult GetByType(string answerType)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllAnswersByType(answerType));
        }

        // GET: api/Answers/1
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAnswerById(id));
        }

        // POST: api/Answers
        [HttpPost]
        public IActionResult Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            AddAnswer(value, value["AnswerType"].Value<string>());
            return Ok();
        }

        // PUT: api/Answers/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            UpdateAnswer(value["ID"].Value<string>(), value, value["AnswerType"].Value<string>());
            return Ok();
        }

        // DELETE: api/Answers/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            DeleteAnswer(id);
            return Ok();
        }

        private void AddAnswer(JObject answer, string type)
        {
            var json = JsonConvert.SerializeObject(answer, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `answers` (`Type`,`Content`) 
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
                    Value = answer
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateAnswer(string id, JObject answer, string type)
        {
            var json = JsonConvert.SerializeObject(answer, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `answers` (`Type`,`Content`) 
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
                    Value = answer
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

        private void DeleteAnswer(int id)
        {
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `answers` WHERE id = @id;", conn);
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

        public List<Answer> GetAllAnswers()
        {
            var list = new List<Answer>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM answers;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Answer()
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

        public List<Answer> GetAllAnswersByType(string answerType)
        {
            var list = new List<Answer>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM answers WHERE `type` = @AnswerType;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@AnswerType",
                    DbType = DbType.String,
                    Value = answerType,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Answer()
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

        public Answer GetAnswerById(int id)
        {
            var list = new List<Answer>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM answers WHERE `id` = @id;", conn);
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
                        list.Add(new Answer()
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
