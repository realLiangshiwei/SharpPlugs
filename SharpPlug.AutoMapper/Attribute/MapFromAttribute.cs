using System;

namespace SharpPlug.AutoMapper.Attribute
{
    /// <summary>
    /// 从类中映射
    /// </summary>
    public class MapFromAttribute : System.Attribute
    {
        public Type[] TargetTypes;

        public MapFromAttribute(params Type[] targeTypes)
        {
            TargetTypes = targeTypes;
        }
    }
}
