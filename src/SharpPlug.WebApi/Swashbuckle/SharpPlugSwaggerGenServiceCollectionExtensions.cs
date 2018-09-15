using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpPlug.Core;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharpPlug.WebApi.Swashbuckle
{
    /// <summary>
    /// SharpPlug Custom SwaggerGenServiceCollction
    /// SwaggerGenServiceCollction Source Code https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerGen/Application/SwaggerGenServiceCollectionExtensions.cs
    /// </summary>
    public static class SharpPlugSwaggerGenServiceCollectionExtensions
    {
        public static ISharpPlugBuilder AddSharpPlugSwaggerGen(this ISharpPlugBuilder builder, Action<SharpPlugSwaggerGenOptions> setupAction)
        {
            builder.Services.Configure<MvcOptions>(c =>
                c.Conventions.Add(new SwaggerApplicationConvention()));

            builder.Services.Configure(setupAction ?? (opts => { }));

            builder.Services.AddTransient(CreateSwaggerProvider);
           
            return builder;
        }

        public static ISwaggerProvider CreateSwaggerProvider(IServiceProvider serviceProvider)
        {
            var swaggerGenOptions = serviceProvider.GetRequiredService<IOptions<SharpPlugSwaggerGenOptions>>().Value;
            return swaggerGenOptions.CreateSwaggerProvider(serviceProvider);
        }
    }
}
