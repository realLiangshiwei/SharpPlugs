using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharpPlug.Core.DI
{
    /// <summary>
    /// 依賴注入管理
    /// </summary>
    public static class DiSharpBuilderExtensions
    {
        /// <summary>
        /// 传入程序集,注册依賴关系
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="classSuffix"></param>
        /// <param name="assembly"></param>
        public static ISharpPlugBuilder Register(this ISharpPlugBuilder builder, string[] classSuffix, params Assembly[] assembly)
        {
            assembly.AsParallel().ForAll(ass => DefaultRegister(builder.Services, classSuffix, ass));
            return builder;
        }

        private static void DefaultRegister(IServiceCollection sercice, string[] classSuffix, Assembly assembly)
        {

            var allType = assembly.GetTypes();
            var types = allType.Where(o =>
            (typeof(ITrasientDependency).IsAssignableFrom(o) || typeof(IScopedDependency).IsAssignableFrom(o) || typeof(ISingletonDependency).IsAssignableFrom(o))
            && classSuffix.Any(x => o.Name.EndsWith(x)
            && !o.IsInterface)
            ).ToList();

            foreach (var type in allType.Where(o =>
            (typeof(ITrasientDependency).IsAssignableFrom(o) || typeof(IScopedDependency).IsAssignableFrom(o) || typeof(ISingletonDependency).IsAssignableFrom(o))
            && classSuffix.All(x => !o.Name.EndsWith(x)
            && !o.IsInterface)).ToList())
            {
                if (typeof(ITrasientDependency).IsAssignableFrom(type))
                {
                    sercice.AddTransient(type);
                }
                else if (typeof(IScopedDependency).IsAssignableFrom(type))
                {

                    sercice.AddScoped(type);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                {
                    sercice.AddSingleton(type);
                }
            }

            foreach (var type in types)
            {
                var @interface = allType.FirstOrDefault(o => o.Name == "I" + type.Name && o.IsInterface);
                bool interfaceExist = @interface != null;

                if (typeof(ITrasientDependency).IsAssignableFrom(type))
                {
                    if (interfaceExist)
                        sercice.AddTransient(@interface, type);
                    else
                        sercice.AddTransient(type);
                }
                else if (typeof(IScopedDependency).IsAssignableFrom(type))
                {
                    if (interfaceExist)
                        sercice.AddScoped(@interface, type);
                    else
                        sercice.AddScoped(type);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                {
                    if (interfaceExist)
                        sercice.AddSingleton(@interface, type);
                    else
                        sercice.AddSingleton(type);
                }


            }
        }
    }
}
