# SharpPlugs DI

Dependency injection in the back-end is already a mainstream way of development, Asp.net core built-in IOC container, we can add rely on like this
```c#
services.AddTransient<ITestService,TestService>();
```
But this shows add rely on, has the following disadvantages
- Easily forgotten
- Dependence when rely on too much, add code will be very large
- Repeat boring work

SharpPlugs can help developers to maintain dependencies,SharpPlugs use naming conventions, these at application startup suitable classes will be added.

The default naming *Naming conventions can be custom
- Service
- Repository

SharpPlugs provides the following interface
- IScopedDependency      The only instance scope (the same request)
- ISingletonDependency   The singleton
- ITrasientDependency    Every time a new instance

The following will through a simple example to illustrate Create a asp.net core MVC project
Now, install SharpPlugs Core packages
```powershell
dotnet add package SharpPlug.Core
```
AddSharpPlugCore during Startup 

ClassSuffix is a collection, used to add custom naming convention
```c#
 services.AddSharpPlugCore(opt=>{
      opt.DiAssembly.Add(Assembly.GetExecutingAssembly());
      opt.ClassSuffix.Add("MySuffix");
 });
```
Create a class with ScopedService IScopedService interface, Life cycle is IScopedDependency
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
Create a class with SingletonMySuffix ISingletonMySuffix interface, Life cycle is ISingletonDependency
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
Create Class TrasientService It does not implement other interfaces,Life cycle is ITrasientDependency
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

Respectively added DI above three kinds of usage, the first is based on the default naming convention, the second custom naming convention, the third did not implement other interfaces

Now we are injected into the HomeController
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
We can see scopeService and scopeService are the same instance,SingleonMySuffix use custom suffix, it is a singleton,TrasientService demonstrated directly into classes instead of interfaces, Instance of it is different every time