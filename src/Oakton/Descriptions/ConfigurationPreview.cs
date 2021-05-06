using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Oakton.Descriptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Spectre.Console;

    internal class ConfigurationPreview : IDescribedSystemPart, IWriteToConsole
    {
        private readonly IConfiguration _configuration;
        private const string PreviewErrorMessage = "Unable to show a preview of the configuration.";

        public ConfigurationPreview(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Title { get; } = "IConfiguration Preview";
        
        public async Task Write(TextWriter writer)
        {
            if (_configuration is IConfigurationRoot root)
            {
                await writer.WriteLineAsync(root.GetDebugView());
            }
            else
            {
                await writer.WriteLineAsync(PreviewErrorMessage);
            }
        }

        public Task WriteToConsole()
        {
            if (this._configuration is not IConfigurationRoot root)
            {
                AnsiConsole.MarkupLine($"[red]{PreviewErrorMessage}[/]");
                return Task.CompletedTask;
            }

            void RecurseChildren(IHasTreeNodes node, IEnumerable<IConfigurationSection> children)
            {
                foreach (IConfigurationSection child in children)
                {
                    var valuesAndProviders = GetValueAndProviders(root, child.Path);

                    IHasTreeNodes parent = node;
                    if (valuesAndProviders.Count == 0)
                    {
                        parent = node.AddNode($"[blue]{child.Key}[/]");
                    }
                    else
                    {
                        var current = valuesAndProviders.Pop();
                        var currentNode = node.AddNode(new Table()
                            .Border(TableBorder.None)
                            .HideHeaders()
                            .AddColumn("Key")
                            .AddColumn("Value")
                            .AddColumn("Provider")
                            .HideHeaders()
                            .AddRow($"[yellow]{child.Key}[/]", current.Value, $@"([grey]{current.Provider}[/])")
                        );

                        // Add the overriden values
                        foreach (var valueAndProvider in valuesAndProviders)
                        {
                            currentNode.AddNode(new Table()
                                .Border(TableBorder.None)
                                .HideHeaders()
                                .AddColumn("Value")
                                .AddColumn("Provider")
                                .HideHeaders()
                                .AddRow($"[strikethrough]{child.Value}[/]", $@"([grey]{current.Provider}[/])")
                            );
                        }
                    }

                    RecurseChildren(parent, child.GetChildren());
                }
            }

            var tree = new Tree(string.Empty);

            RecurseChildren(tree, root.GetChildren());

            AnsiConsole.Render(tree);

            return Task.CompletedTask;
        }

        private static Stack<(string Value, IConfigurationProvider Provider)> GetValueAndProviders(
            IConfigurationRoot root,
            string key)
        {
            var stack = new Stack<(string, IConfigurationProvider)>();
            foreach (IConfigurationProvider provider in root.Providers.Reverse())
            {
                if (provider.TryGet(key, out string value))
                {
                    stack.Push((value, provider));
                }
            }

            return stack;
        }
    }
}