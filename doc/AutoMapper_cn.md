# SharpPlugs AutoMapper

在您阅读这篇文章的时候默认你已经懂得AutoMapper的基本概念与用法，本文不做解释。如果您还没有了解,建议去AutoMapper官方网站查看相关文档

AutoMapper为我们提供了方便的映射，我们可以像下面这样配置，然后进行转换
```c#
Mapper.Initialize(x=>x.CreateMap<TSource,TDestination>);
Mapper.Map<TDestination>(TSource);
```
如果类很多的话配置也会越来越多，配置文件越来越臃肿
SharpPlug.AutoMapper插件提供了简单的配置方式 脱离繁琐的配置

对于简单映射提供了以下Attribute

- AutoMapAttribute ----- 双向映射
- MapFrom   -----   从某类映射
- MapTo  -----   映射到某类

对于复杂映射提供了IMapping接口

通过一个例子来说明,现在创建一个 Asp.Net Core Mvc项目
```c#
dotnet new mvc -n AutoMapperDemo
```
安装 SharpPlug.AutoMapper nuget包
```c#
dotnet add package SharpPlug.Automapper
```

