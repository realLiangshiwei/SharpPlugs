using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SharpPlug.WebApi.Configuration
{
    public class SharpPlugRouterOptions
    {
        public SharpPlugRouterOptions()
        {
            CustomRule = new Dictionary<string, HttpVerbs>();
        }

        public Dictionary<string, HttpVerbs> CustomRule { get; set; }
    }

    public enum HttpVerbs
    {
        HttpGet,
        HttpPost,
        HttpDelete,
        HttpPut
    }
}
