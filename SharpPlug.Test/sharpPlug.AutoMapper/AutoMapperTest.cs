using SharpPlug.Core;
using System.Reflection;
using Xunit;
using SharpPlug.AutoMapper;

namespace SharpPlug.Test.sharpPlug.AutoMapper
{

    public class AutoMapperTest
    {
        [Fact]
        public void AutoMapper_Init_Success()
        {
            ISharpPlugBuilder builder = new DefaultSharpPlugBuilder(null);

            builder.AddAutoMapperPlug(Assembly.GetExecutingAssembly());


            var per = new Person()
            {
                Name = "小明"
            };

            var dto = per.MapTo<PersonDto>();

        
            Assert.Equal<string>(dto.Name,per.Name);
        }
    }
}
