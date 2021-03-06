﻿using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace Administration
{
    public class Startup
    {
        private readonly Guid _serviceId = Guid.NewGuid();
        private const string ServiceType = "Administration";
        private const string ServiceUri = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddXmlSerializerFormatters();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Administration", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment env, Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                appBuilder.UseDeveloperExceptionPage();
                appBuilder.UseSwagger();
                appBuilder.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Administration v1"));
            }
            else
            {
                appBuilder.UseHsts();
            }

            appBuilder.UseHttpsRedirection();

            appBuilder.UseRouting();

            appBuilder.UseAuthorization();

            appBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            OnStartup();

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            //GetDatabasePassword();
        }

        private void GetDatabasePassword()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://most-wanted-secrets.azurewebsites.net/api/Secrets/db_password");
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:44323/api/ServiceDiscovery");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
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
                    ["ServiceID"] = _serviceId, ["ServiceType"] = ServiceType, ["ServiceUri"] = ServiceUri
                };

                streamWriter.Write(json);
            }

            httpWebRequest.BeginGetResponse(null, null);
        }

        private void OnShutdown()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://mostwanteddiscovery.azurewebsites.net/api/ServiceDiscovery/" + _serviceId);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";

            httpWebRequest.BeginGetResponse(null, null);
        }
    }
}
