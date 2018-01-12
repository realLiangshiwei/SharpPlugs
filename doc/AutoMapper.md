# SharpPlugs AutoMapper

When you read this article you already know the basic concepts and usage of AutoMapper by default, this article without explanation . If you have not understand, it is suggested that to AutoMapper's official website to check the related documents

AutoMapper provides us with convenient mapping, we can like this configuration, and then transform

```c#
Mapper.Initialize(x=>x.CreateMap<TSource,TDestination>);
Mapper.Map<TDestination>(TSource);
```
If such a lot of configuration will be more and more, the configuration file is more and more bloated
SharpPlug. AutoMapper plugin provides a simple way of configuration

For a simple mapping provides the following Attribute
- AutoMapAttribute -- -- -- -- -- the bidirectional mapping
- MapFrom -- -- -- -- -- from some kind of mapping
- MapTo -- -- -- -- -- is mapped to a certain

For complex mapping provides IMapping interface

Through an example to illustrate . Using VsCode Create a asp.net core  MVC project
```c#
dotnet new mvc -n AutoMapperDemo
```
Install SharpPlug. AutoMapper nuget packages
```c#
dotnet add package SharpPlug.Automapper
```