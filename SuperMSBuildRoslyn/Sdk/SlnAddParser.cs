
using System.Collections.Generic;
using System.CommandLine;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class SlnAddParser
    {
        public static readonly Argument<IEnumerable<string>> ProjectPathArgument = new Argument<IEnumerable<string>>("PROJECT_PATH")
        {
            Description = "The paths to the projects to add to the solution.",
            Arity = ArgumentArity.ZeroOrMore,
        };

        public static readonly Option<bool> InRootOption = new Option<bool>("--in-root", "Place project in root of the solution, rather than creating a solution folder.");

        public static readonly Option<string> SolutionFolderOption = new Option<string>(new string[] { "-s", "--solution-folder" }, "The destination solution folder path to add the projects to.");

        private static readonly Command Command = ConstructCommand();

        public static Command GetCommand()
        {
            return Command;
        }

        private static Command ConstructCommand()
        {
            var command = new Command("add", "Add one or more projects to a solution file.");

            command.AddArgument(ProjectPathArgument);
            command.AddOption(InRootOption);
            command.AddOption(SolutionFolderOption);

            return command;
        }
    }
}