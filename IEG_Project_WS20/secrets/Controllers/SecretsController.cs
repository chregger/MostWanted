using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Secrets.Models;
using System.Data.SqlClient;

namespace Secrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretsController : ControllerBase
    {
        private const string DbConnectionString = "Server=tcp:most-wanted.database.windows.net,1433;Initial Catalog=Secrets;Persist Security Info=False;User ID=dbuser;Password=IEG_WS2020;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private readonly Logger _logger;

        public SecretsController()
        {
            _logger = new Logger(typeof(SecretsController).FullName);
        }

        // GET: api/Secrets
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(GetAllSecrets());
        }

        // GET: api/Secrets/Random
        [HttpGet("{SecretType}")]
        public IActionResult Get(string secretType)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetAllSecretsByType(secretType));
        }

        // GET: api/Secrets/1
        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.Log(MethodBase.GetCurrentMethod().Name);
            return Ok(GetSecretById(id));
        }

        // POST: api/Secrets
        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            AddSecret(value);
        }

        // PUT: api/Secrets/1
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] JObject value)
        {
            UpdateSecret(id, value);
        }

        // DELETE: api/Secrets/1
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DeleteSecret(id);
        }

        private void AddSecret(JObject secret)
        {
            var json = JsonConvert.SerializeObject(secret, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO `Secrets` (`SecretID`, `SecretType`,`Password`) 
                                                        VALUES (@id, @type, @password);", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = secret["SecretType"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@password",
                    DbType = DbType.String,
                    Value = secret["Password"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = secret["SecretID"].Value<string>()
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateSecret(int id, JObject secret)
        {
            var json = JsonConvert.SerializeObject(secret, Formatting.Indented);
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE `Secrets` (`SecretType`,`Password`) 
                                                        VALUES (@type, @password) WHERE SecretID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@type",
                    DbType = DbType.String,
                    Value = secret["SecretType"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@password",
                    DbType = DbType.String,
                    Value = secret["Password"].Value<string>()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@id",
                    DbType = DbType.Int32,
                    Value = id
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }

        }

        private void DeleteSecret(int id)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"DELETE FROM `Secrets` WHERE SecretID = @id;", conn);
                cmd.Parameters.Add(new SqlParameter
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

        public List<Secret> GetAllSecrets()
        {
            var list = new List<Secret>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Secrets;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Secret()
                        {
                            SecretID = Convert.ToInt16(reader["SecretID"]),
                            SecretType = reader["SecretType"].ToString(),
                            Password = reader["Password"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public List<Secret> GetAllSecretsByType(string secretType)
        {
            var list = new List<Secret>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM Secrets WHERE `SecretType` = @SecretType;", conn);
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@SecretType",
                    DbType = DbType.String,
                    Value = secretType,
                });

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Secret()
                        {
                            SecretID = Convert.ToInt16(reader["SecretID"]),
                            SecretType = reader["SecretType"].ToString(),
                            Password = reader["Password"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public Secret GetSecretById(int id)
        {
            var list = new List<Secret>();

            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM secrets WHERE `SecretID` = @id;", conn);
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
                        list.Add(new Secret()
                        {
                            SecretID = Convert.ToInt16(reader["SecretID"]),
                            SecretType = reader["SecretType"].ToString(),
                            Password = reader["Password"].ToString(),

                        });
                    }
                }
            }
            return list.FirstOrDefault();
        }
    }
}
