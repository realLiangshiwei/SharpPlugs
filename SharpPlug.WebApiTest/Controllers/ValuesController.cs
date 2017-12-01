using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SharpPlug.WebApiTest.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/Values")]
    public class ValuesController : Controller
    {

        private readonly ITestDi _testDi;

        private readonly TestDi2 _testDi2;

        public ValuesController(ITestDi testDi, TestDi2 testDi2)
        {
            _testDi = testDi;
            _testDi2 = testDi2;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTest")]
        public IEnumerable<string> GetVal()
        {
            return _testDi2.Test();
        }

        /// <summary>
        /// xx
        /// </summary>
        /// <returns></returns>

        public string PostTest(int? id)
        {
            return _testDi.Test();
        }
    }
}
