# SharpPlugs DI

依赖注入在后端已经是主流的开发方式，asp.net core内置了IOC容器,我们只需像下面这样添加依赖
```c#
services.AddTransient<ITestService,TestService>();
```
但是这样显示添加依赖,有以下缺点
- 容易被遗忘
- 当依赖过多时,添加的依赖代码会变的很庞大
- 重复枯燥的工作

SharpPlugs 可以帮助开发人员维护依赖关系,SharpPlugs 采用命名约定,这些在应用程序启动时合适的类名将被添加依赖

以下的默认的命名约定
- Service
- Repository
命名约定是完全可配置的

SharpPlugs提供了以下接口
- IScopedDependency      作用域内唯一实例(同一个请求)
- ISingletonDependency   单例 
- ITrasientDependency    每次都是不同的实例

以下将通过一个简单的例子来说明, 使用vscode创建一个asp.net core mvc项目
创建完成后安装sharpplg.core nuget包
```powershell
dotnet add package SharpPlug.Core
```
在StartUp中添加AddSharpPlugCore

ClassSuffix是一个集合, 在这里添加自定义的命名约定后缀
```c#
 services.AddSharpPlugCore(opt=>{
      opt.DiAssembly.Add(Assembly.GetExecutingAssembly());
      opt.ClassSuffix.Add("MySuffix");
 });
```
创建类ScopedService与接口IScopedService，继承自IScopedDependency接口
```c#
public class ScopedService : IScopedService,IScopedDependency
{
    public Guid Str {get; set;}

    public ScopedService()
    {
        Str = Guid.NewGuid();
    }
    public string Hello()
    {
        return Str.ToString();
    }
}

public  interface IScopedService
{
    string Hello();
}
```
创建类SingletonMySuffix与接口ISingletonMySuffix，继承自ISingletonDependency接口
```c#
public class SingletonMySuffix : ISingletonMySuffix,ISingletonDependency
{
    public Guid Str {get; set;}

    public SingletonMySuffix()
    {
        Str = Guid.NewGuid();
    }
    public string Hello()
    {
        return Str.ToString();
    }
}

public interface ISingletonMySuffix
{
    string Hello();
}
```
创建类TrasientService 它并没有继承其他接口,为它继承ITrasientDependency接口
```c#
public class SingletonMySuffix : ITrasientDependency
{
    public Guid Str {get; set;}

    public SingletonMySuffix()
    {
        Str = Guid.NewGuid();
    }
    public string Hello()
    {
        return Str.ToString();
    }
}

```

上面的代码分别演示了DI以上三种用法,首先是基于默认的命名约定,第二个定义命名约定,第三个没有实现其他接口

现在注入到HomeController中
```c#
private readonly IScopedService _scopedService;
private readonly IScopedService _scopedService2;

private readonly ISingletonMySuffix _singletonMySufix;

private readonly TrasientService _trasientService;
private readonly TrasientService _trasientService2;

public  HomeController(IScopedService scopedService,IScopedService scopedService2,ISingletonMySuffix singletonMySuffix,TrasientService trasientService,TrasientService trasientService2)
{
    _scopedService = scopedService;
    _scopedService2 = scopedService2;
    _singletonMySufix = singletonMySuffix;
    _trasientService= trasientService;
    _trasientService2 = trasientService2;
}

public IActionResult Hello()
{
    ViewBag.scopeService = _scopedService.Hello();
    ViewBag.scopeService2 = _scopedService2.Hello();
    ViewBag.singleonMySuffix = _singletonMySufix.Hello();
    ViewBag.trasientService = _trasientService.Hello();
    ViewBag.trasientService2 = _trasientService2.Hello();
    return View();
}

```

Now run the project

![asp.net core Project](/doc/img/DI/1.png)
我们可以看到scopeService scopeService相同的实例,SingleonMySuffix使用的是自定义的后缀它的实例是单例的,TrasientService演示了直接注入类而它每次都是不同的实例