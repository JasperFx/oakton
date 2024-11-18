using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Oakton.Parsing;
using Spectre.Console;

namespace Oakton.Help;

#nullable disable annotations // FIXME

public class UsageGraph
{
    private readonly List<ITokenHandler> _handlers;
    private readonly Type _inputType;
    private readonly IList<CommandUsage> _usages = new List<CommandUsage>();
    private readonly Lazy<IEnumerable<CommandUsage>> _validUsages;

    public UsageGraph(Type commandType)
    {
        _inputType = commandType.FindInterfaceThatCloses(typeof(IOaktonCommand<>)).GetTypeInfo()
            .GetGenericArguments().First();

        CommandName = CommandFactory.CommandNameFor(commandType);
        commandType.ForAttribute<DescriptionAttribute>(att => { Description = att.Description; });

        if (Description == null)
        {
            Description = commandType.Name;
        }

        _handlers = InputParser.GetHandlers(_inputType);

        _validUsages = new Lazy<IEnumerable<CommandUsage>>(() =>
        {
            if (_usages.Any())
            {
                return _usages;
            }

            var usage = new CommandUsage
            {
                Description = Description,
                Arguments = _handlers.OfType<Argument>(),
                ValidFlags = _handlers.Where(x => !(x is Argument))
            };

            return new[] { usage };
        });
    }

    public IEnumerable<ITokenHandler> Handlers => _handlers;

    public string CommandName { get; }

    public IEnumerable<Argument> Arguments => _handlers.OfType<Argument>();

    public IEnumerable<ITokenHandler> Flags
    {
        get { return _handlers.Where(x => !(x is Argument)); }
    }

    public IEnumerable<CommandUsage> Usages => _validUsages.Value;

    public string Description { get; private set; }

    public object BuildInput(Queue<string> tokens, ICommandCreator creator)
    {
        var model = creator.CreateModel(_inputType);
        var responding = new List<ITokenHandler>();

        while (tokens.Any())
        {
            var handler = _handlers.FirstOrDefault(h => h.Handle(model, tokens));
            if (handler == null)
            {
                throw new InvalidUsageException("Unknown argument or flag for value " + tokens.Peek());
            }

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

    public void WriteUsages(string appName)
    {
        if (!Usages.Any())
        {
            AnsiConsole.MarkupLine("[gray]No documentation for this command[/]");
            return;
        }

        var tree = new Tree(new Markup($"[bold]{CommandName}[/] - [italic]{Description}[/]"));

        foreach (var usage in Usages)
        {
            var usageNode = tree.AddNode(usage.Description);
            var text =
                $"[italic]{appName} {CommandName}[/] {usage.Arguments.Select(x => x.ToUsageDescription()).Join(" ")}";
            var execution = usageNode.AddNode(text);
            foreach (var flag in usage.ValidFlags)
                execution.AddNode(new Markup($"[cyan][{flag.ToUsageDescription()}][/]"));
        }

        AnsiConsole.Write(tree);
        AnsiConsole.WriteLine();

        var table = new Table
        {
            Border = TableBorder.SimpleHeavy
        };

        table.AddColumns("Usage", "Description");
        table.Columns[0].Alignment = Justify.Right;

        foreach (var argument in Arguments) table.AddRow(argument.MemberName.ToLower(), argument.Description);

        foreach (var flag in Flags) table.AddRow("[" + flag.ToUsageDescription() + "]", flag.Description);

        AnsiConsole.Write(table);
    }

    public UsageExpression<T> AddUsage<T>(string description)
    {
        return new UsageExpression<T>(this, description);
    }

    public CommandUsage FindUsage(string description)
    {
        return _usages.FirstOrDefault(x => x.Description == description);
    }

    public class UsageExpression<T>
    {
        private readonly CommandUsage _commandUsage;
        private readonly UsageGraph _parent;

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
        ///     Just expresses that this usage has no arguments
        /// </summary>
        public UsageExpression<T> NoArguments()
        {
            return Arguments();
        }


        /// <summary>
        ///     The valid arguments for this command usage in exact order
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public UsageExpression<T> Arguments(params Expression<Func<T, object>>[] properties)
        {
            _commandUsage.Arguments =
                properties.Select(
                    expr =>
                    {
                        var member = FindMembers.Determine(expr).Last();

                        return _parent.Handlers.FirstOrDefault(x => x.MemberName == member.Name);
                    }).OfType
                    <Argument>();

            return this;
        }

        /// <summary>
        ///     Optional, use this to limit the flags that are valid with this usage.  If this method is not called,
        ///     the CLI support assumes that every possible flag from the input type is valid
        /// </summary>
        /// <param name="properties"></param>
        public void ValidFlags(params Expression<Func<T, object>>[] properties)
        {
            _commandUsage.ValidFlags =
                properties.Select(
                    expr =>
                    {
                        var finder = new FindMembers();
                        finder.Visit(expr);
                        var member = finder.Members.Last();

                        return _parent.Handlers.FirstOrDefault(x => x.MemberName == member.Name);
                    }).ToArray();
        }
    }
}

#if !NET451
public static class ReflectionExtensions
{
    public static Type[] GetGenericArguments(this TypeInfo typeInfo)
    {
        var arguments = typeInfo.IsGenericTypeDefinition
            ? typeInfo.GenericTypeParameters
            : typeInfo.GenericTypeArguments;

        return arguments.ToArray();
    }
}
#endif