using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SurveyWorker.Model;

namespace SurveyWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyWorkerController : ControllerBase
    {
        private string SurveyDBConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private string ResultDBConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Surveys;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly Logger _logger;

        public SurveyWorkerController()
        {
            _logger = new Logger(typeof(SurveyWorkerController).FullName);
        }
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Survey>> Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetAllSurveyRows();
        }

        // GET api/values/5
        [HttpGet("{survey}")]
        public ActionResult<IEnumerable<Survey>> Get(String survey)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetAllSurveyRowsBySurvey(survey);
        }

        // POST api/values
        [HttpPost("{type}")]
        public void Post([FromBody] Newtonsoft.Json.Linq.JObject value, String type)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            AddResultSurvey(value, type);
        }

        public List<Survey> GetAllSurveyRows()
        {
            List<Survey> list = new List<Survey>();

            using (MySqlConnection conn = new MySqlConnection(SurveyDBConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Surveys;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),
                           
                        });
                    }
                }
            }
            return list;
        }

        public List<Survey> GetAllSurveyRowsBySurvey(String survey)
        {
            List<Survey> list = new List<Survey>();

            using (MySqlConnection conn = new MySqlConnection(SurveyDBConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"select * from Surveys where `type` = @survey;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@survey",
                    DbType = DbType.String,
                    Value = survey,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Survey()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public void AddResultSurvey(Newtonsoft.Json.Linq.JObject survey, string type)
        {
            using (MySqlConnection conn = new MySqlConnection(ResultDBConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO `data` (`Type`,`Content`) 
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
                    Value = survey
                });
                string json = JsonConvert.SerializeObject(survey, Formatting.Indented);

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        //public void AddResultSurvey(List<SurveyResult> surveyRows)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(SurveyDBConnectionString))
        //    {
        //        conn.Open();
        //        foreach (SurveyResult surveyRow in surveyRows)
        //        {
        //            MySqlCommand cmd = new MySqlCommand(@"INSERT INTO `SurveyResult` (`Survey`,`Name`,`Content`,`Type`) 
        //                                                    VALUES (@survey, @name, @content, @type);", conn);
        //            cmd.Parameters.Add(new MySqlParameter
        //            {
        //                ParameterName = "@survey",
        //                DbType = DbType.String,
        //                Value = surveyRow.Survey
        //            });
        //            cmd.Parameters.Add(new MySqlParameter
        //            {
        //                ParameterName = "@name",
        //                DbType = DbType.String,
        //                Value = surveyRow.Name
        //            });
        //            cmd.Parameters.Add(new MySqlParameter
        //            {
        //                ParameterName = "@content",
        //                DbType = DbType.String,
        //                Value = surveyRow.Content
        //            });
        //            cmd.Parameters.Add(new MySqlParameter
        //            {
        //                ParameterName = "@type",
        //                DbType = DbType.String,
        //                Value = surveyRow.Type
        //            });

        //            using (var reader = cmd.ExecuteReader())
        //            {

        //            }
        //        }
        //    }
        //}

    }
}
