namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

public class DescribeInput : NetCoreInput
{
    [Description("Optionally write the description to the given file location")]
    public string FileFlag { get; set; } = null;

    [Description("Do not write any output to the console")]
    public bool SilentFlag { get; set; } = false;

    [Description("Filter the output to only a single described part")]
    public string TitleFlag { get; set; }

    [Description("If set, the command only lists the known part titles")]
    public bool ListFlag { get; set; }

    [Description("If set, interactively select which part(s) to preview")]
    public bool InteractiveFlag { get; set; }
}