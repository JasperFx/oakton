using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
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
                services.AddSingleton<TestDependency>();
                services.AddOakton(factory =>
                {
                    factory.RegisterCommand<TestDICommand>();
                });
            });

        var app = builder.Build();

        app.RunHostedOaktonCommands(new string[] { "TestDI" });

        Assert.Equal(1, TestDICommand.Value);
    }

    public class TestInput
    {
    }

    public record TestDependency(int Value = 1);

    public class TestDICommand : OaktonCommand<TestInput>
    {
        public static int Value { get; set; } = 0;
        public TestDICommand(TestDependency dep)
        {
            Value = dep.Value;
        }

        public override bool Execute(TestInput input)
        {
            return true;
        }
    }
}
