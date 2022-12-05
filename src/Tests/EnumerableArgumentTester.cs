using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JasperFx.Core.Reflection;
using Oakton;
using Oakton.Internal.Conversion;
using Oakton.Parsing;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class EnumerableArgumentTester
    {
        private EnumerableArgument argFor(Expression<Func<EnumerableArgumentInput, object>> property)
        {
            return new EnumerableArgument(ReflectionHelper.GetProperty(property), new Conversions());
        }

        [Fact]
        public void description_is_just_the_property_name_if_no_description_attribute()
        {
            argFor(x => x.Names).Description.ShouldBe("Names");
        }

        [Fact]
        public void description_comes_from_the_attribute_if_it_exists()
        {
            argFor(x => x.Ages).Description.ShouldBe("ages of target");
        }

        [Fact]
        public void to_usage_description_with_a_simple_string_or_number_type()
        {
            argFor(x => x.Names).ToUsageDescription().ShouldBe("<names1 names2 names3 ...>");
            argFor(x => x.Ages).ToUsageDescription().ShouldBe("<ages1 ages2 ages3 ...>");
        }

        [Fact]
        public void x()
        {
            Console.WriteLine(argFor(x => x.OptionalFlag).ToUsageDescription());
           
        }

        [Fact]
        public void handle()
        {
            var target = new EnumerableArgumentInput();
            var queue = new Queue<string>();
            queue.Enqueue("a");
            queue.Enqueue("b");
            queue.Enqueue("c");

            argFor(x => x.Names).Handle(target, queue);

            target.Names.ShouldHaveTheSameElementsAs("a", "b", "c");
        }

    }

    #region sample_EnumerableArguments
    public class EnumerableArgumentInput
    {
        public IEnumerable<string> Names { get; set; }

        public IEnumerable<string> OptionalFlag { get; set; }
            
        public IEnumerable<TargetEnum> Enums { get; set; }

        [Description("ages of target")]
        public IEnumerable<int> Ages { get; set; }

    }
    #endregion
}
