using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

public static class DescriptionExtensions
{
    /// <summary>
    ///     Register an Oakton part description for console diagnostics
    /// </summary>
    /// <param name="services"></param>
    /// <param name="described"></param>
    public static void AddDescription(this IServiceCollection services, IDescribedSystemPart described)
    {
        services.AddSingleton(described);
    }

    /// <summary>
    ///     Register a custom description part factory
    /// </summary>
    /// <param name="services"></param>
    /// <param name="factory"></param>
    public static void AddDescriptionFactory(this IServiceCollection services, IDescribedSystemPartFactory factory)
    {
        services.AddSingleton(factory);
    }

    /// <summary>
    ///     Register a custom description part factory
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddDescriptionFactory<T>(this IServiceCollection services)
        where T : class, IDescribedSystemPartFactory
    {
        services.AddSingleton<IDescribedSystemPartFactory, T>();
    }

    /// <summary>
    ///     Register an Oakton part description for console diagnostics
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddDescription<T>(this IServiceCollection services) where T : class, IDescribedSystemPart
    {
        services.AddSingleton<IDescribedSystemPart, T>();
    }

    /// <summary>
    ///     Register an Oakton part description for console diagnostics
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title"></param>
    /// <param name="describe"></param>
    /// <typeparam name="T"></typeparam>
    public static void Describe<T>(this IServiceCollection services, string title, Action<T, TextWriter> describe)
    {
        services.DescribeAsync<T>(title, (service, writer) =>
        {
            describe(service, writer);
            return Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Register an Oakton part description for console diagnostics
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title"></param>
    /// <param name="describe"></param>
    /// <typeparam name="T"></typeparam>
    public static void DescribeAsync<T>(this IServiceCollection services, string title,
        Func<T, TextWriter, Task> describe)
    {
        var part = new LambdaDescribedSystemPart<T>(title, describe);

        services.AddSingleton<IDescribedSystemPart>(part);
    }

    /// <summary>
    ///     Create a Spectre table output from a dictionary
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    public static Table BuildTableForProperties(this IDictionary<string, object> props)
    {
        var table = new Table();
        table.AddColumn("Property");
        table.AddColumn("Value");

        foreach (var (key, value) in props) table.AddRow(key, value?.ToString() ?? string.Empty);

        return table;
    }
}