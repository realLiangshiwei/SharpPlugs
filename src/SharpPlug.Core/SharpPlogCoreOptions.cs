using System.Collections.Generic;
using System.Reflection;

namespace SharpPlug.Core
{

    public class SharpPlogCoreOptions
    {
        public SharpPlogCoreOptions()
        {
            DiAssembly = new List<Assembly>();
            ClassSuffix = new List<string>
            {
                "Service",
                "Repository"
            };
        }

        public IList<Assembly> DiAssembly { get; }

        public IList<string> ClassSuffix { get; }

    }
}
