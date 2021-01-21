using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace Statistics
{
    public class Startup
    {
        private readonly Guid _serviceId = Guid.NewGuid();
        private const string ServiceType = "Statistics";
        private const string ServiceUri = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddXmlSerializerFormatters();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder appBuilder, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                appBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                appBuilder.UseHsts();
            }

            appBuilder.UseHttpsRedirection();

            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Statistics API V1");
            });
            
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnStartup()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mostwanteddiscovery.azurewebsites.net/api/ServiceDiscovery/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JObject
                {
                    ["ServiceID"] = _serviceId,
                    ["ServiceType"] = ServiceType,
                    ["ServiceUri"] = ServiceUri
                };
                streamWriter.Write(json);
            }

            httpWebRequest.BeginGetResponse(null, null);
        }

        private void OnShutdown()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mostwanteddiscovery.azurewebsites.net/api/ServiceDiscovery/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            httpWebRequest.BeginGetResponse(null, null);
        }
    }
}
