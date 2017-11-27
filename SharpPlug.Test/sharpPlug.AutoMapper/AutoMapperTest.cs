using SharpPlug.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
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
