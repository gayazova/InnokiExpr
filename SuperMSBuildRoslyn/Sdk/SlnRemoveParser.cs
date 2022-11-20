using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class SlnRemoveParser
    {
        public static readonly Argument<IEnumerable<string>> ProjectPathArgument = new Argument<IEnumerable<string>>(LocalizableStrings.RemoveProjectPathArgumentName)
        {
            Description = LocalizableStrings.RemoveProjectPathArgumentDescription,
            Arity = ArgumentArity.ZeroOrMore
        };

        public static Command GetCommand()
        {
            var command = new Command("remove", LocalizableStrings.RemoveAppFullName);

            command.AddArgument(ProjectPathArgument);

            return command;
        }
    }
}
