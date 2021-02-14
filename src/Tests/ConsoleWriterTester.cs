using Oakton;
using Xunit;

namespace Tests
{
    
    public class ConsoleWriterTester
    {
        [Fact]
        public void TryA81Character()
        {
            ConsoleWriter.Write(new string('a',81));
        }
    }
}