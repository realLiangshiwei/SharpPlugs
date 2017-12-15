using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpPlug.Core.DI;
using SharpPlug.EntityFrameworkCore.Entity;
using SharpPlug.EntityFrameworkCore.Repositories;


namespace SharpPlug.WebApiTest
{
    public interface ITestDiService : IScopedDependency
    {
        string Test();
    }

    public class TestDiService : ITestDiService
    {
        public string Test()
        {
            return "HelloWorld";
        }
    }



    public class TestDi2 : ITrasientDependency
    {
        public IEnumerable<string> Test()
        {
            return new[] { "value1", "value2" };
        }
    }
}
