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
                if (!t.IsInterface)
                    throw new Exception("DI接口请用类实现,不可以使用接口");
            }

            foreach (var type in assembly.GetTypes())
            {
                var intrefaces = type.GetInterfaces();
                if (intrefaces.Contains(typeof(ITrasientDependency)))
                {
                    CheckType(type);
                    sercice.AddTransient(type, intrefaces.First());
                }
                if (intrefaces.Contains(typeof(IScopedDependency)))
                {
                    CheckType(type);
                    sercice.AddScoped(type, intrefaces.First());
                }
                if (intrefaces.Contains(typeof(ISingletonDependency)))
                {
                    CheckType(type);
                    sercice.AddSingleton(type, intrefaces.First());
                }
            }
        }
    }
}
