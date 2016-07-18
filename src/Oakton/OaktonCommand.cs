using System;
using Oakton.Help;

namespace Oakton
{
    public abstract class OaktonCommand<T> : IOaktonCommand<T>
    {
        private readonly UsageGraph _usages;

        protected OaktonCommand()
        {
            _usages = new UsageGraph(GetType());
        }

        /// <summary>
        /// If your command has multiple argument usage patterns ala the Git command line, use
        /// this method to define the valid combinations of arguments and optionally limit the flags that are valid
        /// for each usage
        /// </summary>
        /// <param name="description">The description of this usage to be displayed from the CLI help command</param>
        /// <returns></returns>
        public UsageGraph.UsageExpression<T> Usage(string description)
        {
            return _usages.AddUsage<T>(description);
        }

        public UsageGraph Usages
        {
            get { return _usages; }
        }

        public Type InputType
        {
            get
            {
                return typeof (T);
            }
        }

        public bool Execute(object input)
        {
            return Execute((T)input);
        }

        public abstract bool Execute(T input);
    }
}