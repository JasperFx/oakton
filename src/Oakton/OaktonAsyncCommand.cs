﻿using System;
using System.Threading.Tasks;
using Oakton.Help;

namespace Oakton
{
    /// <summary>
    /// Base class for all Oakton commands
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OaktonAsyncCommand<T> : IOaktonCommand<T>
    {
        protected OaktonAsyncCommand()
        {
            Usages = new UsageGraph(GetType());
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
            return Usages.AddUsage<T>(description);
        }

        public UsageGraph Usages { get; }

        public Type InputType => typeof(T);

        Task<bool> IOaktonCommand.Execute(object input)
        {
            return Execute((T)input);
        }

        /// <summary>
        /// The actual execution of the command. Return "false" to denote failures
        /// or "true" for successes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract Task<bool> Execute(T input);
    }
}
