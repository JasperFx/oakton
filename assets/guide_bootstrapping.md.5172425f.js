import{_ as n,c as a,o as s,a as t}from"./app.e640da3b.js";const h='{"title":"Bootstrapping with CommandExecutor","description":"","frontmatter":{},"headers":[{"level":2,"title":"Single Command","slug":"single-command"},{"level":2,"title":"Multiple Commands","slug":"multiple-commands"},{"level":2,"title":"Custom Command Creators","slug":"custom-command-creators"}],"relativePath":"guide/bootstrapping.md","lastUpdated":1676460857054}',o={},e=t(`<h1 id="bootstrapping-with-commandexecutor" tabindex="-1">Bootstrapping with CommandExecutor <a class="header-anchor" href="#bootstrapping-with-commandexecutor" aria-hidden="true">#</a></h1><p>The easiest way to bootstrap Oakton is to use the <a href="/guide/host/">integration with IHost</a>. Eschewing that, you have the options in this page.</p><p>Oakton applications can be bootstrapped either very simply with a single command, or more elaborately with options to preprocess commands, automatic command discovery, <a href="/guide/opts.html">options files</a>, or custom command object builders.</p><h2 id="single-command" tabindex="-1">Single Command <a class="header-anchor" href="#single-command" aria-hidden="true">#</a></h2><p>If all you have is a single command in your project, the bootstrapping can be as simple as this:</p><p><a id="snippet-sample_quickstart.program1"></a></p><div class="language-cs"><pre><code><span class="token keyword">class</span> <span class="token class-name">Program</span>
<span class="token punctuation">{</span>
    <span class="token keyword">static</span> <span class="token return-type class-name"><span class="token keyword">int</span></span> <span class="token function">Main</span><span class="token punctuation">(</span><span class="token class-name"><span class="token keyword">string</span><span class="token punctuation">[</span><span class="token punctuation">]</span></span> args<span class="token punctuation">)</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// As long as this doesn&#39;t blow up, we&#39;re good to go</span>
        <span class="token keyword">return</span> CommandExecutor<span class="token punctuation">.</span><span class="token generic-method"><span class="token function">ExecuteCommand</span><span class="token generic class-name"><span class="token punctuation">&lt;</span>NameCommand<span class="token punctuation">&gt;</span></span></span><span class="token punctuation">(</span>args<span class="token punctuation">)</span><span class="token punctuation">;</span>
    <span class="token punctuation">}</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/quickstart/Program.cs#L8-L17" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_quickstart.program1" title="Start of snippet">anchor</a></sup></p><h2 id="multiple-commands" tabindex="-1">Multiple Commands <a class="header-anchor" href="#multiple-commands" aria-hidden="true">#</a></h2><p>For more complex applications with multiple commands, you need to interact a little more with the <code>CommandFactory</code> configuration as shown below:</p><p><a id="snippet-sample_bootstrapping_command_executor"></a></p><div class="language-cs"><pre><code><span class="token keyword">public</span> <span class="token keyword">static</span> <span class="token return-type class-name"><span class="token keyword">int</span></span> <span class="token function">Main</span><span class="token punctuation">(</span><span class="token class-name"><span class="token keyword">string</span><span class="token punctuation">[</span><span class="token punctuation">]</span></span> args<span class="token punctuation">)</span>
<span class="token punctuation">{</span>
    <span class="token class-name"><span class="token keyword">var</span></span> executor <span class="token operator">=</span> CommandExecutor<span class="token punctuation">.</span><span class="token function">For</span><span class="token punctuation">(</span>_ <span class="token operator">=&gt;</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// Automatically discover and register</span>
        <span class="token comment">// all OaktonCommand&#39;s in this assembly</span>
        _<span class="token punctuation">.</span><span class="token function">RegisterCommands</span><span class="token punctuation">(</span><span class="token keyword">typeof</span><span class="token punctuation">(</span><span class="token type-expression class-name">Program</span><span class="token punctuation">)</span><span class="token punctuation">.</span><span class="token function">GetTypeInfo</span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">.</span>Assembly<span class="token punctuation">)</span><span class="token punctuation">;</span>
        
        <span class="token comment">// You can also add commands explicitly from</span>
        <span class="token comment">// any assembly</span>
        _<span class="token punctuation">.</span><span class="token generic-method"><span class="token function">RegisterCommand</span><span class="token generic class-name"><span class="token punctuation">&lt;</span>HelloCommand<span class="token punctuation">&gt;</span></span></span><span class="token punctuation">(</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
        
        <span class="token comment">// In the absence of a recognized command name,</span>
        <span class="token comment">// this is the default command to try to </span>
        <span class="token comment">// fit to the arguments provided</span>
        _<span class="token punctuation">.</span>DefaultCommand <span class="token operator">=</span> <span class="token keyword">typeof</span><span class="token punctuation">(</span><span class="token type-expression class-name">ColorCommand</span><span class="token punctuation">)</span><span class="token punctuation">;</span>

        
        _<span class="token punctuation">.</span>ConfigureRun <span class="token operator">=</span> run <span class="token operator">=&gt;</span>
        <span class="token punctuation">{</span>
            <span class="token comment">// you can use this to alter the values</span>
            <span class="token comment">// of the inputs or actual command objects</span>
            <span class="token comment">// just before the command is executed</span>
        <span class="token punctuation">}</span><span class="token punctuation">;</span>
        
        <span class="token comment">// This is strictly for the as yet undocumented</span>
        <span class="token comment">// feature in stdocs to generate and embed usage information</span>
        <span class="token comment">// about console tools built with Oakton into</span>
        <span class="token comment">// stdocs generated documentation websites</span>
        _<span class="token punctuation">.</span><span class="token function">SetAppName</span><span class="token punctuation">(</span><span class="token string">&quot;MyApp&quot;</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
    <span class="token punctuation">}</span><span class="token punctuation">)</span><span class="token punctuation">;</span>

    <span class="token comment">// See the page on Opts files</span>
    executor<span class="token punctuation">.</span>OptionsFile <span class="token operator">=</span> <span class="token string">&quot;myapp.opts&quot;</span><span class="token punctuation">;</span>

    <span class="token keyword">return</span> executor<span class="token punctuation">.</span><span class="token function">Execute</span><span class="token punctuation">(</span>args<span class="token punctuation">)</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/OaktonSample/Program.cs#L10-L48" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_bootstrapping_command_executor" title="Start of snippet">anchor</a></sup></p><p>Note the usage of <code>ConfigureRun</code>. See the <a href="https://github.com/JasperFx/marten/blob/master/src/Marten.CommandLine/MartenCommands.cs#L16-L21" target="_blank" rel="noopener noreferrer">Marten.CommandLine</a> usage of this extension point as an example.</p><h2 id="custom-command-creators" tabindex="-1">Custom Command Creators <a class="header-anchor" href="#custom-command-creators" aria-hidden="true">#</a></h2><div class="tip custom-block"><p class="custom-block-title">warning</p><p>Oakton was purposely built without direct support for an IoC container so users could focus on building fast console tools without the extra complexity of IoC set up</p></div><p>By default, Oakton just tries to create command objects by calling an expected default, no arg constructor with <code>Activator.CreateInstance()</code>. However, if you want to do something different like use an IoC container, you can provide a custom <code>ICommandCreator</code> like this one using <a href="http://structuremap.github.io" target="_blank" rel="noopener noreferrer">StructureMap</a>:</p><p><a id="snippet-sample_structuremapcommandcreator"></a></p><div class="language-cs"><pre><code><span class="token keyword">public</span> <span class="token keyword">class</span> <span class="token class-name">StructureMapCommandCreator</span> <span class="token punctuation">:</span> <span class="token type-list"><span class="token class-name">ICommandCreator</span></span>
<span class="token punctuation">{</span>
    <span class="token keyword">private</span> <span class="token keyword">readonly</span> <span class="token class-name">IContainer</span> _container<span class="token punctuation">;</span>

    <span class="token keyword">public</span> <span class="token function">StructureMapCommandCreator</span><span class="token punctuation">(</span><span class="token class-name">IContainer</span> container<span class="token punctuation">)</span>
    <span class="token punctuation">{</span>
        _container <span class="token operator">=</span> container<span class="token punctuation">;</span>
    <span class="token punctuation">}</span>

    <span class="token keyword">public</span> <span class="token return-type class-name">IOaktonCommand</span> <span class="token function">CreateCommand</span><span class="token punctuation">(</span><span class="token class-name">Type</span> commandType<span class="token punctuation">)</span>
    <span class="token punctuation">{</span>
        <span class="token keyword">return</span> <span class="token punctuation">(</span>IOaktonCommand<span class="token punctuation">)</span>_container<span class="token punctuation">.</span><span class="token function">GetInstance</span><span class="token punctuation">(</span>commandType<span class="token punctuation">)</span><span class="token punctuation">;</span>
    <span class="token punctuation">}</span>

    <span class="token keyword">public</span> <span class="token return-type class-name"><span class="token keyword">object</span></span> <span class="token function">CreateModel</span><span class="token punctuation">(</span><span class="token class-name">Type</span> modelType<span class="token punctuation">)</span>
    <span class="token punctuation">{</span>
        <span class="token keyword">return</span> _container<span class="token punctuation">.</span><span class="token function">GetInstance</span><span class="token punctuation">(</span>modelType<span class="token punctuation">)</span><span class="token punctuation">;</span>
    <span class="token punctuation">}</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/OaktonSample/Program.cs#L63-L83" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_structuremapcommandcreator" title="Start of snippet">anchor</a></sup></p><p>To use this custom command creator, just tell <code>CommandExecutor</code> about it like this:</p><p><a id="snippet-sample_bootstrapping_with_custom_command_factory"></a></p><div class="language-cs"><pre><code><span class="token keyword">public</span> <span class="token keyword">static</span> <span class="token return-type class-name"><span class="token keyword">void</span></span> <span class="token function">Bootstrapping</span><span class="token punctuation">(</span><span class="token class-name">IContainer</span> container<span class="token punctuation">)</span>
<span class="token punctuation">{</span>
    <span class="token class-name"><span class="token keyword">var</span></span> executor <span class="token operator">=</span> CommandExecutor<span class="token punctuation">.</span><span class="token function">For</span><span class="token punctuation">(</span>_ <span class="token operator">=&gt;</span>
    <span class="token punctuation">{</span>
        <span class="token comment">// do the other configuration of the CommandFactory</span>
    <span class="token punctuation">}</span><span class="token punctuation">,</span> <span class="token keyword">new</span> <span class="token constructor-invocation class-name">StructureMapCommandCreator</span><span class="token punctuation">(</span>container<span class="token punctuation">)</span><span class="token punctuation">)</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
</code></pre></div><p><sup><a href="https://github.com/JasperFx/oakton/blob/master/src/OaktonSample/Program.cs#L51-L59" title="Snippet source file">snippet source</a> | <a href="#snippet-sample_bootstrapping_with_custom_command_factory" title="Start of snippet">anchor</a></sup></p>`,24),p=[e];function c(u,l,i,r,m,k){return s(),a("div",null,p)}var g=n(o,[["render",c]]);export{h as __pageData,g as default};
