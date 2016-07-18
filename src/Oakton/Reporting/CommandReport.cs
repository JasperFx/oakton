using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton.Reporting
{
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
}