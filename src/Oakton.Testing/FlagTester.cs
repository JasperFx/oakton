using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Baseline.Conversion;
using Baseline.Reflection;
using Oakton.Parsing;
using Shouldly;
using Xunit;

namespace Oakton.Testing
{
    public class FlagTester
    {
        private Flag forProp(Expression<Func<FlagTarget, object>> expression)
        {
            return new Flag(expression.ToAccessor().InnerProperty, new Conversions());
        }

        private EnumerableFlag forArg(Expression<Func<FlagTarget, object>> expression)
        {
            return new EnumerableFlag(expression.ToAccessor().InnerProperty, new Conversions());
        }

        [Fact]
        public void enumerable_flag_should_handle_arguments_correctly()
        {
            var flagTarget = new FlagTarget();
            forArg(x => x.HerpDerpFlag).Handle(flagTarget, new Queue<string>(new[] {"-h", "a", "b", "-c"}));

            flagTarget.HerpDerpFlag.ShouldHaveTheSameElementsAs("a", "b");
        }

        [Fact]
        public void should_catch_invalid_enum_value()
        {
            Exception<ArgumentException>.ShouldBeThrownBy(() =>
                    forProp(x => x.EnumFlag).Handle(new FlagTarget(), new Queue<string>(new[] {"-e", "x"})));
        }

        [Fact]
        public void should_provide_error_message()
        {
            Exception<InvalidUsageException>.ShouldBeThrownBy(() =>
                        forArg(x => x.HerpDerpFlag).Handle(new FlagTarget(), new Queue<string>(new[] {"-h"})))
                .Message.ShouldBe("No values specified for flag -h.");
        }


        [Fact]
        public void should_provide_useful_error_message_when_no_value_provided()
        {
            Exception<InvalidUsageException>.ShouldBeThrownBy(() =>
            {
                forProp(x => x.AliasFlag).Handle(new FlagTarget(), new Queue<string>(new[] {"-a"}))
                    ;
            }).Message.ShouldBe("No value specified for flag -a.");
            ;
        }

        [Fact]
        public void to_usage_description_for_a_enumerable_flag()
        {
            forArg(x => x.HerpDerpFlag)
                .ToUsageDescription()
                .ShouldBe("[-h, --herp-derp [<herpderp1 herpderp2 herpderp3 ...>]]");
        }

        [Fact]
        public void to_usage_description_for_a_simple_aliased_field()
        {
            forProp(x => x.AliasFlag).ToUsageDescription().ShouldBe("[-a, --aliased <alias>]");
        }

        [Fact]
        public void to_usage_description_for_a_simple_non_aliased_field()
        {
            forProp(x => x.NameFlag).ToUsageDescription().ShouldBe("[-n, --name <name>]");
        }


        [Fact]
        public void to_usage_description_for_an_enum_field()
        {
            forProp(x => x.EnumFlag).ToUsageDescription().ShouldBe("[-e, --enum red|blue|green]");
        }
    }

    public enum FlagEnum
    {
        red,
        blue,
        green
    }

    public class FlagTarget
    {
        public string NameFlag { get; set; }

        public FlagEnum EnumFlag { get; set; }

        [FlagAlias("aliased", 'a')]
        public string AliasFlag { get; set; }

        public IEnumerable<string> HerpDerpFlag { get; set; }
    }
}