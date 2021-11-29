using System;
using Oakton;

namespace OptsFileUsageDemo
{
    public class HelloInput
    {
        public int Times { get; set; }
    }

    public class HelloCommand : OaktonCommand<HelloInput>
    {
        public override bool Execute(HelloInput input)
        {
            for (var i = 0; i < input.Times; i++)
            {
                Console.WriteLine($"{i + 1}. Hello!");
            }

            Console.WriteLine("Press any key to end.");
            Console.ReadLine();

            return true;
        }
    }
}