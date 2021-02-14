using System.Threading;
using System.Threading.Tasks;
using Oakton.Environment;
using Shouldly;
using Xunit;

namespace Tests.Environment
{
    public class LambdaCheckTests
    {
        [Fact]
        public void description()
        {
            var check = new LambdaCheck("it's okay", (s, t) => Task.CompletedTask);
            check.Description.ShouldBe("it's okay");
        }

        [Fact]
        public async Task call_the_assert()
        {
            bool wasCalled = false;
            var check = new LambdaCheck("it's okay", (s, t) =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            });

            await check.Assert(null, default(CancellationToken));
            
            wasCalled.ShouldBeTrue();
        }
    }
}