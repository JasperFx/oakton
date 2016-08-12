using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Oakton;

namespace CommonAssemblyInfo
{
    [Description("Dynamically generates an AssemblyInfo.cs file based on the command inputs")]
    public class AssemblyInfoInput
    {
        private readonly IDictionary<string, string> _values = new Dictionary<string, string>();

        [Description("The relative file path to write the generated file")] public string Path { get; set; }

        [Description("[AssemblyDescription] value")]
        public string DescriptionFlag
        {
            set
            {
                writeAttribute<AssemblyDescriptionAttribute>(value);
            }
        }

        [Description("[AssemblyProduct] value")]
        public string ProductFlag
        {
            set
            {
                writeAttribute<AssemblyProductAttribute>(value);
            }
        }

        [Description("[AssemblyCopyright] value")]
        public string CopyrightFlag
        {
            set
            {
                writeAttribute<AssemblyCopyrightAttribute>(value);
            }
        }

        [Description("[AssemblyTrademark] value")]
        public string TrademarkFlag
        {
            set
            {
                writeAttribute<AssemblyTrademarkAttribute>(value);
            }
        }

        [Description("[AssemblyVersion] value")]
        public string VersionFlag
        {
            set
            {
                writeAttribute<AssemblyVersionAttribute>(value);
            }
        }

        [Description("[AssemblyFileVersion] value"), FlagAlias("file-alias", 'f')]
        public string FileVersionFlag
        {
            set
            {
                writeAttribute<AssemblyFileVersionAttribute>(value);
            }
        }

        [Description("[AssemblyInformationalVersion] value")]
        public string InformationalVersion
        {
            set
            {
                writeAttribute<AssemblyInformationalVersionAttribute>(value);
            }
        }

        private void writeAttribute<T>(string value)
        {
            var attName = typeof(T).Name.Replace("Attribute", "");
            if (_values.ContainsKey(attName))
            {
                _values[attName] = value;
            }
            else
            {
                _values.Add(attName, value);
            }
        }

        public void WriteValues(TextWriter writer)
        {
            writer.WriteLine("using System.Reflection;");
            writer.WriteLine("using System.Runtime.InteropServices;");
            writer.WriteLine("");

            foreach (var pair in _values)
            {
                writer.WriteLine($"[assembly: {pair.Key}(\"{pair.Value}\")]");
            }
        }
    }
}