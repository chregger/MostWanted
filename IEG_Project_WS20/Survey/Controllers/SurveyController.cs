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
        private int _currentService = 0;
        private const string UrlBase1 = "http://surveyworker";
        private const string UrlBase2 = ".azurewebsites.net/api/AcceptedCreditCards";
        private string _currentUrl = "http://surveyworker0.azurewebsites.net/api/AcceptedCreditCards";
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
            return GetCall(_currentUrl, "");
        }

        // GET api/values/5
        [HttpGet("{survey}")]
        public ActionResult<string> Get(string survey)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return GetCall(_currentUrl, "/" + survey);
        }

        // POST api/values
        [HttpPost("{type}")]
        public void Post(string type, [FromBody] string value)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            PostCall(_currentUrl, value, type);
        }

        private void PostCall(string url, string value, string type)
        {
            try
            {
                RestCallPost(url + "/" + type, value);
            }
            catch
            {
                if (_currentService == SERVICES)
                {
                    _currentService = 1;
                }
                else
                {
                    _currentService++;
                }
                _currentUrl = UrlBase1 + _currentService + UrlBase2;
                PostCall(_currentUrl, value, type);
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
                if (_currentService == SERVICES)
                {
                    _currentService = 1;
                }
                else
                {
                    _currentService++;
                }
                _currentUrl = UrlBase1 + _currentService + UrlBase2;
                return RestCall(_currentUrl, type);
            }
        }


        private static void RestCallPost(string url, string value)
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

        private static string RestCall(string url, string type)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + type);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            //httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            string content;

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
