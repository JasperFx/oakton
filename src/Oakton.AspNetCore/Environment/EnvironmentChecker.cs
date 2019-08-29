using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.AspNetCore.Environment
{
    /// <summary>
    /// Executes the environment checks registered in an IoC container 
    /// </summary>
    public static class EnvironmentChecker
    {
        public static async Task<EnvironmentCheckResults> ExecuteAllEnvironmentChecks(IServiceProvider services, CancellationToken token = default(CancellationToken))
        {
            var results = new EnvironmentCheckResults();


            var checks = services.discoverChecks().ToArray();
            if (!checks.Any()) return results;
            
            Console.WriteLine("Running Environment Checks");

            for (int i = 0; i < checks.Length; i++)
            {
                var check = checks[i];    
                
                try
                {
                    await check.Assert(services, token);
                    ConsoleWriter.Write(ConsoleColor.Green, $"{(i + 1).ToString().PadLeft(4)}.) Success: {check.Description}");
                    results.RegisterSuccess(check.Description);
                }
                catch (Exception e)
                {
                    ConsoleWriter.Write(ConsoleColor.Red, $"{(i + 1).ToString().PadLeft(4)}.) Failed: {check.Description}");
                    ConsoleWriter.Write(ConsoleColor.Yellow, e.ToString());
                    results.RegisterFailure(check.Description, e);
                }
            }

            return results;

        }

        private static IEnumerable<IEnvironmentCheck> discoverChecks(this IServiceProvider services)
        {
            foreach (var check in services.GetServices<IEnvironmentCheck>())
            {
                yield return check;
            }

            foreach (var factory in services.GetServices<IEnvironmentCheckFactory>())
            {
                foreach (var check in factory.Build())
                {
                    yield return check;
                }
            }
        }
    }


}