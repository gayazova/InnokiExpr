using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class ParseResultExtensions
    {
        public static bool BothArchAndOsOptionsSpecified(this ParseResult parseResult) =>
    parseResult.HasOption(CommonOptions.ArchitectureOption().Aliases.First()) &&
    parseResult.HasOption(CommonOptions.OperatingSystemOption().Aliases.First());
    }
}
