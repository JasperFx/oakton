namespace Oakton.Descriptions
{
    public class DescribeInput : NetCoreInput
    {
        [Description("Also write an HTML report to the location specified by the --file flag")]
        public bool HtmlFlag { get; set; } = false;
        
        [Description("Optionally write the description to the given file location")]
        public string FileFlag { get; set; } = null;

        [Description("Do not write any output to the console")]
        public bool SilentFlag { get; set; } = false;
        
        [Description("Filter the output to only a single described part")]
        public string PartFlag { get; set; }
        
        [Description("If set, the command only lists the known part keys")]
        public bool ListFlag { get; set; }

    }
}