using Oakton;
using Shouldly;
using Xunit;

namespace Tests.Bugs
{
    public class bug_24_negative_numbers
    {
        [Description("Test")]
        public class MyInput
        {
            public int ArgNum { get; set; }

            public int NumFlag { get; set; }
        }
        
        class MyCommand : OaktonCommand<MyInput>
        {
            
            public override bool Execute(MyInput input)
            {
                return true;
            }
        }

        [Fact]
        public void should_allow_negative_numbers_in_arguments()
        {
            var factory = new CommandFactory();
            factory.RegisterCommand<MyCommand>();
            
            factory.BuildRun("my \"-3\"")
                .Input.ShouldBeOfType<MyInput>()
                .ArgNum.ShouldBe(-3);
        }

        [Fact]
        public void should_allow_negative_numbes_in_flag()
        {
            var factory = new CommandFactory();
            factory.RegisterCommand<MyCommand>();
            
            factory.BuildRun("my \"-3\" --num \"-5\"")
                .Input.ShouldBeOfType<MyInput>()
                .NumFlag.ShouldBe(-5);
        }
        
        [Theory]
        [InlineData(1, 2)]
        [InlineData(10, 20)]
        [InlineData(1, -2)]
        [InlineData(1, -20)]
        [InlineData(-1, 2)]
        [InlineData(-1, -20)]
        [InlineData(-10, -20)]
        public void ShouldBeAbleToParseAllNumbersWithQuotes(int argVal, int optVal)
        {
            var cmd = $"my \"{argVal}\" --num \"{optVal}\"";

            var f = new CommandFactory();
            f.RegisterCommand<MyCommand>();
            
            var runner = f.BuildRun(cmd);

            var input = runner.Input.ShouldBeOfType<MyInput>();
            
            input.ArgNum.ShouldBe(argVal);
            input.NumFlag.ShouldBe(optVal);
        }
    }
}