using System;
using SharpPlug.Core;
using System.Reflection;
using AutoMapper;
using Xunit;
using SharpPlug.AutoMapper;
using SharpPlug.AutoMapper.Attribute;

namespace SharpPlug.Test.sharpPlug.AutoMapper
{

    public class AutoMapperTest
    {
        public void Builder()
        {
            ISharpPlugBuilder builder = new DefaultSharpPlugBuilder(null);

            builder.AddAutoMapperPlug(Assembly.GetExecutingAssembly());
        }

        [Fact]
        public void AutoMapper_Init_Success()
        {
            Builder();
        }

        public void Map_Success()
        {
            var per = new Person()
            {
                Name = "小明"
            };
            var dto = per.MapTo<PersonDto>();
            Assert.Equal<string>(dto.Name, per.Name);

            var mapfrom = new MapFrom { Str = "test" };
            Assert.Equal<string>(mapfrom.MapTo<MapFromDto>().Str, mapfrom.Str);

            var custom = new Custom()
            {
                Str = "test"
            };
            Assert.Equal<string>(custom.MapTo<CustomDto>().Str1, mapfrom.Str);
        }
    }

    public class Custom
    {
        public string Str { get; set; }
    }

    public class CustomDto
    {
        public string Str1 { get; set; }
    }

    public class CustomMapper : IMapping
    {
        public void CreateMapping(IMapperConfigurationExpression mapperConfig)
        {
            mapperConfig.CreateMap<Custom, CustomDto>().ForMember(s => s.Str1, c => c.MapFrom(q => q.Str));
        }
    }

    public class MapFrom
    {
        public string Str { get; set; }
    }

    [MapFrom(typeof(MapFrom))]
    public class MapFromDto
    {
        public string Str { get; set; }
    }
}
