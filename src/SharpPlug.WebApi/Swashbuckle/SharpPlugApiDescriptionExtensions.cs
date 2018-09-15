using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace SharpPlug.WebApi.Swashbuckle
{
    public static class SharpPlugApiDescriptionExtensions
    {
        public static IEnumerable<object> ControllerAttributes(this ApiDescription apiDescription)
        {
            var controllerActionDescriptor = apiDescription.ControllerActionDescriptor();
            return (controllerActionDescriptor == null)
                ? Enumerable.Empty<object>()
                : controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(false);
        }

        public static IEnumerable<object> ActionAttributes(this ApiDescription apiDescription)
        {
            var controllerActionDescriptor = apiDescription.ControllerActionDescriptor();
            return (controllerActionDescriptor == null)
                ? Enumerable.Empty<object>()
                : controllerActionDescriptor.MethodInfo.GetCustomAttributes(false);
        }

        internal static string ControllerName(this ApiDescription apiDescription)
        {
            var controllerActionDescriptor = apiDescription.ControllerActionDescriptor();
            return controllerActionDescriptor?.ControllerName;
        }

        public static string FriendlyId(this ApiDescription apiDescription)
        {
            var parts = (apiDescription.RelativePathSansQueryString() + "/" + apiDescription.HttpMethod.ToLower())
                .Split('/');

            var builder = new StringBuilder();
            foreach (var part in parts)
            {
                var trimmed = part.Trim('{', '}');
                builder.AppendFormat("{0}{1}",
                    (part.StartsWith("{") ? "By" : string.Empty),
                    trimmed.ToTitleCase()
                );
            }

            return builder.ToString();
        }

        public static string RelativePathSansQueryString(this ApiDescription apiDescription)
        {
            return apiDescription.RelativePath.Split('?').First();
        }

        public static IEnumerable<string> SupportedRequestMediaTypes(this ApiDescription apiDescription)
        {
            return apiDescription.SupportedRequestFormats
                .Select(requestFormat => requestFormat.MediaType);
        }

        public static IEnumerable<string> SupportedResponseMediaTypes(this ApiDescription apiDescription)
        {
            return apiDescription.SupportedResponseTypes
                .SelectMany(responseType => responseType.ApiResponseFormats)
                .Select(responseFormat => responseFormat.MediaType)
                .Distinct();
        }

        public static bool IsObsolete(this ApiDescription apiDescription)
        {
            return apiDescription.ActionAttributes().OfType<ObsoleteAttribute>().Any();
        }

        public static ControllerActionDescriptor ControllerActionDescriptor(this ApiDescription apiDescription)
        {
            var controllerActionDescriptor = apiDescription.GetProperty<ControllerActionDescriptor>();
            if (controllerActionDescriptor == null)
            {
                controllerActionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;

                if (controllerActionDescriptor != null)
                {
                    apiDescription.SetProperty(controllerActionDescriptor);
                }
            }
            return controllerActionDescriptor;
        }

    }
}
