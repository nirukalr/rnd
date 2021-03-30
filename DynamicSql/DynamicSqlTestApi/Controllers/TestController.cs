using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicSqlTestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly string _connectionString =
            "Password=Sqlp@$$W0rd;Persist Security Info=True;User ID=sa;Initial Catalog=AL_SECURITY;Data Source=.";
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpPost("test-search")]
        public async Task<IActionResult> TestSearch([FromBody] SearchData searchData)
        {
            var url = "https://localhost:44333/test/search";
            using HttpClient client = new HttpClient();
            var response = await client.PostDataAsync(url, searchData);
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                //DataSet data = JsonConvert.DeserializeObject<DataSet>(jsonString);
                //DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonString, (typeof(DataTable)));
                var data = Tabulate(jsonString);
                return Ok(data);
            }

            return BadRequest();
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchData searchData)
        {
            await using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<dynamic>(searchData.Query);

            return Ok(new SearchResult
            {
                Metadata = await GetQueryMetadata(searchData, connection),
                Data = result.ToList()
            });
        }

        private static async Task<IEnumerable<dynamic>> GetQueryMetadata(SearchData searchData, SqlConnection connection)
        {
            return await connection.QueryAsync("sp_describe_first_result_set", new {tsql = searchData.Query},
                commandType: CommandType.StoredProcedure);
        }

        public static DataTable Tabulate(string json)
        {
            var jsonLinq = JObject.Parse(json);

            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }

                trgArray.Add(cleanRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }
    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostDataAsync<T>(this HttpClient client, string requestUri, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var dataContent = new StringContent(json, Encoding.UTF8, "application/json");
            return client.PostAsync(requestUri, dataContent);
        }

        public static Task<HttpResponseMessage> PutDataAsync<T>(this HttpClient client, string requestUri, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var dataContent = new StringContent(json, Encoding.UTF8, "application/json");
            return client.PutAsync(requestUri, dataContent);
        }
    }

    public class SearchData
    {
        public string Query { get; set; }
    }

    public class SearchResult
    {
        public dynamic Metadata { get; set; }
        public dynamic Data { get; set; }
    }
}
