using System;
using AutoMapper;

namespace SharpPlug.AutoMapper.Attribute
{
    /// <summary>
    /// 从类中映射
    /// </summary>
    public class MapFromAttribute : MapAttributeBase
    {

        public MapFromAttribute(params Type[] targeTypes) : base(targeTypes)
        {
            
        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes == null || TargetTypes.Length <= 0)
                return;

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateMap(targetType, type, MemberList.Destination);
            }
        }
    }
}
