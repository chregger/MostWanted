using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace SurveyWorker
{
    public class Startup
    {
        private readonly Guid _serviceId = Guid.NewGuid();
        private const string ServiceType = "SurveyWorker";
        private const string ServiceUri = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true; // false by default
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddXmlSerializerFormatters();
            //services.AddMvc(options => { options.OutputFormatters.Insert(0, new XmlOutputFormatter()); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnStartup()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://most-wanted-discovery.azurewebsites.net/api/ServiceDiscovery/");
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:44323/api/ServiceDiscovery");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JObject
                {
                    ["ID"] = _serviceId,
                    ["ServiceType"] = ServiceType,
                    ["ServiceUri"] = ServiceUri
                };

                streamWriter.Write(json);
            }

            httpWebRequest.BeginGetResponse(null, null);
        }

        private void OnShutdown()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://most-wanted-discovery.azurewebsites.net/api/ServiceDiscovery/id/" + _serviceId);
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:44323/api/ServiceDiscovery");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            httpWebRequest.BeginGetResponse(null, null);
        }
    }
}
