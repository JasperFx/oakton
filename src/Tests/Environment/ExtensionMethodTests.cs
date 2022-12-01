using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Oakton.Environment;
using Oakton.Resources;
using Shouldly;
using Xunit;

namespace Tests.Environment
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
        public void use_a_resource_automatically_success()
        {
            var resource = Substitute.For<IStatefulResource>();
            resource.Name.Returns("Envelopes");
            resource.Type.Returns("Database");
            
            theServices.AddSingleton<IStatefulResource>(resource);
            
            theResults.Successes.Single().ShouldBe("Resource Envelopes (Database)");
        }
        
        [Fact]
        public void use_a_resource_automatically_fail()
        {
            var resource = Substitute.For<IStatefulResource>();
            resource.Name.Returns("Envelopes");
            resource.Type.Returns("Database");
            resource.Check(Arg.Any<CancellationToken>()).Throws(new DivideByZeroException());
            
            theServices.AddSingleton<IStatefulResource>(resource);
            
            theResults.Failures.Single().Description.ShouldBe("Resource Envelopes (Database)");
        }
        
        [Fact]
        public void use_a_resource_collection_automatically_success()
        {
            var resource = Substitute.For<IStatefulResource>();
            resource.Name.Returns("Envelopes");
            resource.Type.Returns("Database");

            var collection = Substitute.For<IStatefulResourceSource>();
            collection.FindResources().Returns(new List<IStatefulResource> { resource });
            theServices.AddSingleton(collection);
            
            theResults.Successes.Single().ShouldBe("Resource Envelopes (Database)");
        }
        
        [Fact]
        public void use_a_resource_collection_automatically_fail()
        {
            var resource = Substitute.For<IStatefulResource>();
            resource.Name.Returns("Envelopes");
            resource.Type.Returns("Database");
            resource.Check(Arg.Any<CancellationToken>()).Throws(new DivideByZeroException());
            
            var collection = Substitute.For<IStatefulResourceSource>();
            collection.FindResources().Returns(new List<IStatefulResource> { resource });
            theServices.AddSingleton(collection);
            
            theResults.Failures.Single().Description.ShouldBe("Resource Envelopes (Database)");
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
            
            theServices.CheckEnvironment<Thing>("Name cannot be Blob", async (t, token) =>
            {
                await Task.Delay(1.Milliseconds(), token);
                if (t.Name == "Blob") throw new DivideByZeroException();

                
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