using Microsoft.Extensions.DependencyInjection;

namespace SharpPlug.Core
{
    public interface ISharpPlugBuilder
    {
        IServiceCollection Services { get; }
    }


    public class DefaultSharpPlugBuilder : ISharpPlugBuilder
    {
        public DefaultSharpPlugBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
