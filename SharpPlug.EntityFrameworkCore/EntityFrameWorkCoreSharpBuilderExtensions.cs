
using Microsoft.Extensions.DependencyInjection;
using SharpPlug.Core;
using SharpPlug.EntityFrameworkCore.RepositoriesBase;

namespace SharpPlug.EntityFrameworkCore
{
    public static class EntityFrameWorkCoreSharpBuilderExtensions
    {
        /// <summary>
        /// Auto Add RouterAttribute And  HttpMethodAttribute
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ISharpPlugBuilder AddEntityFramework(this ISharpPlugBuilder builder)
        {
            builder.Services.AddTransient(typeof(Repository<,>));
            return builder;
        }
    }
}
