using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton.Reporting
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
}