using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SharpPlug.Core;
using SharpPlug.WebApi.Configuration;

namespace SharpPlug.WebApi.Router
{
    public static class WebApiSharpBuilderExtensions
    {
        /// <summary>
        /// Auto Add RouterAttribute And  HtptMethodAttrute
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        /// <param name="startAction">custom option</param>
        /// <returns></returns>
        public static ISharpPlugBuilder AddWebApiRouter(this ISharpPlugBuilder builder,
           Assembly assembly, Action<WebApiRouterOptions> startAction = null)
        {
            var options = new WebApiRouterOptions()
            {
                CustomRule = new Dictionary<string, HttpMethodAttribute>()
            };
            startAction?.Invoke(options);
            var types = assembly.GetTypes().Where(o => o.Name.EndsWith("Controller")).ToList();
            foreach (var controller in types)
            {
                if (controller.IsDefined(typeof(RouteAttribute)))
                    continue;
                TypeDescriptor.AddAttributes(controller,
                    new RouteAttribute($"api/{controller.Name.Replace("Controller", "")}"));
                foreach (var methodInfo in controller.GetMethods())
                {
                    if (methodInfo.IsDefined(typeof(RouteAttribute)))
                        continue;

                    foreach (var custemRule in options.CustomRule)
                    {
                        if (methodInfo.Name.StartsWith(custemRule.Key))
                        {
                            TypeDescriptor.AddAttributes(methodInfo, custemRule.Value);
                            continue;
                        }
                        switch (methodInfo.Name)
                        {
                            case "Get":
                                TypeDescriptor.AddAttributes(methodInfo, new HttpGetAttribute(methodInfo.Name));
                                break;
                            case "Add":
                            case "Post":
                                TypeDescriptor.AddAttributes(methodInfo, new HttpPostAttribute(methodInfo.Name));
                                break;
                            default:
                                TypeDescriptor.AddAttributes(methodInfo, new HttpPostAttribute(methodInfo.Name));
                                break;
                        }

                    }
                }

            }
            return builder;
        }
    }
}
