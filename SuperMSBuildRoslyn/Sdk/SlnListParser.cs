using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class SlnListParser
    {
        public static Command GetCommand()
        {
            var command = new Command("list", LocalizableStrings.ListAppFullName);

            return command;
        }
    }
}
