using AutoMapper;

namespace SharpPlug.AutoMapper
{
    /// <summary>
    /// 用于创建复杂映射配置
    /// </summary>
    public interface IMaping
    {
        void CreateMapping(IMapperConfigurationExpression mapperConfig);
    }
}
