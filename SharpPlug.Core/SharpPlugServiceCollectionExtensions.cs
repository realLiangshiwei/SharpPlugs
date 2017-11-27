using Microsoft.Extensions.DependencyInjection;
using SharpPlug.Core.DI;

namespace SharpPlug.Core
{
    /// <summary>
    ///  Extension methods for setting up SharpPlug services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class SharpPlugServiceCollectionExtensions
    {
        public static ISharpPlugBuilder AddSharpPlugCore(this IServiceCollection services)
        {
            var builder = new DefaultSharpPlugBuilder(services);
            builder.Register();

            return builder;
        }
    }
}
