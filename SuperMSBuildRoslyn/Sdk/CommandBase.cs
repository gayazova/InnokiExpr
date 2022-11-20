using System.CommandLine.Parsing;

namespace SuperMSBuildRoslyn.Sdk
{
    public abstract class CommandBase
    {
        protected ParseResult _parseResult;

        protected CommandBase(ParseResult parseResult)
        {
            _parseResult = parseResult;
            ShowHelpOrErrorIfAppropriate(parseResult);
        }

        protected virtual void ShowHelpOrErrorIfAppropriate(ParseResult parseResult)
        {
            //parseResult.ShowHelpOrErrorIfAppropriate();
        }

        public abstract int Execute();
    }
}
