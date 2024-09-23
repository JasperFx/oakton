using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using System;
using Xunit;

namespace Tests;

public class HostedCommandsTester
{
    [Fact]
    public void CanInjectServicesIntoCommands()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<TestDependency>();
                services.AddOakton(options =>
                {
                    options.Factory = factory =>
                    {
                        factory.RegisterCommand<TestDICommand>();
                    };
                    options.DefaultCommand = "TestDI";
                });
            });

        var app = builder.Build();

        app.RunHostedOaktonCommands(Array.Empty<string>());

        Assert.Equal(1, TestDICommand.Value);
    }

    public class TestInput
    {
    }

    public class TestDependency : IDisposable
    {
        public int Value { get; private set; }

        public TestDependency()
        {
            Value = 1;
        }

        public void Dispose()
        {
            Value = 0;
            GC.SuppressFinalize(this);
        }
    }

    public class TestDICommand : OaktonCommand<TestInput>
    {
        public static int Value { get; set; } = 0;
        private readonly TestDependency _dep;
        public TestDICommand(TestDependency dep)
        {
            _dep = dep;
        }

        public override bool Execute(TestInput input)
        {
            Value = _dep.Value;
            return true;
        }
    }
}
