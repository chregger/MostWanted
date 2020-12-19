using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Discovery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discovery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDiscoveryController : ControllerBase
    {
        private const string ServiceDbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=services; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";

        private readonly ILogger<ServiceDiscoveryController> _logger;

        public ServiceDiscoveryController(ILogger<ServiceDiscoveryController> logger)
        {
            _logger = logger;
        }

        // GET: api/ServiceDiscovery
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(GetAllServices());
        }

        // GET: api/ServiceDiscovery/database
        [HttpGet("{serviceType}")]
        public IActionResult Get(string serviceType)
        {
            return Ok(GetAllServicesByType(serviceType));
        }

        // GET: api/ServiceDiscovery/database
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(GetServiceById(id));
        }

        // POST: api/ServiceDiscovery
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            AddService(value["ID"].Value<string>(), value, value["ServiceType"].Value<string>());
        }

        // PUT: api/ServiceDiscovery/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            UpdateService(value["ID"].Value<string>(), value, value["ServiceType"].Value<string>());
        }

        // DELETE: api/ServiceDiscovery/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DeleteService(id);
        }

        private void AddService(string id, JObject service, string type)
        {
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `data` (`id`,`Type`,`Content`) 
                                                        VALUES (@id, @type, @content);", conn);
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
                    Value = service
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

        private void UpdateService(string id, JObject service, string type)
        {
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `data` (`Type`,`Content`) 
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
                    Value = service
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

        private void DeleteService(int id)
        {
            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `data` WHERE id = @id;", conn);
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

        public List<Service> GetAllServices()
        {
            var list = new List<Service>();

            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM DATA;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Service()
                        {
                            Id = reader["id"].ToString(),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public List<Service> GetAllServicesByType(string serviceType)
        {
            var list = new List<Service>();

            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM DATA WHERE `type` = @serviceType;", conn);
                cmd.Parameters.Add(new MySqlParameter
                {
                    ParameterName = "@serviceType",
                    DbType = DbType.String,
                    Value = serviceType,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Service()
                        {
                            Id = reader["id"].ToString(),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public Service GetServiceById(int id)
        {
            var list = new List<Service>();

            using (var conn = new MySqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM DATA WHERE `id` = @id;", conn);
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
                        list.Add(new Service()
                        {
                            Id = reader["id"].ToString(),
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
