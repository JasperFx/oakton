using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JasperFx.Core.Reflection;
using Oakton;
using Oakton.Help;
using Oakton.Parsing;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class InputParserTester
    {
        private readonly InputModel theInput = new InputModel();


        private ITokenHandler handlerFor(Expression<Func<InputModel, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            return InputParser.BuildHandler(property);
        }

        private bool handle(Expression<Func<InputModel, object>> expression, params string[] args)
        {
            var queue = new Queue<string>(args);

            var handler = handlerFor(expression);

            return handler.Handle(theInput, queue);
        }

        [Fact]
        public void the_handler_for_a_normal_property_not_marked_as_flag()
        {
            handlerFor(x => x.File).ShouldBeOfType<Argument>();
        }

        [Fact]
        public void the_handler_for_an_enumeration_property_marked_as_flag()
        {
            handlerFor(x => x.ColorFlag).ShouldBeOfType<Flag>();
        }

        [Fact]
        public void the_handler_for_an_enumeration_property_not_marked_as_flag()
        {
            handlerFor(x => x.Color).ShouldBeOfType<Argument>();
        }

        [Fact]
        public void the_handler_for_a_property_suffixed_by_flag()
        {
            handlerFor(x => x.OrderFlag).ShouldBeOfType<Flag>();
        }

        [Fact]
        public void the_handler_for_a_boolean_flag()
        {
            handlerFor(x => x.TrueFalseFlag).ShouldBeOfType<BooleanFlag>();
        }

        [Fact]
        public void handler_for_an_array()
        {
            handlerFor(x => x.Ages).ShouldBeOfType<EnumerableArgument>();
        }

        [Fact]
        public void get_the_long_flag_name_for_a_property()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.OrderFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldBe("--order");
        }

        [Fact]
        public void the_long_name_should_allow_for_dashes()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.TrueOrFalseFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldBe("--true-or-false");
        }

        [Fact]
        public void the_long_name_should_allow_overriding()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.MakeSuckModeFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldBe("--makesuckmode");
        }

        [Fact]
        public void get_the_short_flag_name_for_a_property()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.OrderFlag);
            InputParser.ToFlagAliases(property).ShortForm.ShouldBe("-o");
        }
        
        [Fact]
        public void get_the_long_flag_name_for_a_property_with_an_alias()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.AliasedFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldBe("--aliased");
        }

        [Fact]
        public void get_the_long_flag_name_for_property_named_flag()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.Flag);
            InputParser.ToFlagAliases(property).LongForm.ShouldBe("--flag");
        }

        [Fact]
        public void get_the_short_name_for_property_named_flag()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.Flag);
            InputParser.ToFlagAliases(property).ShortForm.ShouldBe("-f");
        }

        [Fact]
        public void get_the_short_flag_name_for_a_property_with_an_alias()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.AliasedFlag);
            InputParser.ToFlagAliases(property).ShortForm.ShouldBe("-a");
        }

        [Fact]
        public void boolean_flag_does_not_catch()
        {
            handle(x => x.TrueFalseFlag, "nottherightthing").ShouldBeFalse();
            theInput.TrueFalseFlag.ShouldBeFalse();
        }

        [Fact]
        public void boolean_flag_long_form_should_be_case_insensitive()
        {
            handle(x => x.TrueFalseFlag, "--True-False").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Fact]
        public void boolean_flag_does_catch_2()
        {
            handle(x => x.TrueFalseFlag, "--true-false").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Fact]
        public void enumerable_argument()
        {
            handle(x => x.Ages, "1", "2", "3").ShouldBeTrue();
            theInput.Ages.ShouldHaveTheSameElementsAs(1, 2, 3);
        }

        [Fact]
        public void enumeration_argument()
        {
            handle(x => x.Color, "red").ShouldBeTrue();
            theInput.Color.ShouldBe(Color.red);
        }

        [Fact]
        public void enumeration_argument_2()
        {
            handle(x => x.Color, "green").ShouldBeTrue();
            theInput.Color.ShouldBe(Color.green);
        }

        [Fact]
        public void enumeration_flag_negative()
        {
            handle(x => x.ColorFlag, "green").ShouldBeFalse();
        }

        [Fact]
        public void enumeration_flag_positive()
        {
            handle(x => x.ColorFlag, "--color", "blue").ShouldBeTrue();
            theInput.ColorFlag.ShouldBe(Color.blue);
        }
        
        [Fact]
        public void string_argument()
        {
            handle(x => x.File, "the file").ShouldBeTrue();
            theInput.File.ShouldBe("the file");
        }

        [Fact]
        public void int_flag_does_not_catch()
        {
            handle(x => x.OrderFlag, "not order flag").ShouldBeFalse();
            theInput.OrderFlag.ShouldBe(0);
        }

        [Fact]
        public void int_flag_catches()
        {
            handle(x => x.OrderFlag, "--order", "23").ShouldBeTrue();
            theInput.OrderFlag.ShouldBe(23);
        }

        private InputModel build(params string[] tokens)
        {
            var queue = new Queue<string>(tokens);
            var graph = new InputCommand().Usages;
            var creator = new ActivatorCommandCreator();

            return (InputModel) graph.BuildInput(queue, creator);
        }

        [Fact]
        public void integrated_test_arguments_only()
        {
            var input = build("file1", "red");
            input.File.ShouldBe("file1");
            input.Color.ShouldBe(Color.red);

            // default is not touched
            input.OrderFlag.ShouldBe(0);
        }

        [Fact]
        public void integrated_test_with_mix_of_flags()
        {
            var input = build("file1", "--color", "green", "blue", "--order", "12");
            input.File.ShouldBe("file1");
            input.Color.ShouldBe(Color.blue);
            input.ColorFlag.ShouldBe(Color.green);
            input.OrderFlag.ShouldBe(12);
        }

        [Fact]
        public void integrated_test_with_a_boolean_flag()
        {
            var input = build("file1", "blue", "--true-false");
            input.TrueFalseFlag.ShouldBeTrue();

            build("file1", "blue").TrueFalseFlag.ShouldBeFalse();
        }

        [Fact]
        public void long_flag_with_dashes_should_pass()
        {
            var input = build("file1", "blue", "--herp-derp");
            input.HerpDerpFlag.ShouldBeTrue();

            build("file1", "blue").HerpDerpFlag.ShouldBeFalse();
        }

        [Fact]
        public void isflag_should_match_on_double_hyphen()
        {
            InputParser.IsFlag("--f").ShouldBeTrue();
        }

        [Fact]
        public void isflag_should_not_match_without_double_hyphen()
        {
            InputParser.IsFlag("f").ShouldBeFalse();
        }

        [Fact]
        public void boolean_short_flag_does_not_catch()
        {
            handle(x => x.TrueFalseFlag, "-f").ShouldBeFalse();
            theInput.TrueFalseFlag.ShouldBeFalse();
        }

        [Fact]
        public void boolean_short_flag_does_catch()
        {
            handle(x => x.TrueFalseFlag, "-t").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Fact]
        public void boolean_short_flag_case_uppercase()
        {

            var input = build("file1", "blue", "-T");
            input.TrueFalseFlag.ShouldBeFalse();
            input.TrueOrFalseFlag.ShouldBeTrue();
        }

        [Fact]
        public void boolean_short_flag_case_lowercase()
        {

           var  input = build("file1", "blue", "-t");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeFalse();
        }

        [Fact]
        public void boolean_short_flag_case_both()
        {
            var input = build("file1", "blue", "-t", "-T");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeTrue();
        }

        [Fact]
        public void integration_test_with_enumerable_flags()
        {
            var input = build("file1", "blue", "-t", "-s", "suck", "fail", "-T");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeTrue();
            input.SillyFlag.ShouldHaveTheSameElementsAs("suck","fail");
        }

        [Fact]
        public void enumeration_short_flag_negative()
        {
            handle(x => x.ColorFlag, "green").ShouldBeFalse();
        }

        [Fact]
        public void enumeration_short_flag_positive()
        {
            handle(x => x.ColorFlag, "-c", "blue").ShouldBeTrue();
            theInput.ColorFlag.ShouldBe(Color.blue);
        }

        [Fact]
        public void IsFlag_should_match_for_short_flag()
        {
            InputParser.IsFlag("-x").ShouldBeTrue();
        }

        [Fact]
        public void IsFlag_should_match_for_long_flag()
        {
            InputParser.IsFlag("--xerces").ShouldBeTrue();
        }

        [Fact]
        public void IsFlag_negative()
        {
            InputParser.IsFlag("x").ShouldBeFalse();
        }

        [Fact]
        public void IsFlag_negative_2()
        {
            InputParser.IsFlag("---x").ShouldBeFalse();
        }

        [Fact]
        public void IsShortFlag_should_match_for_short_flag()
        {
            InputParser.IsShortFlag("-x").ShouldBeTrue();
        }

        [Fact]
        public void IsShortFlag_should_not_match_for_long_flag()
        {
            InputParser.IsShortFlag("--xerces").ShouldBeFalse();
        }

        [Fact]
        public void IsLongFlag_should_not_match_for_short_flag()
        {
            InputParser.IsLongFlag("-x").ShouldBeFalse();
        }

        [Fact]
        public void IsLongFlag_should_match_for_long_flag()
        {
            InputParser.IsLongFlag("--xerces").ShouldBeTrue();
        }

        [Fact]
        public void IsLongFlag_with_dashes_should_match_for_long_flag()
        {
            InputParser.IsLongFlag("--herp-derp").ShouldBeTrue();
        }

        [Fact]
        public void IsLongFlag_should_not_match_for_triple_long_flag()
        {
            InputParser.IsLongFlag("---xerces").ShouldBeFalse();
        }

        [Fact]
        public void RemoveFlagSuffix_should_remove_suffix()
        {
            InputParser.RemoveFlagSuffix("FlagFlag").ShouldBe("Flag");
        }

        [Fact]
        public void RemoveFlagSuffix_should_stay_same_if_is_suffix()
        {
            InputParser.RemoveFlagSuffix("Flag").ShouldBe("Flag");
        }

        [Fact]
        public void complex_usage_smoketest()
        {
            new UsageGraph(typeof(InputCommand)).WriteUsages("fubu");
        }

        [Fact]
        public void use_dictionary_flag_for_dict()
        {
            handlerFor(x => x.PropsFlag).ShouldBeOfType<DictionaryFlag>();
        }
      
    }


    public enum Color
    {
        red,
        green,
        blue
    }

    public class InputModel
    {
        public string File { get; set; }
        public Color ColorFlag { get; set; }

        public Color Color { get; set; }
        public int OrderFlag { get; set; }
        public bool TrueFalseFlag { get; set; }
        [FlagAlias('T')]
        public bool TrueOrFalseFlag { get; set; }

        public IEnumerable<string> SillyFlag { get; set; } 

        public bool HerpDerpFlag { get; set; }

        [FlagAlias("makesuckmode")]
        public bool MakeSuckModeFlag { get; set; }

        public IEnumerable<int> Ages { get; set; }

        [FlagAlias("aliased", 'a')]
        public string AliasedFlag { get; set; }
        
        public Dictionary<string, string> PropsFlag { get; set; } = new Dictionary<string, string>();

        public string Flag { get; set; }
    }

    public class InputCommand : OaktonCommand<InputModel>
    {
        public InputCommand()
        {
            Usage("default").Arguments(x => x.File, x => x.Color);
            Usage("ages").Arguments(x => x.File, x => x.Color, x => x.Ages);
        }

        public override bool Execute(InputModel input)
        {
            throw new NotImplementedException();
        }
    }
}