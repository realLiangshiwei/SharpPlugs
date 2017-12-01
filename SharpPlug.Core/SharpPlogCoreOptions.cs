using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharpPlug.Core
{

    public class SharpPlogCoreOptions
    {
        public SharpPlogCoreOptions()
        {
            DiAssembly = new List<Assembly>();
        }

        public IList<Assembly> DiAssembly { get; }


    }
}
