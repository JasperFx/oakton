using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.AspNetCore.Environment
{
    public static class EnvironmentCheckExtensions
    {
        public static void EnvironmentCheck(this IServiceCollection services,
            string description,
            Func<IServiceProvider, CancellationToken, Task> test)
        {
            var check = new LambdaCheck(description, test);
            services.AddSingleton<IEnvironmentCheck>(check);
        }

        public static void EnvironmentCheck(this IServiceCollection services, string description, Action action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action();
                return Task.CompletedTask;
            });
        }
        
        public static void EnvironmentCheck<T>(this IServiceCollection services, string description, Action<T> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T>());
                return Task.CompletedTask;
            });
        }
        
        public static void EnvironmentCheck<T1, T2>(this IServiceCollection services, string description, Action<T1, T2> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T1>(), s.GetService<T2>());
                return Task.CompletedTask;
            });
        }
        
        public static void EnvironmentCheck<T1, T2, T3>(this IServiceCollection services, string description, Action<T1, T2, T3> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T1>(), s.GetService<T2>(), s.GetService<T3>());
                return Task.CompletedTask;
            });
        }
        
        
        public static void EnvironmentCheck<T>(this IServiceCollection services, string description, Func<T, CancellationToken, Task> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T>(), c);
                return Task.CompletedTask;
            });
        }
        
        public static void EnvironmentCheck<T1, T2>(this IServiceCollection services, string description, Func<T1, T2, CancellationToken, Task> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T1>(), s.GetService<T2>(), c);
                return Task.CompletedTask;
            });
        }
        
        public static void EnvironmentCheck<T1, T2, T3>(this IServiceCollection services, string description, Func<T1, T2, T3, CancellationToken, Task> action)
        {
            services.EnvironmentCheck(description, (s, c) =>
            {
                action(s.GetService<T1>(), s.GetService<T2>(), s.GetService<T3>(), c);
                return Task.CompletedTask;
            });
        }
        
        
        
        public static void FileMustExist(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }
        
        public static void DirectoryMustBeAccessible(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        public static void ServiceMustBeRegistered<T>(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
    
    
    // SAMPLE: IEnvironmentCheck
    // ENDSAMPLE


    // SAMPLE: FileExistsCheck

    // ENDSAMPLE
}