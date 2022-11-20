using System.CommandLine;

namespace SuperMSBuildRoslyn.Sdk
{
    //public static class AddCommandParser
    //{
    //    public static readonly Argument<string> ProjectArgument = new Argument<string>(CommonLocalizableStrings.ProjectArgumentName)
    //    {
    //        Description = CommonLocalizableStrings.ProjectArgumentDescription
    //    }.DefaultToCurrentDirectory();

    //    public static Command GetCommand()
    //    {
    //        var command = new Command("add", LocalizableStrings.NetAddCommand);

    //        command.AddArgument(ProjectArgument);
    //        command.AddCommand(AddPackageParser.GetCommand());
    //        command.AddCommand(AddProjectToProjectReferenceParser.GetCommand());

    //        return command;
    //    }
    //}

    //public static class AddPackageParser
    //{
    //    public static readonly Argument<string> CmdPackageArgument = new Argument<string>(LocalizableStrings.CmdPackage)
    //    {
    //        Description = LocalizableStrings.CmdPackageDescription
    //    }.AddSuggestions((parseResult, match) => QueryNuGet(match));

    //    public static Command GetCommand()
    //    {
    //        var command = new Command("package", LocalizableStrings.AppFullName);

    //        command.AddArgument(CmdPackageArgument);
    //        command.AddOption(VersionOption);
    //        command.AddOption(FrameworkOption);
    //        command.AddOption(NoRestoreOption);
    //        command.AddOption(SourceOption);
    //        command.AddOption(PackageDirOption);
    //        command.AddOption(InteractiveOption);
    //        command.AddOption(PrereleaseOption);

    //        return command;
    //    }
    //}
}