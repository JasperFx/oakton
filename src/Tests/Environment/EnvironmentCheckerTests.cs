using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Oakton.Environment;
using Shouldly;
using Xunit;

namespace Tests.Environment
{
    public class EnvironmentCheckerTests
    {
        private readonly ServiceCollection Services = new ServiceCollection();

        private EnvironmentCheckResults _results;

        private EnvironmentCheckResults theResults
        {
            get
            {
                if (_results == null)
                    _results = EnvironmentChecker.ExecuteAllEnvironmentChecks(Services.BuildServiceProvider())
                        .GetAwaiter().GetResult();

                return _results;
            }
        }

        [Fact]
        public void fail_with_factory()
        {
            var factory1 = new StubEnvironmentFactory();
            factory1.Success("One");
            factory1.Success("Two");
            factory1.Success("Three");

            var factory2 = new StubEnvironmentFactory();
            factory2.Success("One");
            factory2.Success("Two");
            factory2.Failure("Three");

            Services.AddSingleton<IEnvironmentCheckFactory>(factory1);
            Services.AddSingleton<IEnvironmentCheckFactory>(factory2);

            theResults.Succeeded().ShouldBeFalse();

            theResults.Successes.Length.ShouldBe(5);
            theResults.Failures.Length.ShouldBe(1);
        }

        [Fact]
        public void happy_path_with_good_checks()
        {
            Services.CheckEnvironment("Ok", s => { });
            Services.CheckEnvironment("Fine", s => { });
            Services.CheckEnvironment("Not bad", s => { });

            theResults.Assert();
        }

        [Fact]
        public void sad_path_with_some_failures()
        {
            Services.CheckEnvironment("Ok", s => { });
            Services.CheckEnvironment("Fine", s => { });
            Services.CheckEnvironment("Not bad", s => { });
            Services.CheckEnvironment("Bad!", s => throw new NotImplementedException());
            Services.CheckEnvironment("Worse!", s => throw new NotImplementedException());

            theResults.Succeeded().ShouldBeFalse();
        }

        [Fact]
        public void success_with_factory()
        {
            var factory1 = new StubEnvironmentFactory();
            factory1.Success("One");
            factory1.Success("Two");
            factory1.Success("Three");

            var factory2 = new StubEnvironmentFactory();
            factory2.Success("One");
            factory2.Success("Two");
            factory2.Success("Three");

            Services.AddSingleton<IEnvironmentCheckFactory>(factory1);
            Services.AddSingleton<IEnvironmentCheckFactory>(factory2);

            theResults.Succeeded().ShouldBeTrue();

            theResults.Successes.Length.ShouldBe(6);
        }
    }

    public class StubEnvironmentFactory : IEnvironmentCheckFactory
    {
        public readonly IList<IEnvironmentCheck> Checks = new List<IEnvironmentCheck>();

        public IEnvironmentCheck[] Build()
        {
            return Checks.ToArray();
        }

        public void Success(string description)
        {
            Checks.Add(new GoodCheck(description));
        }

        public void Failure(string description)
        {
            Checks.Add(new BadCheck(description));
        }
    }

    public class GoodCheck : IEnvironmentCheck
    {
        public GoodCheck(string description)
        {
            Description = description;
        }

        public string Description { get; }

        public Task Assert(IServiceProvider services, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }
    }

    public class BadCheck : IEnvironmentCheck
    {
        public BadCheck(string description)
        {
            Description = description;
        }

        public string Description { get; }

        public Task Assert(IServiceProvider services, CancellationToken cancellation)
        {
            throw new InvalidOperationException(Description);
        }
    }
}