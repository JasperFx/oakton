using Oakton.Internal;
using Shouldly;
using Xunit;

namespace Tests
{
    public class filtering_launcher_args
    {
        [Fact]
        public void nothing_for_an_empty_array()
        {
            new string[0].FilterLauncherArgs()
                .Length.ShouldBe(0);
        }

        [Fact]
        public void just_the_launcher_args()
        {
            new string[]{"%launcher_args%"}.FilterLauncherArgs()
                .Length.ShouldBe(0);
        }
        
        [Fact]
        public void extra_args()
        {
            new string[]{"%launcher_args%", "command"}.FilterLauncherArgs()
                .ShouldHaveSingleItem("command");
        }
    }
}