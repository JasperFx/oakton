using System;
using System.Reflection;
using Oakton;

namespace OaktonSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var executor = CommandExecutor.For(_ => { _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly); });

            executor.Execute(args);
        }
    }

    public class ColorInput
    {
        public ConsoleColor Color = ConsoleColor.Yellow;
    }


    public class ColorCommand : OaktonCommand<ColorInput>
    {
        public ColorCommand()
        {
            Usage("Writes Hello World in the designated color").Arguments(x => x.Color);
        }

        public override bool Execute(ColorInput input)
        {
            var original = Console.ForegroundColor;

            Console.ForegroundColor = input.Color;

            Console.WriteLine("Hello World.");

            Console.ForegroundColor = original;

            Console.WriteLine("Press any key to end.");
            Console.ReadLine();

            return true;
        }
    }

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