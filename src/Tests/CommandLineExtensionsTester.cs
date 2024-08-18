using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using System;
using System.Threading.Tasks;
using Xunit;

[assembly: OaktonCommandAssembly]

namespace Tests;

public class CommandLineExtensionsTester
{
    [Fact]
    public async Task CanInjectServicesIntoCommands()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<TestDependency>();
                services.AddSingleton<TestDICommand>();
            });

        var app = builder.Build();

        await app.RunOaktonCommands(new[] { "TestDI" }, new DICommandCreator(app.Services));

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

    public class DICommandCreator : ICommandCreator
    {
        private readonly IServiceProvider _serviceProvider;
        public DICommandCreator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IOaktonCommand CreateCommand(Type commandType)
        {
            var scope = _serviceProvider.CreateScope();
            return (IOaktonCommand)scope.ServiceProvider.GetRequiredService(commandType);
        }

        public object CreateModel(Type modelType)
        {
            return Activator.CreateInstance(modelType)!;
        }
    }
}
