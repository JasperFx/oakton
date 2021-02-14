using System;
using System.Collections.Generic;
using Oakton;
using Shouldly;
using Xunit;

namespace Tests
{
    public class can_use_fields_as_arguments_and_flags
    {
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


        private FieldModel build(params string[] tokens)
        {
            var queue = new Queue<string>(tokens);
            var graph = new FieldCommand().Usages;
            var creator = new ActivatorCommandCreator();

            return (FieldModel)graph.BuildInput(queue, creator);
        }

        public class FieldModel
        {
            public string File ;
            public Color ColorFlag ;

            public Color Color ;
            public int OrderFlag ;
            public bool TrueFalseFlag ;
            [FlagAlias('T')]
            public bool TrueOrFalseFlag ;

            public IEnumerable<string> SillyFlag ;

            public bool HerpDerpFlag ;

            [FlagAlias("makesuckmode")]
            public bool MakeSuckModeFlag ;

            public IEnumerable<int> Ages ;

            [FlagAlias("aliased", 'a')]
            public string AliasedFlag ;
        }

        public class FieldCommand : OaktonCommand<FieldModel>
        {
            public FieldCommand()
            {
                Usage("default").Arguments(x => x.File, x => x.Color);
                Usage("ages").Arguments(x => x.File, x => x.Color, x => x.Ages);
            }

            public override bool Execute(FieldModel input)
            {
                throw new NotImplementedException();
            }
        }
    }
}