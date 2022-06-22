import{_ as e,c as n,o as t,a as o}from"./app.012e36bd.js";const p='{"title":"Improved \\"Run\\" Command","description":"","frontmatter":{},"headers":[{"level":2,"title":"Overriding the Hosting Environment","slug":"overriding-the-hosting-environment"},{"level":2,"title":"Overriding Configuration Items","slug":"overriding-configuration-items"},{"level":2,"title":"Overriding the Minimum Log Level","slug":"overriding-the-minimum-log-level"},{"level":2,"title":"Running Environment Checks Before Starting the Application","slug":"running-environment-checks-before-starting-the-application"}],"relativePath":"guide/host/run.md","lastUpdated":1655919011693}',r={},i=o(`<h1 id="improved-run-command" tabindex="-1">Improved &quot;Run&quot; Command <a class="header-anchor" href="#improved-run-command" aria-hidden="true">#</a></h1><p>To run your application normally from a command prompt with all the default configuration <strong>from the project root directory</strong>, there&#39;s no real change from what you&#39;d do without <em>Oakton.AspNetCore</em>. The command is still just:</p><div class="language-"><pre><code>dotnet run
</code></pre></div><p>If your command prompt is at the solution directory (my personal default), you can use all the available <a href="https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21" target="_blank" rel="noopener noreferrer">dotnet run</a> options, and in this case tell the dotnet CLI to run a project in another directory like this example:</p><div class="language-"><pre><code>dotnet run --project src/MvcApp/MvcApp.csproj
</code></pre></div><p>So far, no changes from what you have today, so let&#39;s dig into what&#39;s changed. First, at any time to see the list of available commands in your system, use either the command <code>dotnet run -- ?</code> or <code>dotnet run -- help</code> as shown below:</p><div class="language-"><pre><code>dotnet run -- ?
</code></pre></div><p>Which gave this output on a sample MVC application with commands from an extension library:</p><div class="language-"><pre><code>Searching &#39;AspNetCoreExtensionCommands, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&#39; for commands

  -----------------------------------------------------------------------
    Available commands:
  -----------------------------------------------------------------------
    check-env -&gt; Execute all environment checks against the application
          run -&gt; Runs the configured AspNetCore application
        smoke -&gt; Simply try to build a web host as a smoke test
  ------------------------------------------------------------------------
</code></pre></div><div class="tip custom-block"><p class="custom-block-title">warning</p><p>When you&#39;re using the <code>dotnet run</code> command, the usage of the double dashes <em>--</em> separates command line arguments to the <code>dotnet run</code> command itself from the command arguments to your application. The <code>args</code> array passed into your application will be any arguments or flags to the right of the <em>--</em> separator.</p></div><p>The <em>run</em> command shown above is the default command for Oakton and what will be executed unless you explicitly choose another named command.</p><p>Looking farther into what the <em>run</em> command provides with:</p><div class="language-"><pre><code>dotnet run -- ? run
</code></pre></div><p>gives you this help text for the options on the <em>run</em> command:</p><div class="language-"><pre><code> Usages for &#39;run&#39; (Runs the configured AspNetCore application)
  run [-c, --check] [-e, --environment &lt;environment&gt;] [-v, --verbose] [-l, --log-level &lt;logleve&gt;] [----config:&lt;prop&gt; &lt;value&gt;]

  ---------------------------------------------------------------------------------------------------------------------------------------
    Flags
  ---------------------------------------------------------------------------------------------------------------------------------------
                        [-c, --check] -&gt; Run the environment checks before starting the host
    [-e, --environment &lt;environment&gt;] -&gt; Use to override the ASP.Net Environment name
                      [-v, --verbose] -&gt; Write out much more information at startup and enables console logging
          [-l, --log-level &lt;logleve&gt;] -&gt; Override the log level
          [----config:&lt;prop&gt; &lt;value&gt;] -&gt; Overwrite individual configuration items
  ---------------------------------------------------------------------------------------------------------------------------------------
</code></pre></div><h2 id="overriding-the-hosting-environment" tabindex="-1">Overriding the Hosting Environment <a class="header-anchor" href="#overriding-the-hosting-environment" aria-hidden="true">#</a></h2><p>To override the hosting environment that your ASP.Net Core application runs under, use the <em>environment</em> flag as shown below:</p><div class="language-"><pre><code>dotnet run -- --environment Testing
</code></pre></div><p>This would be the equivalent of running your application with this code (note the usage of <code>UseEnvironment(&quot;Testing&quot;)</code>):</p><div class="language-"><pre><code>    public class Program
    {
        public static Task&lt;int&gt; Main(string[] args)
        {
            return CreateWebHostBuilder(args)
                .RunOaktonCommands(args);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =&gt;
            WebHost.CreateDefaultBuilder(args)
                .UseStartup&lt;Startup&gt;()

                // This is what the --environment flag does
                .UseEnvironment(&quot;Testing&quot;);
        
    }
</code></pre></div><p>See <a href="https://andrewlock.net/how-to-use-multiple-hosting-environments-on-the-same-machine-in-asp-net-core/" target="_blank" rel="noopener noreferrer">Andrew Lock on how the hosting environment can be useful</a>.</p><h2 id="overriding-configuration-items" tabindex="-1">Overriding Configuration Items <a class="header-anchor" href="#overriding-configuration-items" aria-hidden="true">#</a></h2><p>Individual values in your system&#39;s <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2" target="_blank" rel="noopener noreferrer">IConfiguration</a> can be overridden at the command line using the <em>--config</em> flag like so:</p><div class="language-"><pre><code>dotnet run -- --config:key1 value1 --config:key2 value2
</code></pre></div><p>The flag usage above would override the system configuration with the values <em>key1=value1</em> and <em>key2=value2</em>.</p><h2 id="overriding-the-minimum-log-level" tabindex="-1">Overriding the Minimum Log Level <a class="header-anchor" href="#overriding-the-minimum-log-level" aria-hidden="true">#</a></h2><p>You can override the minimum log level in the running application using any valid value of the <a href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-2.2" target="_blank" rel="noopener noreferrer">LogLevel</a> enumeration and the <em>--log-level</em> flag as shown below:</p><div class="language-"><pre><code>dotnet run -- --log-level Information
</code></pre></div><p>See <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2" target="_blank" rel="noopener noreferrer">Logging in .NET Core and ASP.NET Core</a> for more information about ASP.Net Core logging.</p><h2 id="running-environment-checks-before-starting-the-application" tabindex="-1">Running Environment Checks Before Starting the Application <a class="header-anchor" href="#running-environment-checks-before-starting-the-application" aria-hidden="true">#</a></h2><p>You can also opt into running any configured <a href="/guide/host/environment.html">environment checks</a> before starting Kestrel. If any of the environment checks fail, the application startup will fail. The goal of this feature is to make deployments be self-diagnosing and fail fast at startup time if the system can detect problems in its configuration or with its dependencies.</p><p>To run the environment checks as part of the run command, just use the <em>--environment</em> flag like this:</p><div class="language-"><pre><code>dotnet run -- --environment
</code></pre></div>`,33),a=[i];function s(l,d,c,m,u,g){return t(),n("div",null,a)}var v=e(r,[["render",s]]);export{p as __pageData,v as default};
