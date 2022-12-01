using System;
using System.IO;
using System.Text;
using JasperFx.StringExtensions;
using Oakton;

namespace CommonAssemblyInfo
{

    public class AssemblyInfoCommand : OaktonCommand<AssemblyInfoInput>
    {
        public AssemblyInfoCommand()
        {
            Usage("Generate an AssemblyInfo.cs file").Arguments(x => x.Path);
        }

        public override bool Execute(AssemblyInfoInput input)
        {
            var path = Directory.GetCurrentDirectory().AppendPath(input.Path);
            Console.WriteLine("Writing an AssemblyInfo to " + input.Path);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var file = new StreamWriter(stream);
                var writer = new CompoundWriter(file, Console.Out);

                input.WriteValues(writer);

                file.Flush();
            }

            return true;
        }

        public class CompoundWriter : TextWriter
        {
            private readonly TextWriter _inner1;
            private readonly TextWriter _inner2;

            public CompoundWriter(TextWriter inner1, TextWriter inner2)
            {
                _inner1 = inner1;
                _inner2 = inner2;
            }

            public override void Write(char value)
            {
                _inner1.Write(value);
                _inner2.Write(value);
            }

            public override void WriteLine(string value)
            {
                _inner1.WriteLine(value);
                _inner2.WriteLine(value);
            }

            public override Encoding Encoding => _inner1.Encoding;
        }
    }
}