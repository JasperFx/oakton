using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using Oakton.Help;
using Shouldly;
using Xunit;

[assembly: OaktonCommandAssembly]

namespace Tests;

public class using_injected_services
{
    [Fact]
    public async Task can_use_injected_services()
    {
        var success = await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<MyService>();
                services.AddScoped<OtherService>();
            })
            .RunOaktonCommands(new string[] { "injected", "Bob Marley" });
        
        success.ShouldBe(0);
        
        MyService.WasCalled.ShouldBeTrue();
        MyService.Name.ShouldBe("Bob Marley");
        MyService.WasDisposed.ShouldBeTrue();
        
        OtherService.WasCalled.ShouldBeTrue();
        OtherService.Name.ShouldBe("Bob Marley");
        OtherService.WasDisposed.ShouldBeTrue();

    }
}

public class InjectedInput
{
    public string Name { get; set; }
}

[Description("Injected command", Name = "injected")]
public class InjectedCommand : OaktonCommand<InjectedInput>
{
    [InjectService]
    public MyService One { get; set; }
    
    [InjectService]
    public OtherService Two { get; set; }
    
    public override bool Execute(InjectedInput input)
    {
        One.DoStuff(input.Name);
        Two.DoStuff(input.Name);

        return true;
    }
}

public class MyService : IDisposable
{
    public static bool WasCalled;
    public static string Name;

    public static bool WasDisposed;

    public void DoStuff(string name)
    {
        WasCalled = true;
        Name = name;
    }

    public void Dispose()
    {
        WasDisposed = true;
    }
}

public class OtherService : IDisposable
{
    public static bool WasCalled;
    public static string Name;
    
    public static bool WasDisposed;
    
    public void DoStuff(string name)
    {
        WasCalled = true;
        Name = name;
    }
    
    public void Dispose()
    {
        WasDisposed = true;
    }
}

public class MyDbContext{}

public class MyInput
{
    
}

#region sample_MyDbCommand

public class MyDbCommand : OaktonAsyncCommand<MyInput>
{
    [InjectService]
    public MyDbContext DbContext { get; set; }
    
    public override Task<bool> Execute(MyInput input)
    {
        // do stuff with DbContext from up above
        return Task.FromResult(true);
    }
}

#endregion