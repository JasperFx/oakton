using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace Oakton;

#nullable disable annotations // FIXME

internal class PreBuiltHostBuilder : IHostBuilder
{
    private readonly string _notSupportedMessage;

    public PreBuiltHostBuilder(IHost host)
    {
        Host = host;
        _notSupportedMessage =
            $"The IHost ({Host}) is already constructed. See https://jasperfx.github.io/oakton for alternative bootstrapping to enable this feature.";
    }

    public IHost Host { get; }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        AnsiConsole.MarkupLine("[yellow]Cannot override host configuration when the IHost is already constructed[/]");
        return this;
    }

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        AnsiConsole.MarkupLine("[yellow]Cannot override app configuration when the IHost is already constructed[/]");
        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        AnsiConsole.MarkupLine("[yellow]Cannot override services when the IHost is already constructed[/]");
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
    {
        throw new NotSupportedException(_notSupportedMessage);
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
        Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
    {
        throw new NotSupportedException(_notSupportedMessage);
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(
        Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        throw new NotSupportedException(_notSupportedMessage);
    }

    public IHost Build()
    {
        return Host;
    }

    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();
}