using System;
using Oakton.Environment;
using Shouldly;
using Xunit;

namespace Tests.Environment
{
    public class EnvironmentCheckResultsTests
    {
        [Fact]
        public void empty_results_asserts_just_fine()
        {
            new EnvironmentCheckResults().Assert();
        }
        
        [Fact]
        public void assert_with_only_successes()
        {
            var checkResults = new EnvironmentCheckResults();
            checkResults.RegisterSuccess("Okay");
            checkResults.RegisterSuccess("Still Okay");
            
            checkResults.Assert();
        }

        [Fact]
        public void assert_with_failures()
        {
            var checkResults = new EnvironmentCheckResults();
            
            checkResults.RegisterSuccess("Okay");
            
            checkResults.RegisterSuccess("Still Okay");
            
            checkResults.RegisterFailure("bad!", new DivideByZeroException());

            var ex = Should.Throw<EnvironmentCheckException>(() => checkResults.Assert());
            
            ex.Results.ShouldBeSameAs(checkResults);
        }

        [Fact]
        public void succeeded()
        {
            var checkResults = new EnvironmentCheckResults();
            checkResults.Succeeded().ShouldBeTrue();
            
            checkResults.RegisterSuccess("Okay");
            checkResults.Succeeded().ShouldBeTrue();
            
            checkResults.RegisterSuccess("Still Okay");
            checkResults.Succeeded().ShouldBeTrue();
            
            checkResults.RegisterFailure("bad!", new DivideByZeroException());
            checkResults.Succeeded().ShouldBeFalse();
        }
    }
}