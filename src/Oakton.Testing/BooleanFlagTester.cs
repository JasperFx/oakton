using System;
using System.Linq.Expressions;
using Baseline.Reflection;
using Shouldly;
using Xunit;

namespace Oakton.Testing
{
    
    public class BooleanFlagTester
    {

        private BooleanFlag getFlag(Expression<Func<BooleanFlagTarget, object>> expression)
        {
            return new BooleanFlag(expression.ToAccessor().InnerProperty);
        }


        [Fact]
        public void get_usage_description_with_an_alias()
        {
            getFlag(x => x.AliasedFlag).ToUsageDescription().ShouldBe("[-a, --aliased]");
        }

        [Fact]
        public void get_usage_description_without_an_alias()
        {
            getFlag(x => x.NormalFlag).ToUsageDescription().ShouldBe("[-n, --normal]");
        }
    }

    public class BooleanFlagTarget
    {
        [FlagAlias("aliased", 'a')]
        public bool AliasedFlag { get; set; }
        public bool NormalFlag { get; set; }
    }
}