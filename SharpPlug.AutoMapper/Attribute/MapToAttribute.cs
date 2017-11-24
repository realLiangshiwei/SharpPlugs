using System;

namespace SharpPlug.AutoMapper.Attribute
{
    /// <summary>
    /// 映射到类
    /// </summary>
    public class MapToAttribute : System.Attribute
    {
        public Type[] TargetTypes { get; set; }

        public MapToAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}
