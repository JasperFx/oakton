using System;
using System.Reflection;
using Oakton;
using StructureMap;

namespace OaktonSample
{
    public class Program
    {
        // SAMPLE: bootstrapping-command-executor
        public static int Main(string[] args)
        {
            var executor = CommandExecutor.For(_ =>
            {
                // Automatically discover and register
                // all OaktonCommand's in this assembly
                _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
                
                // You can also add commands explicitly from
                // any assembly
                _.RegisterCommand<HelloCommand>();
                
                // In the absence of a recognized command name,
                // this is the default command to try to 
                // fit to the arguments provided
                _.DefaultCommand = typeof(ColorCommand);

                
                _.ConfigureRun = run =>
                {
                    // you can use this to alter the values
                    // of the inputs or actual command objects
                    // just before the command is executed
                };
                
                // This is strictly for the as yet undocumented
                // feature in stdocs to generate and embed usage information
                // about console tools built with Oakton into
                // stdocs generated documentation websites
                _.SetAppName("MyApp");
            });

            // See the page on Opts files
            executor.OptionsFile = "myapp.opts";

            return executor.Execute(args);
        }
        // ENDSAMPLE
        
        
        // SAMPLE: bootstrapping-with-custom-command-factory
        public static void Bootstrapping(IContainer container)
        {
            var executor = CommandExecutor.For(_ =>
            {
                // do the other configuration of the CommandFactory
            }, new StructureMapCommandCreator(container));
        }
        // ENDSAMPLE
    }
    

    // SAMPLE: StructureMapCommandCreator
    public class StructureMapCommandCreator : ICommandCreator
    {
        private readonly IContainer _container;

        public StructureMapCommandCreator(IContainer container)
        {
            _container = container;
        }

        public IOaktonCommand Create(Type commandType)
        {
            return (IOaktonCommand)_container.GetInstance(commandType);
        }
    }
    // ENDSAMPLE



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