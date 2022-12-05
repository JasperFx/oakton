using System;
using System.IO;
using JasperFx.Core;
using Oakton.Parsing;
using Shouldly;
using Xunit;

namespace Tests
{
    public class OptionReaderTester
    {
#if NET451
        private string directory = AppDomain.CurrentDomain.BaseDirectory;
#else
        private string directory = AppContext.BaseDirectory;
#endif

        [Fact]
        public void read_from_one_line()
        {
            var path = directory.AppendPath("opts1.txt");
            File.WriteAllText(path, "-f -a -b");

            OptionReader.Read(path)
                .ShouldBe("-f -a -b");
        }

        [Fact]
        public void read_from_multiple_lines()
        {
            var path = directory.AppendPath("opts2.txt");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var writer = new StreamWriter(stream);

                writer.WriteLine("--color Blue");
                writer.WriteLine("--size Medium    ");
                writer.WriteLine("   --direction East");

                writer.Flush();

                writer.Dispose();
            }

            OptionReader.Read(path)
                .ShouldBe("--color Blue --size Medium --direction East");
        }
    }
}