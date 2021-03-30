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
    public class Test2Controller : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        private readonly string _connectionString =
            "Password=Sqlp@$$W0rd;Persist Security Info=True;User ID=sa;Initial Catalog=AL_SECURITY;Data Source=.";

        public Test2Controller(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpPost("test-search")]
        public async Task<IActionResult> TestSearch([FromBody] SearchData searchData)
        {
           
            return Ok();
        }
    }

    public class TestSearchData : SearchData
    {
        public int Add { get; set; }
    }
}