using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton
{
    public class CommandLineApplicationReport
    {
        private readonly IList<CommandReport> _commands = new List<CommandReport>();

        public string ApplicationName { get; set; }
        public CommandReport[] Commands
        {
            get { return _commands.ToArray(); }
            set
            {
                _commands.Clear();
                _commands.AddRange(value);
            }
        }
    }

    public class CommandReport
    {
        public string Name { get; set; }
        public string Description { get; set; }
        

        private readonly IList<ArgumentReport> _arguments = new List<ArgumentReport>();
        private readonly IList<FlagReport> _flags = new List<FlagReport>();
        private readonly IList<UsageReport> _usages = new List<UsageReport>(); 

        public ArgumentReport[] Arguments
        {
            get { return _arguments.ToArray(); }
            set
            {
                _arguments.Clear();
                _arguments.AddRange(value);
            }
        }

        public FlagReport[] Flags
        {
            get { return _flags.ToArray(); }
            set
            {
                _flags.Clear();
                _flags.AddRange(value);
            }
        }

        public UsageReport[] Usages
        {
            get { return _usages.ToArray(); }
            set
            {
                _usages.Clear();
                _usages.AddRange(value);
            }
        }
    }

    public class UsageReport
    {
        public string Description { get; set; }
        public string Usage { get; set; }
    }

    public class ArgumentReport
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class FlagReport
    {
        public FlagReport()
        {
        }

        public FlagReport(ITokenHandler token)
        {
            UsageDescription = token.ToUsageDescription();
            Description = token.Description;
        }

        public string UsageDescription { get; set; }
        public string Description { get; set; }
    }
}