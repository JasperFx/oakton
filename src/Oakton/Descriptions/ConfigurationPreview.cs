using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Oakton.Descriptions
{
    internal class ConfigurationPreview : IDescribedSystemPart
    {
        private readonly IConfiguration _configuration;

        public ConfigurationPreview(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Title { get; } = "IConfiguration Preview";
        public async Task Write(TextWriter writer)
        {
            if (_configuration is IConfigurationRoot root)
            {
                await writer.WriteLineAsync(root.GetDebugView());
            }
            else
            {
                await writer.WriteLineAsync("Unable to show a preview of the configuration.");
            }
        }
    }
}