using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SharpPlug.Core.DI;

namespace SharpPlug.Core
{
    /// <summary>
    ///  Extension methods for setting up SharpPlug services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class SharpPlugServiceCollectionExtensions
    {
        public static ISharpPlugBuilder AddSharpPlugCore(this IServiceCollection services, Action<SharpPlogCoreOptions> setupAction = null)
        {
            var builder = new DefaultSharpPlugBuilder(services);
            var options = new SharpPlogCoreOptions();
            setupAction?.Invoke(options);
            builder.Register(options.ClassSuffix.ToArray(), options.DiAssembly.ToArray());
            return builder;
        }
    }
}
