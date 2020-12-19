using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Secrets.Models;

namespace Secrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretsController : ControllerBase
    {
        private const string DbConnectionString = "Server=most-wanted-database.mysql.database.azure.com; Port=3306; Database=secrets; Uid=mostwanted@most-wanted-database; Pwd=start1234@; SslMode=Preferred;";
        private readonly Logger _logger;

        public SecretsController()
        {
            _logger = new Logger(typeof(SecretsController).FullName);
        }

        //// GET: api/Secrets
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    return Ok(GetAllSecrets());
        //}

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

        //// POST: api/Secrets
        //[HttpPost]
        //public void Post([FromBody] JObject value)
        //{
        //    AddSecret(value, value["SecretType"].Value<string>());
        //}

        //// PUT: api/Secrets/1
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] Newtonsoft.Json.Linq.JObject value)
        //{
        //    UpdateSecret(value["ID"].Value<string>(), value, value["SecretType"].Value<string>());
        //}

        //// DELETE: api/Secrets/1
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //    DeleteSecret(id);
        //}

        private void AddSecret(JObject secret, string type)
        {
            var json = JsonConvert.SerializeObject(secret, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"INSERT INTO `Secrets` (`Type`,`Content`) 
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
                    Value = secret
                });

                Console.WriteLine(json);
                using (var reader = cmd.ExecuteReader())
                {

                }
            }
        }

        private void UpdateSecret(string id, JObject secret, string type)
        {
            var json = JsonConvert.SerializeObject(secret, Formatting.Indented);
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"UPDATE `Secrets` (`Type`,`Content`) 
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
                    Value = secret
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

        private void DeleteSecret(int id)
        {
            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"DELETE FROM `Secrets` WHERE id = @id;", conn);
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

        public List<Secret> GetAllSecrets()
        {
            var list = new List<Secret>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Secrets;", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Secret()
                        {
                            Id = Convert.ToInt16(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public List<Secret> GetAllSecretsByType(string secretType)
        {
            var list = new List<Secret>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM Secrets WHERE `type` = @SecretType;", conn);
                cmd.Parameters.Add(new MySqlParameter
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
                            Id = Convert.ToInt16(reader["id"]),
                            Type = reader["type"].ToString(),
                            Content = reader["content"].ToString(),

                        });
                    }
                }
            }
            return list;
        }

        public Secret GetSecretById(int id)
        {
            var list = new List<Secret>();

            using (var conn = new MySqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM secrets WHERE `id` = @id;", conn);
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
                        list.Add(new Secret()
                        {
                            Id = Convert.ToInt16(reader["id"]),
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
