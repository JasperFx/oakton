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
    public class FlagTester
    {
        private Flag forProp(Expression<Func<FlagTarget, object>> expression)
        {
            return new Flag(ReflectionHelper.GetProperty(expression), new Conversions());
        }

        private EnumerableFlag forArg(Expression<Func<FlagTarget, object>> expression)
        {
            return new EnumerableFlag(ReflectionHelper.GetProperty(expression), new Conversions());
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
                .ShouldBe("[-h, --herp-derp <herpderp1 herpderp2 herpderp3 ...>]");
        }

        [Fact]
        public void to_usage_description_for_a_simple_aliased_field()
        {
            forProp(x => x.AliasFlag).ToUsageDescription().ShouldBe("[-a, --aliased <alias>]");
        }

        [Fact]
        public void to_usage_description_for_a_simple_aliased_field_with_longform_only()
        {
            forProp(x => x.LongFormAliasFlag).ToUsageDescription().ShouldBe("[--longformaliased <longformalias>]");
        }


        [Fact]
        public void should_ignore_default_shortform_for_longform_only_alias()
        {            
            forProp(x => x.LongFormAliasFlag).Handle(new FlagTarget(), new Queue<string>(new[] { "-l" })).ShouldBe(false);
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

        [Fact]
        public void to_usage_description_for_string_ending_with_flag_letters()
        {
            forProp(x => x.LagFlag).ToUsageDescription().ShouldBe("[-l, --lag <lag>]");
        }

        [Fact]
        public void to_usage_description_for_flag_flag_string()
        {
            forProp(x => x.FlagFlag).ToUsageDescription().ShouldBe("[-f, --flag <flag>]");
        }

        [Fact]
        public void to_usage_description_for_flag_string()
        {
            forProp(x => x.Flag).ToUsageDescription().ShouldBe("[-f, --flag <flag>]");
        }

        [Fact]
        public void to_usage_description_for_enumerable_flag_ending_with_flag_letters()
        {
            forArg(x => x.SlagFlag)
                .ToUsageDescription()
                .ShouldBe("[-s, --slag <slag1 slag2 slag3 ...>]");
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


        [FlagAlias("longformaliased", true)]
        public string LongFormAliasFlag { get; set; }

        public IEnumerable<string> HerpDerpFlag { get; set; }

        public string LagFlag { get; set; }

        public string FlagFlag { get; set; }

        public string Flag { get; set; }

        public IEnumerable<string> SlagFlag { get; set; }
    }

    #region sample_FileInput
    public class FileInput
    {
        public string[] FilesFlag;

        public string[] DirectoriesFlag;
    }
    #endregion
}
