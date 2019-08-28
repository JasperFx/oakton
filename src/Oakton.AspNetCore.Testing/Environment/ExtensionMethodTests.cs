using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Microsoft.Extensions.DependencyInjection;
using Oakton.AspNetCore.Environment;
using Shouldly;
using Xunit;

namespace Oakton.AspNetCore.Testing.Environment
{
    public class ExtensionMethodTests
    {
        private readonly ServiceCollection theServices = new ServiceCollection();

        private EnvironmentCheckResults _results;

        private EnvironmentCheckResults theResults
        {
            get
            {
                if (_results == null)
                    _results = EnvironmentChecker.ExecuteAllEnvironmentChecks(theServices.BuildServiceProvider())
                        .GetAwaiter().GetResult();

                return _results;
            }
        }

        public class Thing
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        [Fact]
        public void asynchronous_action()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);
            
            theServices.CheckEnvironment("good", (s, c) => Task.CompletedTask);
            theServices.CheckEnvironment("bad", (s, c) => throw new InvalidOperationException(s.GetService<Thing>().Name));
            
            theResults.Successes.Single().ShouldBe("good");
            theResults.Failures.Single().Exception.Message.ShouldBe("Blob");
        }
        
        [Fact]
        public void synchronous_action()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);
            
            theServices.CheckEnvironment("good", (s) => {});
            theServices.CheckEnvironment("bad", s => throw new InvalidOperationException(s.GetService<Thing>().Name));
            
            theResults.Successes.Single().ShouldBe("good");
            theResults.Failures.Single().Exception.Message.ShouldBe("Blob");
        }

        [Fact]
        public void synchronous_with_service()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);
            
            theServices.CheckEnvironment<Thing>("Name cannot be Blob", t =>
            {
                if (t.Name == "Blob") throw new DivideByZeroException();
            });
            
            theResults.Failures.Single().Description.ShouldBe("Name cannot be Blob");
        }
        
        [Fact]
        public void asynchronous_with_service()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);
            
            theServices.CheckEnvironment<Thing>("Name cannot be Blob", (t, token) =>
            {
                if (t.Name == "Blob") throw new DivideByZeroException();

                return Task.CompletedTask;
            });
            
            theResults.Failures.Single().Description.ShouldBe("Name cannot be Blob");
        }

        [Fact]
        public void file_must_exist()
        {
            // Making sure we know what's going on here
            File.WriteAllText("a.txt", "something");
            File.Delete("b.txt");
            
            theServices.CheckThatFileExists("a.txt");
            theServices.CheckThatFileExists("b.txt");
            
            theResults.Successes.Single().ShouldBe("File 'a.txt' exists");
            theResults.Failures.Single().Description.ShouldBe("File 'b.txt' exists");
        }

        public interface IMissingService
        {
            
        }
        
        [Fact]
        public void check_service_is_registered_by_generic()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);

            
            theServices.CheckServiceIsRegistered<Thing>();
            theServices.CheckServiceIsRegistered<IMissingService>();
            
            theResults.Successes.Single().ShouldBe($"Service {typeof(Thing).FullName} should be registered");
            theResults.Failures.Single().Description.ShouldBe($"Service {typeof(IMissingService).FullName} should be registered");
            
        }
        
        [Fact]
        public void check_service_is_registered_by_type()
        {
            var thing = new Thing{Name = "Blob"};
            theServices.AddSingleton(thing);

            
            theServices.CheckServiceIsRegistered(typeof(Thing));
            theServices.CheckServiceIsRegistered(typeof(IMissingService));
            
            theResults.Successes.Single().ShouldBe($"Service {typeof(Thing).FullName} should be registered");
            theResults.Failures.Single().Description.ShouldBe($"Service {typeof(IMissingService).FullName} should be registered");
            
        }
    }
}