using System;
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
        /// <param name="assembly"></param>
        public static ISharpPlugBuilder Register(this ISharpPlugBuilder builder, params Assembly[] assembly)
        {
            assembly.AsParallel().ForAll(ass => DefaultRegister(builder.Services, ass));
            return builder;
        }

        private static void DefaultRegister(IServiceCollection sercice, Assembly assembly)
        {
            void CheckType(Type t)
            {
                if (t.IsInterface)
                    throw new Exception("Please use class implements interface, can not use interface");
            }

            bool CheckInterface(Type t, string dependency)
            {
                if (t.GetInterfaces().Any(o => o.Name != dependency))
                {
                    return true;
                }
                return false;
            }
            foreach (var type in assembly.GetTypes())
            {

                if (typeof(ITrasientDependency).IsAssignableFrom(type))
                {
                    CheckType(type);
                    if (CheckInterface(type, nameof(ITrasientDependency)))
                        sercice.AddTransient(type.GetInterfaces().First(), type);
                    else
                        sercice.AddTransient(type);

                }
                else if (typeof(IScopedDependency).IsAssignableFrom(type))
                {
                    CheckType(type);
                    if (CheckInterface(type, nameof(IScopedDependency)))
                        sercice.AddScoped(type.GetInterfaces().First(), type);
                    else
                        sercice.AddScoped(type);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                {
                    CheckType(type);
                    if (CheckInterface(type, nameof(ISingletonDependency)))
                        sercice.AddSingleton(type.GetInterfaces().First(), type);
                    else
                        sercice.AddSingleton(type);
                }
            }
        }
    }
}
