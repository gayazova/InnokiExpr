using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public class SlnCommandParser
    {
        public static readonly Argument<string> SlnArgument = new Argument<string>(LocalizableStrings.SolutionArgumentName)
        {
            Description = LocalizableStrings.SolutionArgumentDescription,
            Arity = ArgumentArity.ExactlyOne
        }.DefaultToCurrentDirectory();

        public static Command GetCommand()
        {
            var command = new Command("sln", LocalizableStrings.AppFullName);

            command.AddArgument(SlnArgument);
            command.AddCommand(SlnAddParser.GetCommand());
            command.AddCommand(SlnListParser.GetCommand());
            command.AddCommand(SlnRemoveParser.GetCommand());

            return command;
        }
    }
}
