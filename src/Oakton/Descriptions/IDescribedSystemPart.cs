using System.IO;
using System.Threading.Tasks;

namespace Oakton.Descriptions
{
    /// <summary>
    /// Interface that can be registered in your .Net Core application
    /// container to be used by the "describe" command
    /// </summary>
    public interface IDescribedSystemPart
    {
        /// <summary>
        /// Human readable title that will appear in diagnostic reports
        /// </summary>
        string Title { get; }
        
        
        string Key { get; }
        Task Write(TextWriter writer);
    }
}