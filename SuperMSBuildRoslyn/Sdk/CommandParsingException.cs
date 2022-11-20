using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public class CommandParsingException : Exception
    {
        public CommandParsingException(
            string message,
            ParseResult parseResult = null) : base(message)
        {
            ParseResult = parseResult;
            Data.Add("CLI_User_Displayed_Exception", true);
        }

        public ParseResult ParseResult;
    }
}
