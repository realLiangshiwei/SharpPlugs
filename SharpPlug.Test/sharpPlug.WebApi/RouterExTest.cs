using SharpPlug.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using SharpPlug.WebApi.Router;
namespace SharpPlug.Test.sharpPlug.WebApi
{
    public class RouterExTest
    {
        private void Builder()
        {
            ISharpPlugBuilder builder = new DefaultSharpPlugBuilder(null);

            builder.AddWebApiRouter(Assembly.GetExecutingAssembly(), options =>
            {
                options.CustomRule.Add("Custem", new HttpDeleteAttribute());
            });
        }

        [Fact]
        public void WebApiRouter_Init_Success()
        {
            Builder();

            var testType = typeof(TestController);
            var test1Type = typeof(Test1Controller);
            Assert.True(testType.IsDefined(typeof(RouteAttribute)));
            Assert.True(test1Type.GetCustomAttributes<RouteAttribute>().Count() == 1);

            var methodType = testType.GetMethods();

            Assert.True(methodType.First(o => o.Name == "Get").GetCustomAttribute<HttpGetAttribute>().Template ==
                        "Get");
            Assert.True(methodType.Where(o => o.Name == "Post" || o.Name == "Add").ToList()
                .All(o => o.IsDefined(typeof(HttpPostAttribute))));

            Assert.True(methodType.First(o => o.Name == "CustemMethod").IsDefined(typeof(HttpDeleteAttribute)));


        }
    }

    public class TestController
    {
        public void Get()
        {

        }

        public void Post()
        {

        }

        public void Add()
        {

        }

        public void CustemMethod()
        {

        }

        [HttpPost]
        public void Get1()
        {

        }
    }

    [Route("api/Test1")]
    public class Test1Controller
    {

    }
}
