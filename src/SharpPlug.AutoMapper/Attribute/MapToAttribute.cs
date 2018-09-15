using System;
using AutoMapper;

namespace SharpPlug.AutoMapper.Attribute
{
    /// <summary>
    /// 映射到类
    /// </summary>
    public class MapToAttribute : MapAttributeBase
    {
        public MapToAttribute(params Type[] targeTypes) : base(targeTypes)
        {

        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes == null || TargetTypes.Length <= 0)
                return;

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateMap(targetType, type, MemberList.Source);
            }
        }
    }
}
