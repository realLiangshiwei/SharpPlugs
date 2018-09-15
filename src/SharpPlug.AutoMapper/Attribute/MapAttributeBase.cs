using System;
using AutoMapper;

namespace SharpPlug.AutoMapper.Attribute
{
    public abstract class MapAttributeBase : System.Attribute
    {
        public Type[] TargetTypes { get; }

        protected MapAttributeBase(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        public abstract void CreateMap(IMapperConfigurationExpression configuration, Type type);
    }
}
