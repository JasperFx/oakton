using System;
using System.Linq.Expressions;
using JasperFx.Core.Reflection;
using Oakton;
using Oakton.Internal.Conversion;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class ArgumentTester
    {
        private Argument argFor(Expression<Func<ArgumentTarget, object>> property)
        {
            return new Argument(ReflectionHelper.GetProperty(property), new Conversions());
        }

        [Fact]
        public void description_is_just_the_property_name_if_no_description_attribute()
        {
            argFor(x => x.Name).Description.ShouldBe("Name");
        }

        [Fact]
        public void description_comes_from_the_attribute_if_it_exists()
        {
            argFor(x => x.Age).Description.ShouldBe("age of target");
        }

        [Fact]
        public void to_usage_description_with_a_simple_string_or_number_type()
        {
            argFor(x => x.Name).ToUsageDescription().ShouldBe("<name>");
            argFor(x => x.Age).ToUsageDescription().ShouldBe("<age>");
        }

        [Fact]
        public void to_usage_description_with_an_enumeration()
        {
            argFor(x => x.Enum).ToUsageDescription().ShouldBe("red|blue|green");
        }


    }

    public enum TargetEnum
    {
        red,blue,green
    }

    public class ArgumentTarget
    {
        public string Name { get; set; }

        public TargetEnum Enum{ get; set;}

        [Description("age of target")]
        public int Age { get; set; }

    }
}