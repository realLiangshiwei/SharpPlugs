using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using SharpPlug.WebApi.Configuration;
using System.Reflection;

namespace SharpPlug.WebApi.Router
{
    public class SharpPlugRouterActionModelConvention : IActionModelConvention
    {
        private readonly SharpPlugRouterOptions _options;

        public SharpPlugRouterActionModelConvention(SharpPlugRouterOptions options)
        {
            _options = options;
        }

        public void Apply(ActionModel action)
        {
            var method = action.ActionMethod;

            if (method.IsDefined(typeof(HttpMethodAttribute)))
                return;
            action.Selectors.Clear();
            //custom prefix
            HttpMethodAttribute attr;
            foreach (var custom in _options.CustomRule)
            {
                if (action.ActionName.StartsWith(custom.Key))
                {
                    switch (custom.Value)
                    {
                        case HttpVerbs.HttpGet:
                            attr = new HttpGetAttribute(action.ActionName);
                            break;
                        case HttpVerbs.HttpPost:
                            attr = new HttpPostAttribute(action.ActionName);
                            break;
                        case HttpVerbs.HttpDelete:
                            attr = new HttpDeleteAttribute(action.ActionName);
                            break;
                        case HttpVerbs.HttpPut:
                            attr = new HttpPutAttribute(action.ActionName);
                            break;
                        default:
                            attr = new HttpPostAttribute(action.ActionName);
                            break;
                    }
                    ModelConventionHelper.AddRange(action.Selectors, ModelConventionHelper.CreateSelectors(new List<object> { attr }));
                    return;
                }
            }

            if (method.Name.StartsWith("Get"))
                attr = new HttpGetAttribute(method.Name);
            else if (method.Name.StartsWith("Post") || method.Name.StartsWith("Add"))
                attr = new HttpPostAttribute(method.Name);
            else if (method.Name.StartsWith("Update") || method.Name.StartsWith("Put"))
                attr = new HttpPutAttribute(method.Name);
            else if (method.Name.StartsWith("Del") || method.Name.StartsWith("Delete"))
                attr = new HttpDeleteAttribute(method.Name);
            else
                attr = new HttpPostAttribute(method.Name);
            ModelConventionHelper.AddRange(action.Selectors, ModelConventionHelper.CreateSelectors(new List<object> { attr }));

        }
    }
}
