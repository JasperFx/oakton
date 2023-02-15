import{_ as s,c as n,o as a,a as t}from"./app.e640da3b.js";const h='{"title":"Stateful Resources","description":"","frontmatter":{},"headers":[{"level":2,"title":"IStatefulResource Adapter","slug":"istatefulresource-adapter"},{"level":2,"title":"At Startup Time","slug":"at-startup-time"},{"level":2,"title":"At Testing Time","slug":"at-testing-time"},{"level":2,"title":"\\"resources\\" Command","slug":"resources-command"}],"relativePath":"guide/host/resources.md","lastUpdated":1676460857058}',e={},o=t(`<h1 id="stateful-resources" tabindex="-1">Stateful Resources <a class="header-anchor" href="#stateful-resources" aria-hidden="true">#</a></h1><div class="tip custom-block"><p class="custom-block-title">TIP</p><p>This feature was added in Oakton 4.5.0</p></div><p>When you&#39;re working with the codebase of an application or service, you&#39;re also likely to also be working with external infrastructure like databases or messaging brokers. Taking the example of a database, at various times during development you may want to:</p><ul><li>Set up the database schema from a brand new database installation</li><li>Completely tear down the database schema to reclaim resources</li><li>Clear all existing data out of the development database, but leave the schema in place</li><li>Check that your code can indeed connect to the database</li><li>Maybe interrogate the database for some kind of metrics that helps you test or troubleshoot your code</li></ul><p>To that end, Oakton has the <code>IStatefulResource</code> model and the new <code>resources</code> command as a way of interacting with these stateful resources like databases or messaging brokers from the command line or even at application startup time.</p><h2 id="istatefulresource-adapter" tabindex="-1">IStatefulResource Adapter <a class="header-anchor" href="#istatefulresource-adapter" aria-hidden="true">#</a></h2><div class="tip custom-block"><p class="custom-block-title">TIP</p><p>Oakton assumes that there will be anywhere from 0 to many stateful resources in your application.</p></div><p>The first element is the <code>Oakton.Resources.IStatefulResource</code> interface shown below:</p><p><a id="snippet-sample_istatefulresource"></a></p><div class="language-cs"><pre><code><span class="token comment">/// &lt;summary&gt;</span>
<span class="token comment">///     Adapter interface used by Oakton enabled applications to allow</span>
<span class="token comment">///     Oakton to setup/teardown/clear the state/check on stateful external</span>
<span class="token comment">///     resources of the system like databases or messaging queues</span>
<span class="token comment">/// &lt;/summary&gt;</span>
<span class="token keyword">public</span> <span class="token keyword">interface</span> <span class="token class-name">IStatefulResource</span>
<span class="token punctuation">{</span>
    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Categorical type name of this resource for filtering</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token return-type class-name"><span class="token keyword">string</span></span> Type <span class="token punctuation">{</span> <span class="token keyword">get</span><span class="token punctuation">;</span> <span class="token punctuation">}</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Identifier for this resource</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token return-type class-name"><span class="token keyword">string</span></span> Name <span class="token punctuation">{</span> <span class="token keyword">get</span><span class="token punctuation">;</span> <span class="token punctuation">}</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Check whether the configuration for this resource is valid. An exception</span>
    <span class="token comment">///     should be thrown if the check is invalid</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token comment">/// &lt;param name=&quot;token&quot;&gt;&lt;/param&gt;</span>
    <span class="token comment">/// &lt;returns&gt;&lt;/returns&gt;</span>
    <span class="token return-type class-name">Task</span> <span class="token function">Check</span><span class="token punctuation">(</span><span class="token class-name">CancellationToken</span> token<span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Clear any persisted state within this resource</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token comment">/// &lt;param name=&quot;token&quot;&gt;&lt;/param&gt;</span>
    <span class="token comment">/// &lt;returns&gt;&lt;/returns&gt;</span>
    <span class="token return-type class-name">Task</span> <span class="token function">ClearState</span><span class="token punctuation">(</span><span class="token class-name">CancellationToken</span> token<span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Tear down the stateful resource represented by this implementation</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token comment">/// &lt;param name=&quot;token&quot;&gt;&lt;/param&gt;</span>
    <span class="token comment">/// &lt;returns&gt;&lt;/returns&gt;</span>
    <span class="token return-type class-name">Task</span> <span class="token function">Teardown</span><span class="token punctuation">(</span><span class="token class-name">CancellationToken</span> token<span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Make any necessary configuration to this stateful resource</span>
    <span class="token comment">///     to make the system function correctly</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token comment">/// &lt;param name=&quot;token&quot;&gt;&lt;/param&gt;</span>
    <span class="token comment">/// &lt;returns&gt;&lt;/returns&gt;</span>
    <span class="token return-type class-name">Task</span> <span class="token function">Setup</span><span class="token punctuation">(</span><span class="token class-name">CancellationToken</span> token<span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">/// &lt;summary&gt;</span>
    <span class="token comment">///     Optionally return a report of the current state of this resource</span>
    <span class="token comment">/// &lt;/summary&gt;</span>
    <span class="token comment">/// &lt;param name=&quot;token&quot;&gt;&lt;/param&gt;</span>
    <span class="token comment">/// &lt;returns&gt;&lt;/returns&gt;</span>
    <span class="token return-type class-name">Task<span class="token punctuation">&lt;</span>IRenderable<span class="token punctuation">&gt;</span></span> <span class="token function">DetermineStatus</span><span class="token punctuation">(</span><span class="token class-name">CancellationToken</span> token<span class="token punctuation">)</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Oakton/Resources/IStatefulResource.cs#L7-L64" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_istatefulresource" title="Start of snippet">anchor</a></sup></p><p>You can create a new adapter for your infrastructure by implementing this interface and registering a service in your .Net application&#39;s DI container. As an example, Jasper creates an <code>IStatefulResource</code> adapter to <a href="https://github.com/JasperFx/jasper/blob/36f86aa20634e8839d7d68838e2a9f5b2b023ef0/src/Jasper.RabbitMQ/Internal/RabbitMqTransport.Resource.cs" target="_blank" rel="noopener noreferrer">its Rabbit MQ integration</a> to allow Oakton to setup, teardown, purge, and check on the expected Rabbit MQ queues for an application.</p><p>The second abstraction is the smaller <code>Oakton.Resources.IStatefulResourceSource</code> that&#39;s just a helper to &quot;find&quot; other stateful resources. The <a href="https://github.com/jasperfx/weasel" target="_blank" rel="noopener noreferrer">Weasel library</a> exposes the <a href="https://github.com/JasperFx/weasel/blob/606099d2cbbb0505ea93b10af0118cfbeda20657/src/Weasel.CommandLine/DatabaseResources.cs" target="_blank" rel="noopener noreferrer">DatabaseResources</a> adapter to &quot;find&quot; all the known Weasel managed databases to enable Oakton&#39;s stateful resource management.</p><p><a id="snippet-sample_istatefulresourcesource"></a></p><div class="language-cs"><pre><code><span class="token comment">/// &lt;summary&gt;</span>
<span class="token comment">///     Expose multiple stateful resources</span>
<span class="token comment">/// &lt;/summary&gt;</span>
<span class="token keyword">public</span> <span class="token keyword">interface</span> <span class="token class-name">IStatefulResourceSource</span>
<span class="token punctuation">{</span>
    <span class="token return-type class-name">IReadOnlyList<span class="token punctuation">&lt;</span>IStatefulResource<span class="token punctuation">&gt;</span></span> <span class="token function">FindResources</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Oakton/Resources/IStatefulResourceSource.cs#L5-L15" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_istatefulresourcesource" title="Start of snippet">anchor</a></sup></p><p>To make the implementations easier, there is also an <code>Oakton.Resources.StatefulResourceBase</code> base class you can use to make stateful resource adapters that only implement some of the possible operations.</p><div class="tip custom-block"><p class="custom-block-title">TIP</p><p>Oakton automatically adds <a href="/guide/host/environment.html">environment checks</a> for each stateful resource using its <code>Check()</code> method</p></div><h2 id="at-startup-time" tabindex="-1">At Startup Time <a class="header-anchor" href="#at-startup-time" aria-hidden="true">#</a></h2><p>Forget the command line for a second, if you have service registrations for <code>IStatefulResource</code>, you&#39;ve got some available tooling at runtime.</p><p>First, to just have your system automatically setup all resources on startup, use this option:</p><p><a id="snippet-sample_using_addresourcesetuponstartup"></a></p><div class="language-cs"><pre><code><span class="token keyword">using</span> <span class="token class-name"><span class="token keyword">var</span></span> host <span class="token operator">=</span> <span class="token keyword">await</span> Host<span class="token punctuation">.</span><span class="token function">CreateDefaultBuilder</span><span class="token punctuation">(</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">ConfigureServices</span><span class="token punctuation">(</span>services <span class="token operator">=&gt;</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// More service registrations like this is a real app!</span>

        services<span class="token punctuation">.</span><span class="token function">AddResourceSetupOnStartup</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
    <span class="token punctuation">}</span><span class="token punctuation">)</span><span class="token punctuation">.</span><span class="token function">StartAsync</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Tests/Resources/ResourceHostExtensionsTests.cs#L19-L29" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_using_addresourcesetuponstartup" title="Start of snippet">anchor</a></sup></p><p>The code above adds a custom <code>IHostedService</code> at the front of the line to call the <code>Setup()</code> method on each registered <code>IStatefulResource</code> in your application.</p><p>The exact same functionality can be used with slightly different syntax:</p><p><a id="snippet-sample_using_addresourcesetuponstartup2"></a></p><div class="language-cs"><pre><code><span class="token keyword">using</span> <span class="token class-name"><span class="token keyword">var</span></span> host <span class="token operator">=</span> <span class="token keyword">await</span> Host<span class="token punctuation">.</span><span class="token function">CreateDefaultBuilder</span><span class="token punctuation">(</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">ConfigureServices</span><span class="token punctuation">(</span>services <span class="token operator">=&gt;</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// More service registrations like this is a real app!</span>
    <span class="token punctuation">}</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">UseResourceSetupOnStartup</span><span class="token punctuation">(</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">StartAsync</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Tests/Resources/ResourceHostExtensionsTests.cs#L34-L44" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_using_addresourcesetuponstartup2" title="Start of snippet">anchor</a></sup></p><p>Or, you can only have this applied when the system is running in &quot;Development&quot; mode:</p><p><a id="snippet-sample_using_addresourcesetuponstartup3"></a></p><div class="language-cs"><pre><code><span class="token keyword">using</span> <span class="token class-name"><span class="token keyword">var</span></span> host <span class="token operator">=</span> <span class="token keyword">await</span> Host<span class="token punctuation">.</span><span class="token function">CreateDefaultBuilder</span><span class="token punctuation">(</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">ConfigureServices</span><span class="token punctuation">(</span>services <span class="token operator">=&gt;</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// More service registrations like this is a real app!</span>
    <span class="token punctuation">}</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">UseResourceSetupOnStartupInDevelopment</span><span class="token punctuation">(</span><span class="token punctuation">)</span>
    <span class="token punctuation">.</span><span class="token function">StartAsync</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Tests/Resources/ResourceHostExtensionsTests.cs#L49-L59" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_using_addresourcesetuponstartup3" title="Start of snippet">anchor</a></sup></p><h2 id="at-testing-time" tabindex="-1">At Testing Time <a class="header-anchor" href="#at-testing-time" aria-hidden="true">#</a></h2><p>There are some extension methods on <code>IHost</code> in the <code>Oakton.Resources</code> namespace that you may find helpful at testing or development time:</p><p><a id="snippet-sample_programmatically_control_resources"></a></p><div class="language-cs"><pre><code><span class="token keyword">public</span> <span class="token keyword">static</span> <span class="token keyword">async</span> <span class="token return-type class-name">Task</span> <span class="token function">usages_for_testing</span><span class="token punctuation">(</span><span class="token class-name">IHost</span> host<span class="token punctuation">)</span>
<span class="token punctuation">{</span>
    <span class="token comment">// Programmatically call Setup() on all resources</span>
    <span class="token keyword">await</span> host<span class="token punctuation">.</span><span class="token function">SetupResources</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
    
    <span class="token comment">// Maybe between integration tests, clear any</span>
    <span class="token comment">// persisted state. For example, I&#39;ve used this to </span>
    <span class="token comment">// purge Rabbit MQ queues between tests</span>
    <span class="token keyword">await</span> host<span class="token punctuation">.</span><span class="token function">ResetResourceState</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">// Tear it all down!</span>
    <span class="token keyword">await</span> host<span class="token punctuation">.</span><span class="token function">TeardownResources</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/Tests/Resources/ResourceHostExtensionsTests.cs#L62-L78" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_programmatically_control_resources" title="Start of snippet">anchor</a></sup></p><h2 id="resources-command" tabindex="-1">&quot;resources&quot; Command <a class="header-anchor" href="#resources-command" aria-hidden="true">#</a></h2><div class="tip custom-block"><p class="custom-block-title">TIP</p><p>The <em>list</em> option was added in Oakton 4.6.0.</p></div><p>Because Oakton is primarily about command line usage, you can of course interact with your stateful resources through the command line using the <code>resources</code> command that&#39;s automatically added with Oakton usage. If you&#39;ll type <code>dotnet run -- help resources</code> at the command line of your application, you&#39;ll get this output:</p><div class="language-bash"><pre><code>resources - Check, setup, or teardown stateful resources of this system
\u251C\u2500\u2500 Ensure all stateful resources are <span class="token builtin class-name">set</span> up
\u2502   \u2514\u2500\u2500 dotnet run -- resources
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-t, --timeout <span class="token operator">&lt;</span>timeout<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-t, --type <span class="token operator">&lt;</span>type<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-n, --name <span class="token operator">&lt;</span>name<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-e, --environment <span class="token operator">&lt;</span>environment<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-v, --verbose<span class="token punctuation">]</span>
\u2502       \u251C\u2500\u2500 <span class="token punctuation">[</span>-l, --log-level <span class="token operator">&lt;</span>loglevel<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2502       \u2514\u2500\u2500 <span class="token punctuation">[</span>----config:<span class="token operator">&lt;</span>prop<span class="token operator">&gt;</span> <span class="token operator">&lt;</span>value<span class="token operator">&gt;</span><span class="token punctuation">]</span>
\u2514\u2500\u2500 Execute an action against all resources
    \u2514\u2500\u2500 dotnet run -- resources <span class="token function">clear</span><span class="token operator">|</span>teardown<span class="token operator">|</span>setup<span class="token operator">|</span>statistics<span class="token operator">|</span>check<span class="token operator">|</span>list
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-t, --timeout <span class="token operator">&lt;</span>timeout<span class="token operator">&gt;</span><span class="token punctuation">]</span>
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-t, --type <span class="token operator">&lt;</span>type<span class="token operator">&gt;</span><span class="token punctuation">]</span>
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-n, --name <span class="token operator">&lt;</span>name<span class="token operator">&gt;</span><span class="token punctuation">]</span>
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-e, --environment <span class="token operator">&lt;</span>environment<span class="token operator">&gt;</span><span class="token punctuation">]</span>
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-v, --verbose<span class="token punctuation">]</span>
        \u251C\u2500\u2500 <span class="token punctuation">[</span>-l, --log-level <span class="token operator">&lt;</span>loglevel<span class="token operator">&gt;</span><span class="token punctuation">]</span>
        \u2514\u2500\u2500 <span class="token punctuation">[</span>----config:<span class="token operator">&lt;</span>prop<span class="token operator">&gt;</span> <span class="token operator">&lt;</span>value<span class="token operator">&gt;</span><span class="token punctuation">]</span>


                              Usage   Description
\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500
                             action   Resource action, default is setup
          <span class="token punctuation">[</span>-t, --timeout <span class="token operator">&lt;</span>timeout<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Timeout <span class="token keyword">in</span> seconds, default is <span class="token number">60</span>
                <span class="token punctuation">[</span>-t, --type <span class="token operator">&lt;</span>type<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Optionally filter by resource <span class="token builtin class-name">type</span>
                <span class="token punctuation">[</span>-n, --name <span class="token operator">&lt;</span>name<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Optionally filter by resource name
  <span class="token punctuation">[</span>-e, --environment <span class="token operator">&lt;</span>environment<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Use to override the ASP.Net Environment name
                    <span class="token punctuation">[</span>-v, --verbose<span class="token punctuation">]</span>   Write out much <span class="token function">more</span> information at startup and enables console logging
       <span class="token punctuation">[</span>-l, --log-level <span class="token operator">&lt;</span>loglevel<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Override the log level
        <span class="token punctuation">[</span>----config:<span class="token operator">&lt;</span>prop<span class="token operator">&gt;</span> <span class="token operator">&lt;</span>value<span class="token operator">&gt;</span><span class="token punctuation">]</span>   Overwrite individual configuration items
</code></pre></div><p>You&#39;ve got a couple of options. First, to just see what resources are registered in your system, use:</p><div class="language-bash"><pre><code>dotnet run -- resources list
</code></pre></div><p>To simply check on the state of each of the resources, use:</p><div class="language-bash"><pre><code>dotnet run -- resources check
</code></pre></div><p>To set up all resources, use:</p><div class="language-bash"><pre><code>dotnet run -- resources setup
</code></pre></div><p>Likewise, to teardown all resources:</p><div class="language-bash"><pre><code>dotnet run -- resources teardown
</code></pre></div><p>Or clear any existing state:</p><div class="language-bash"><pre><code>dotnet run -- resources <span class="token function">clear</span>
</code></pre></div><p>Or finally just to see any statistics:</p><div class="language-bash"><pre><code>dotnet run -- resources statistics
</code></pre></div>`,54),p=[o];function c(r,l,u,i,k,m){return a(),n("div",null,p)}var g=s(e,[["render",c]]);export{h as __pageData,g as default};
