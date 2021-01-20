using Discovery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Discovery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDiscoveryController : ControllerBase
    {
        private const string ServiceDbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Discovery;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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
        public IActionResult GetById(string id)
        {
            return Ok(GetServiceById(id));
        }

        // POST: api/ServiceDiscovery
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            AddService(value);
        }

        // PUT: api/ServiceDiscovery/1
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] JObject value)
        {
            UpdateService(id, value);
        }

        // DELETE: api/ServiceDiscovery/1
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            DeleteService(id);
        }

        private static void AddService(JObject service)
        {
            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO Services (ServiceID,ServiceType,ServiceUri) 
                                                        VALUES (@id, @type, @uri);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.String,
                    Value = service["ServiceID"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = service["ServiceType"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@uri",
                    DbType = DbType.String,
                    Value = service["ServiceUri"].Value<string>()
                });

                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private static void UpdateService(string id, JObject service)
        {
            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE Services SET ServiceType = @type, ServiceUri = @uri WHERE ServiceID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = service["ServiceType"].Value<string>()

                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@uri",
                    DbType = DbType.String,
                    Value = service["ServiceUri"].Value<string>()

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

        private static void DeleteService(string id)
        {
            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM Services WHERE ServiceID = @id;", conn);
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

        public List<Service> GetAllServices()
        {
            var list = new List<Service>();

            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Services;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Service()
                        {
                            Id = reader["ServiceID"].ToString(),
                            Type = reader["ServiceType"].ToString(),
                            Uri = reader["ServiceUri"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public List<Service> GetAllServicesByType(string serviceType)
        {
            var list = new List<Service>();

            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Services WHERE ServiceType = @serviceType;", conn);
                cmd.Parameters.Add(new SqlParameter
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
                            Id = reader["ServiceID"].ToString(),
                            Type = reader["ServiceType"].ToString(),
                            Uri = reader["ServiceUri"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public Service GetServiceById(string id)
        {
            var list = new List<Service>();

            using (var conn = new SqlConnection(ServiceDbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Services WHERE ServiceID = @id;", conn);
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
                        list.Add(new Service()
                        {
                            Id = reader["ServiceID"].ToString(),
                            Type = reader["ServiceType"].ToString(),
                            Uri = reader["ServiceUri"].ToString(),

                        });
                    }
                }
            }
            return list.FirstOrDefault();
        }
    }
}
