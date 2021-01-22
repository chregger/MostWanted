using Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Survey.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {

        //private const int MAX_RETRIES = 2;
        private const int SERVICES = 1;
        private int CURRENT_SERVICE = 0;
        private const string URL_BASE_1 = "https://mostwantedsurveyworker";
        private const string URL_BASE_2 = ".azurewebsites.net/api/SurveyWorker/";
        private string currentUrl = "https://mostwantedsurveyworker.azurewebsites.net/api/SurveyWorker/";
        //private int RETRY = 0;
        private readonly Logger _logger;

        public SurveyController()
        {
            _logger = new Logger(typeof(SurveyController).FullName);
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetCall(currentUrl);
        }

        // GET api/values/5
        [HttpGet("{survey}")]
        public ActionResult<string> Get(String survey)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetCall(currentUrl);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            PostCall(currentUrl, value);
        }

        private void PostCall(string url, JObject value)
        {
            try
            {
                RestCallPost(url, value);
            }
            catch
            {
                if (CURRENT_SERVICE == SERVICES)
                {
                    CURRENT_SERVICE = 1;
                }
                else
                {
                    CURRENT_SERVICE++;
                }
                currentUrl = URL_BASE_1 + CURRENT_SERVICE + URL_BASE_2;
                PostCall(currentUrl, value);
            }
        }

        private string GetCall(string url)
        {
            try
            {
                return RestCall(url);
            }
            catch
            {
                if (CURRENT_SERVICE == SERVICES)
                {
                    CURRENT_SERVICE = 1;
                }
                else
                {
                    CURRENT_SERVICE++;
                }
                currentUrl = URL_BASE_1 + CURRENT_SERVICE + URL_BASE_2;
                return RestCall(currentUrl);
            }
        }


        private void RestCallPost(string url, JObject value)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JObject
                {
                    ["SurveyName"] = value["SurveyName"].ToString(),
                    ["Question"] = value["Question"].ToString(),
                    ["Answer"] = value["Answer"].ToString()
                };

                streamWriter.Write(json);
            }

            //httpWebRequest.BeginGetResponse(null, null);
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }   
        }

        private string RestCall(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var content = string.Empty;

            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    content = streamReader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
