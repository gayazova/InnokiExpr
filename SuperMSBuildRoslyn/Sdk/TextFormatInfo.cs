using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public class TextFormatInfo
    {
        public TextFormatInfo()
        {
            NewLine = Environment.NewLine;
            Encoding = null;
            EndsWithEmptyLine = true;
        }

        public string NewLine { get; set; }
        public Encoding Encoding { get; set; }
        public bool EndsWithEmptyLine { get; set; }
    }
}
