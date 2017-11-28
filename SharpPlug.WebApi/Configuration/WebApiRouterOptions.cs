using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SharpPlug.WebApi.Configuration
{
    public class WebApiRouterOptions
    {
        public Dictionary<string, HttpMethodAttribute> CustomRule { get; set; }
    }
}
