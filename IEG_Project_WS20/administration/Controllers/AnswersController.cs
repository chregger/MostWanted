﻿using Administration.Models;
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
    public class AnswersController : ControllerBase
    {
        private const string DbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly Logger _logger;

        public AnswersController()
        {
            _logger = new Logger(typeof(AnswersController).FullName);
        }

        // GET: api/Answers
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return Ok(GetAllAnswers());
        }

        // GET: api/Answers/1
        [HttpGet("{QuestionID}")]
        public IActionResult GetAnswersFromQuestion(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return Ok(GetAllAnswersByQuestion(id));
        }

        // GET: api/Answers/1
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return Ok(GetAnswerById(id));
        }

        // POST: api/Answers
        [HttpPost]
        public IActionResult Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            AddAnswer(value);
            return Ok();
        }

        // PUT: api/Answers/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            UpdateAnswer(value["ID"].Value<string>(), value);
            return Ok();
        }

        // DELETE: api/Answers/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            DeleteAnswer(id);
            return Ok();
        }

        private static void AddAnswer(JObject answer)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Answers (QuestionID, Answer) 
                                                        VALUES (@questionID, @Answer);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@questionID",
                    DbType = DbType.Int32,
                    Value = answer["QuestionID"].Value<int>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@Answer",
                    DbType = DbType.String,
                    Value = answer["Answer"].Value<string>()
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private static void UpdateAnswer(string id, JObject answer)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE Answers SET QuestionID = @questionID, Answer = @answer WHERE AnswerID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@questionID",
                    DbType = DbType.Int32,
                    Value = answer["QuestionID"].Value<int>()

                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@answer",
                    DbType = DbType.String,
                    Value = answer["Answer"].Value<string>()

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

        private static void DeleteAnswer(int id)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM Answers WHERE AnswerID = @id;", conn);
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

        private static List<Answer> GetAllAnswers()
        {
            var list = new List<Answer>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Answers;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Answer
                        {
                            AnswerId = Convert.ToInt16(reader["AnswerID"]),
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            AnswerText = reader["Answer"].ToString(),

                        });
                    }
                }
            }

            return list;
        }

        private static List<Answer> GetAllAnswersByQuestion(int questionId)
        {
            var list = new List<Answer>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM answers WHERE QuestionID = @questionID;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@questionID",
                    DbType = DbType.String,
                    Value = questionId,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Answer
                        {
                            AnswerId = Convert.ToInt16(reader["AnswerID"]),
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            AnswerText = reader["Answer"].ToString(),

                        });
                    }
                }
            }

            return list;
        }

        private static Answer GetAnswerById(int id)
        {
            var list = new List<Answer>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM answers WHERE AnswerID = @id;", conn);
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
                        list.Add(new Answer
                        {
                            AnswerId = Convert.ToInt16(reader["AnswerID"]),
                            QuestionId = Convert.ToInt16(reader["QuestionID"]),
                            AnswerText = reader["Answer"].ToString(),

                        });
                    }
                }
            }

            return list.FirstOrDefault();
        }
    }
}
