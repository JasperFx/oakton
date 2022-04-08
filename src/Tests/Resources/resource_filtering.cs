using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Tests.Resources
{
    public class resource_filtering : ResourceCommandContext
    {
        [Fact]
        public void uses_resource_source()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");

            AddSource(col =>
            {
                col.Add("purple", "color");
                col.Add("orange", "color");
            });
            
            AddSource(col =>
            {
                col.Add("green", "color");
                col.Add("white", "color");
            });
            
            var resources = applyTheResourceFiltering();

            var colors = resources.Select(x => x.Name).OrderBy(x => x)
                .ToList();
            
            colors.ShouldHaveTheSameElementsAs("blue", "green", "orange", "purple", "red", "white");
        }
        
        [Fact]
        public void no_filtering()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
    
            var tx = AddResource("tx", "state");
            var ar = AddResource("ar", "state");
    
            var resources = applyTheResourceFiltering();
    
            resources.Count.ShouldBe(4);
            
            resources.ShouldContain(blue);
            resources.ShouldContain(red);
            resources.ShouldContain(tx);
            resources.ShouldContain(ar);
        }
    
        [Fact]
        public void filter_by_name()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
    
            var tx = AddResource("tx", "state");
            var ar = AddResource("ar", "state");
    
            theInput.NameFlag = "tx";
    
            var resources = applyTheResourceFiltering();
            resources.Single()
                .ShouldBe(tx);
        }
        
        [Fact]
        public void filter_by_type()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");
    
            var tx = AddResource("tx", "state");
            var ar = AddResource("ar", "state");
            var mo = AddResource("mo", "state");
    
            theInput.TypeFlag = "color";
            var resources = applyTheResourceFiltering();
            
            resources.Count.ShouldBe(3);
            resources.ShouldContain(blue);
            resources.ShouldContain(red);
            resources.ShouldContain(green);
        }
    }
}

