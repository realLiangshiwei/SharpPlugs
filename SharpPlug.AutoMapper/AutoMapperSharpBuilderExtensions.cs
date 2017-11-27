using System;
using System.Linq;
using System.Reflection;
using AspectCore.Extensions.Reflection;
using AutoMapper;
using SharpPlug.AutoMapper.Attribute;
using SharpPlug.Core;

namespace SharpPlug.AutoMapper
{
    public static class AutoMapperSharpBuilderExtensions
    {
        public static ISharpPlugBuilder AddAutoMapperPlug(this ISharpPlugBuilder builder, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length <= 0)
                throw new ArgumentNullException("assemblies args is null or length with 0");
            Mapper.Initialize(config =>
            {
                foreach (var assembly in assemblies)
                {
        
                    var types = from type in assembly.GetTypes()
                                let refType = type.GetReflector()
                                where refType.IsDefined(typeof(AutoMapAttribute)) || refType.IsDefined(typeof(MapToAttribute)) ||
                                      refType.IsDefined(typeof(MapFromAttribute)) 
                                select type;
                    
                    foreach (var type in types)
                    {
                        foreach (var autoMapAttribute in type.GetReflector().GetCustomAttributes<MapAttributeBase>())
                        {
                            autoMapAttribute.CreateMap(config, type);
                        }
                    }
                   
                   var mappingTypes = assembly.GetTypes().Where(o => o.GetInterfaces().Contains(typeof(IMapping))).ToArray();
                    foreach (var mapingType in mappingTypes)
                    {
                        ((IMapping)Activator.CreateInstance(mapingType, true)).CreateMapping(config);
                    }

                }
            });

            return builder;
        }
    }
}
