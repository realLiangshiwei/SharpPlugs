using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpPlug.Core.DI;

namespace SharpPlug.WebApiTest
{
    public interface ITestDi
    {
        string Test();
    }

    public class TestDi : ITestDi, IScopedDependency
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
