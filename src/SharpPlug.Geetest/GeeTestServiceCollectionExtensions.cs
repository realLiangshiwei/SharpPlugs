using System;
using Microsoft.Extensions.DependencyInjection;

namespace SharpPlug.Geetest
{
    public static class GeeTestServiceCollectionExtensions
    {
        public static IServiceCollection AddGeetest(this IServiceCollection services, Action<GeetestOptions> setupAction)
        {
            services.AddHttpClient();
            services.Configure(setupAction);
            services.AddSingleton<GeetestManager>();
           
            return services;
        }
    }
}
