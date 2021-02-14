using Oakton.Parsing;
using Xunit;

namespace Tests
{
    
    public class ArgPreprocessorTester
    {
        [Fact]
        public void should_split_multi_args()
        {
            ArgPreprocessor.Process(new[] {"-abc"}).ShouldHaveTheSameElementsAs("-a", "-b", "-c");
        }

        [Fact]
        public void combined_short_flags_should_be_case_sensitive()
        {
            ArgPreprocessor.Process(new[] { "-aAbBcC" }).ShouldHaveTheSameElementsAs("-a","-A", "-b","-B", "-c","-C");
        }

        [Fact]
        public void should_ignore_long_flag_args()
        {
            ArgPreprocessor.Process(new[] {"--abc"}).ShouldHaveTheSameElementsAs("--abc");
        }

        [Fact]
        public void should_support_multiple_types_of_flags()
        {
            ArgPreprocessor.Process(new[] { "-abc", "--xyz", "b" }).ShouldHaveTheSameElementsAs("-a", "-b", "-c", "--xyz", "b");
        }

    }
}