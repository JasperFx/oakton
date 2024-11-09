using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

public static class EnvironmentCheckExtensions
{
    /// <summary>
    ///     Issue a check against the running environment asynchronously. Throw an
    ///     exception to denote environment failures
    /// </summary>
    /// <param name="services"></param>
    /// <param name="description"></param>
    /// <param name="test"></param>
    public static void CheckEnvironment(this IServiceCollection services,
        string description,
        Func<IServiceProvider, CancellationToken, Task> test)
    {
        var check = new LambdaCheck(description, test);
        services.AddSingleton<IEnvironmentCheck>(check);
    }

    /// <summary>
    ///     Issue a check against the running environment synchronously. Throw an
    ///     exception to denote environment failures
    /// </summary>
    /// <param name="services"></param>
    /// <param name="description"></param>
    /// <param name="action"></param>
    public static void CheckEnvironment(this IServiceCollection services, string description,
        Action<IServiceProvider> action)
    {
        services.CheckEnvironment(description, (s, c) =>
        {
            action(s);
            return Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Issue a check against the running environment using a registered service of type T synchronously. Throw an
    ///     exception to denote environment failures
    /// </summary>
    /// <param name="services"></param>
    /// <param name="description"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void CheckEnvironment<T>(this IServiceCollection services, string description, Action<T> action)
    {
        services.CheckEnvironment(description, (s, c) =>
        {
            action(s.GetService<T>());
            return Task.CompletedTask;
        });
    }


    /// <summary>
    ///     Issue a check against the running environment using a registered service of type T asynchronously. Throw an
    ///     exception to denote environment failures
    /// </summary>
    /// <param name="services"></param>
    /// <param name="description"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void CheckEnvironment<T>(this IServiceCollection services, string description,
        Func<T, CancellationToken, Task> action)
    {
        services.CheckEnvironment(description, async (s, c) => { await action(s.GetService<T>(), c); });
    }

    #region sample_CheckThatFileExists

    /// <summary>
    ///     Issue an environment check for the existence of a named file
    /// </summary>
    /// <param name="services"></param>
    /// <param name="path"></param>
    public static void CheckThatFileExists(this IServiceCollection services, string path)
    {
        var check = new FileExistsCheck(path);
        services.AddSingleton<IEnvironmentCheck>(check);
    }

    #endregion

    #region sample_CheckServiceIsRegistered

    /// <summary>
    ///     Issue an environment check for the registration of a service in the underlying IoC
    ///     container
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    public static void CheckServiceIsRegistered<T>(this IServiceCollection services)
    {
        services.CheckEnvironment($"Service {typeof(T).FullName} should be registered", s => s.GetRequiredService<T>());
    }

    /// <summary>
    ///     Issue an environment check for the registration of a service in the underlying IoC
    ///     container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceType"></param>
    public static void CheckServiceIsRegistered(this IServiceCollection services, Type serviceType)
    {
        services.CheckEnvironment($"Service {serviceType.FullName} should be registered",
            s => s.GetRequiredService(serviceType));
    }

    #endregion
}