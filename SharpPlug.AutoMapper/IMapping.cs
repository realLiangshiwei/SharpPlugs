using AutoMapper;

namespace SharpPlug.AutoMapper
{
    /// <summary>
    /// 用于创建复杂映射配置
    /// </summary>
    public interface IMapping
    {
        void CreateMapping(IMapperConfigurationExpression mapperConfig);
    }
}
