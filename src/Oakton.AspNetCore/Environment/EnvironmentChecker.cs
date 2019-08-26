using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.AspNetCore.Environment
{
    public static class EnvironmentChecker
    {
        public static async Task ExecuteAllEnvironmentChecks(IServiceProvider services, CancellationToken token = default(CancellationToken))
        {
            var exceptions = new List<Exception>();


            var checks = services.discoverChecks().ToArray();
            if (!checks.Any()) return;
            
            ConsoleWriter.Write("Running Environment Checks");

            for (int i = 0; i < checks.Length; i++)
            {
                try
                {
                    await checks[i].Assert(services, token);
                    ConsoleWriter.Write(ConsoleColor.Green, $"{(i + 1).ToString().PadLeft(4)}.) Success: {checks[i].Description}");
                }
                catch (Exception e)
                {
                    ConsoleWriter.Write(ConsoleColor.Red, $"{(i + 1).ToString().PadLeft(4)}.) Failed: {checks[i].Description}");
                    ConsoleWriter.Write(ConsoleColor.Yellow, e.ToString());
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("Environment Checks Failed!", exceptions);
            }

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