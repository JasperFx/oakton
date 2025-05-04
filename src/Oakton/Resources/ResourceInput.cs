using System;
using System.Threading;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

public class ResourceInput : NetCoreInput
{
    private readonly Lazy<CancellationTokenSource> _cancellation;

    public ResourceInput()
    {
        _cancellation =
            new Lazy<CancellationTokenSource>(() => new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutFlag)));
    }

    [Description("Resource action, default is setup")]
    public ResourceAction Action { get; set; } = ResourceAction.setup;

    [Description("Timeout in seconds, default is 60")]
    public int TimeoutFlag { get; set; } = 60;

    [IgnoreOnCommandLine] public CancellationTokenSource TokenSource => _cancellation.Value;

    [Description("Optionally filter by resource type")]
    public string TypeFlag { get; set; }

    [Description("Optionally filter by resource name")]
    public string NameFlag { get; set; }
}