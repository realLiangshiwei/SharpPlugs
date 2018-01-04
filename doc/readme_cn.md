
<img src="https://raw.githubusercontent.com/ShiWei-L/SharpPlugs/master/SharpPlug.Core/logo.png" width="200" height="200" /> 

# SharpPlugs 

.Net Core 鋒利扩展


[![Build status](https://ci.appveyor.com/api/projects/status/74whrxjajlnacjma?svg=true)](https://ci.appveyor.com/project/ShiWei-L/sharpplugs)
[![NuGet](https://img.shields.io/nuget/v/SharpPlug.Core.svg)](https://www.nuget.org/packages/SharpPlug.Core/)
[![NuGet](https://img.shields.io/nuget/dt/SharpPlug.Core.svg)](https://www.nuget.org/packages/SharpPlug.Core/)

## 当前功能

- DI
- AutoMapper
- ElasticSearch
- WebAPiRoute
- EntityFramework Repoistory

## 快速开始

首先我们需要一个Asp.net Core的项目，在这里我提前创建了一个Asp.net Core MVC项目

![asp.net core Project](/doc/img/getStarted/createProject.png)

现在安装 SharpPlug.core Nuget包
```powershell
dotnet add package SharpPlug.Core
```
在Startup添加AddSharpPlugCore
```c#
 services.AddSharpPlugCore(opt=>{
      opt.DiAssembly.Add(Assembly.GetExecutingAssembly());
 });
```
![asp.net core Project](/doc/img/getStarted/2.png)
现在我们已经有了自动依赖注入的功能,我创建了TestService类与ITestService接口

*自动依赖注入是有命名约定的, 以Service或Repository结尾的将被自动注入*

```c#
public class TestService : ITestService,IScopedDependency
{

    public string Hello()
    {
         return "Hello World";
    }
}
```
在HomeController注入ITestService
```c#
public class HomeController : Controller
{
    private readonly ITestService _testService;
    public  HomeController(ITestService testService)
    {
        _testService = testService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Hello()
    {
        return Json(_testService.Hello())   ;
    }
       
}
```
按F5进行调试，在浏览器地址栏输入/Home/Index，请求会停留在断点的位置

![asp.net core Project](/doc/img/getStarted/3.png)

按F5继续运行, 会看到浏览器输出 Hello World

![asp.net core Project](/doc/img/getStarted/4.png)

想要了解更多的使用方法? 请查看[高级文档](/doc/document_cn.md)部分