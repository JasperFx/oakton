using Baseline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Reflection;

namespace Oakton
{
    public class UsageGraph
    {
        private readonly string _commandName;
        private readonly Type _commandType;
        private readonly IList<CommandUsage> _usages = new List<CommandUsage>();
        private string _description;
        private readonly Type _inputType;
        private readonly List<ITokenHandler> _handlers;
        private readonly Lazy<IEnumerable<CommandUsage>> _validUsages; 

        public UsageGraph(Type commandType)
        {
            _commandType = commandType;
            _inputType = commandType.FindInterfaceThatCloses(typeof (IOaktonCommand<>)).GetTypeInfo().GetGenericArguments().First();

            _commandName = CommandFactory.CommandNameFor(commandType);
            _commandType.ForAttribute<CommandDescriptionAttribute>(att => { _description = att.Description; });

            if (_description == null) _description = _commandType.Name;

            _handlers = InputParser.GetHandlers(_inputType);

            _validUsages = new Lazy<IEnumerable<CommandUsage>>(() => {
                if (_usages.Any()) return _usages;

                var usage = new CommandUsage()
                {
                    Description = _description,
                    Arguments = _handlers.OfType<Argument>(),
                    ValidFlags = _handlers.Where(x => !(x is Argument))
                };

                return new CommandUsage[]{usage};
            });
        }

        public CommandReport ToReport(string appName)
        {
            return new CommandReport
            {
                Name = _commandName,
                Description = _description,
                Arguments = Arguments.Select(x => x.ToReport()).ToArray(),
                Flags = Flags.Select(x => new FlagReport(x)).ToArray(),
                Usages = Usages.Select(x => x.ToReport(appName, _commandName)).ToArray()
            };
        }

        public object BuildInput(Queue<string> tokens)
        {
            var model = Activator.CreateInstance(_inputType);
            var responding = new List<ITokenHandler>();

            while (tokens.Any())
            {
                var handler = _handlers.FirstOrDefault(h => h.Handle(model, tokens));
                if (handler == null) throw new InvalidUsageException("Unknown argument or flag for value " + tokens.Peek());
                responding.Add(handler);
            }

            if (!IsValidUsage(responding))
            {
                throw new InvalidUsageException();
            }

            return model;
        }

        public bool IsValidUsage(IEnumerable<ITokenHandler> handlers)
        {
            return _validUsages.Value.Any(x => x.IsValidUsage(handlers));       
        }

        public IEnumerable<ITokenHandler> Handlers => _handlers;

        public string CommandName => _commandName;

        public IEnumerable<Argument> Arguments => _handlers.OfType<Argument>();

        public IEnumerable<ITokenHandler> Flags
        {
            get
            {
                return _handlers.Where(x => !(x is Argument));
            }
        }

        public IEnumerable<CommandUsage> Usages => _validUsages.Value;

        public string Description => _description;

        public void WriteUsages(string appName)
        {
            if (!Usages.Any())
            {
                Console.WriteLine("No documentation for this command");
                return;
            }

            Console.WriteLine(" Usages for '{0}' ({1})", _commandName, _description);

            if (Usages.Count() == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" " + Usages.Single().ToUsage(appName, _commandName));
                Console.ResetColor();
            }
            else
            {
                writeMultipleUsages(appName);
            }

            if(Arguments.Any())
                writeArguments();


            if (!Flags.Any()) return;

            writeFlags();
        }

        private void writeMultipleUsages(string appName)
        {
            var usageReport = new TwoColumnReport("Usages"){
                SecondColumnColor = ConsoleColor.Cyan
            };

            Usages.OrderBy(x => x.Arguments.Count()).ThenBy(x => x.ValidFlags.Count()).Each(u =>
            {
                usageReport.Add(u.Description, u.ToUsage(appName, _commandName));
            });

            usageReport.Write();
        }

        private void writeArguments()
        {
            var argumentReport = new TwoColumnReport("Arguments");
            Arguments.Each(x => argumentReport.Add(x.PropertyName.ToLower(), x.Description));
            argumentReport.Write();
        }

        private void writeFlags()
        {
            var flagReport = new TwoColumnReport("Flags");
            Flags.Each(x => flagReport.Add(x.ToUsageDescription(), x.Description));
            flagReport.Write();
        }

        public UsageExpression<T> AddUsage<T>(string description)
        {
            return new UsageExpression<T>(this, description);
        } 

        public class UsageExpression<T>
        {
            private readonly UsageGraph _parent;
            private readonly CommandUsage _commandUsage;

            public UsageExpression(UsageGraph parent, string description)
            {
                _parent = parent;

                _commandUsage = new CommandUsage
                {
                    Description = description,
                    Arguments = new Argument[0],
                    ValidFlags = _parent.Handlers.Where(x => !x.GetType().CanBeCastTo<Argument>()).ToArray() // Hokum.
                };

                _parent._usages.Add(_commandUsage);
            }

            
            /// <summary>
            /// The valid arguments for this command usage in exact order
            /// </summary>
            /// <param name="properties"></param>
            /// <returns></returns>
            public UsageExpression<T> Arguments(params Expression<Func<T, object>>[] properties)
            {
                _commandUsage.Arguments =
                    properties.Select(
                        expr => _parent.Handlers.FirstOrDefault(x => x.PropertyName == expr.ToAccessor().Name)).OfType
                        <Argument>();

                return this;
            }

            /// <summary>
            /// Optional, use this to limit the flags that are valid with this usage.  If this method is not called,
            /// the CLI support assumes that every possible flag from the input type is valid
            /// </summary>
            /// <param name="properties"></param>
            public void ValidFlags(params Expression<Func<T, object>>[] properties)
            {
                _commandUsage.ValidFlags =
                    properties.Select(
                        expr => _parent.Handlers.FirstOrDefault(x => x.PropertyName == expr.ToAccessor().Name)).ToArray();
            }
        }

        public CommandUsage FindUsage(string description)
        {
            return _usages.FirstOrDefault(x => x.Description == description);
        }
    }
}