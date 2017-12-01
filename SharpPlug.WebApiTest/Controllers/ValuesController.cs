using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharpPlug.WebApi.Configuration;

namespace SharpPlug.WebApiTest.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/Values")]
    public class ValuesController : Controller
    {

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTest")]
        public IEnumerable<string> GetVal()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// xx
        /// </summary>
        /// <returns></returns>

        public string PostTest(int? id)
        {
            return "hello world";
        }
    }
}
