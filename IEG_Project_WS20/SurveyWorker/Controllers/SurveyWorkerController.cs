using Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SurveyWorker.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace SurveyWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyWorkerController : ControllerBase
    {
        private const string SurveyDbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private const string ResultDbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Results;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly Logger _logger;

        public SurveyWorkerController()
        {
            _logger = new Logger(typeof(SurveyWorkerController).FullName);
        }

        // GET api/SurveyWorker
        [HttpGet]
        public ActionResult<IEnumerable<Survey>> Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return GetAllSurveys();
        }

        // GET api/SurveyWorker/5
        [HttpGet("{surveyid}")]
        public ActionResult<IEnumerable<Survey>> Get(int surveyid)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            return GetAllInfosFromSurvey(surveyid);
        }

        // POST api/SurveyWorker
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod()?.Name);
            AddResultSurvey(value);
        }

        private static List<Survey> GetAllSurveys()
        {
            var list = new List<Survey>();

            using (SqlConnection conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select * from Surveys;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey
                        {
                            Id = Convert.ToInt32(reader["SurveyID"]),
                            SurveyName = reader["SurveyName"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        private static List<Survey> GetAllInfosFromSurvey(int surveyid)
        {
            var list = new List<Survey>();

            using (SqlConnection conn = new SqlConnection(SurveyDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT s.SurveyID,	s.SurveyName, q.Question, a.Answer
                                                FROM Surveys as s, Questions as q, Answers as a
                                               WHERE q.SurveyID = @surveyid and q.QuestionID = a.QuestionID", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyid",
                    DbType = DbType.Int16,
                    Value = surveyid,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey
                        {
                            Id = Convert.ToInt32(reader["SurveyID"]),
                            SurveyName = reader["SurveyName"].ToString(),
                            Question = reader["Question"].ToString(),
                            Answer = reader["Answer"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        private static void AddResultSurvey(JObject survey)
        {
            var json = JsonConvert.SerializeObject(survey, Formatting.Indented);
            using (var conn = new SqlConnection(ResultDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Results (SurveyName, Question, Answer) 
                                                        VALUES (@surveyname, @question, @answer);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@surveyname",
                    DbType = DbType.String,
                    Value = survey["SurveyName"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@question",
                    DbType = DbType.String,
                    Value = survey["Question"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@answer",
                    DbType = DbType.String,
                    Value = survey["Answer"].Value<string>()
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }
    }
}
