using Logging;
using Microsoft.AspNetCore.Mvc;
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
        private const string URL_BASE_1 = "http://surveyworker";
        private const string URL_BASE_2 = ".azurewebsites.net/api/AcceptedCreditCards";
        private string currentUrl = "http://surveyworker0.azurewebsites.net/api/AcceptedCreditCards";
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
            return GetCall(currentUrl, "");
        }

        // GET api/values/5
        [HttpGet("{survey}")]
        public ActionResult<string> Get(String survey)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetCall(currentUrl, "/" + survey);
        }

        // POST api/values
        [HttpPost("{type}")]
        public void Post(String type, [FromBody] string value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            PostCall(currentUrl, value, type);
        }

        private void PostCall(string url, string value, string type)
        {
            try
            {
                RestCallPost(url + "/" + type, value);
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
                PostCall(currentUrl, value, type);
            }
        }

        private string GetCall(string url, string type)
        {
            try
            {
                return RestCall(url, type);
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
                return RestCall(currentUrl, type);
            }
        }


        private void RestCallPost(string url, string value)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        private string RestCall(string url, string type)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + type);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            //httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

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
