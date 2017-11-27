using SharpPlug.AutoMapper.Attribute;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPlug.Test.sharpPlug.AutoMapper
{
    public class Person
    {
        public string Name { get; set; }
    }

    [AutoMap(typeof(Person))]
    public class PersonDto
    {
        public string Name { get; set; }
    }
}
