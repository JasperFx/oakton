using System.Linq;
using Shouldly;
using Xunit;

namespace Tests.Resources;

public class resource_ordering : ResourceCommandContext
{
    [Fact]
    public void respect_ordering_by_dependencies()
    {
        var one = AddResourceWithDependencies("one", "system", "blue", "red");
        var two = AddResourceWithDependencies("two", "system", "blue", "red", "one");
        
        var blue = AddResource("blue", "color");
        var red = AddResource("red", "color");
    
        var tx = AddResource("tx", "state");
        var ar = AddResource("ar", "state");
    
        var resources = applyTheResourceFiltering();
    
        resources.Count.ShouldBe(6);
        
        resources.Last().ShouldBe(two);

        resources.Select(x => x.Name).ShouldBe(new string[] { "blue", "red", "ar", "tx", "one", "two" });

    }
}