using Microsoft.Build.Evaluation;
using Microsoft.DotNet.Cli.Utils;
using NuGet.Frameworks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SuperMSBuildRoslyn.Sdk
{
    internal class AddProjectToProjectReferenceCommand
    {
        private readonly string fileOrDirectory;

        private readonly List<string> referenceProjects = new();

        private readonly string frameworkString;

        public AddProjectToProjectReferenceCommand(string fileOrDirectory, List<string> referenceProjects, string frameworkString = "")
        {
            this.fileOrDirectory = fileOrDirectory;
            this.referenceProjects = referenceProjects;
            this.frameworkString = frameworkString;
        }

        public int Execute()
        {
            var projects = new ProjectCollection();
            bool interactive = false;
            MsbuildProject msbuildProj = MsbuildProject.FromFileOrDirectory(
                projects,
                fileOrDirectory,
                interactive);

            PathUtility.EnsureAllPathsExist(referenceProjects,
                CommonLocalizableStrings.CouldNotFindProjectOrDirectory, true);
            List<MsbuildProject> refs =
                referenceProjects
                    .Select((r) => MsbuildProject.FromFileOrDirectory(projects, r, interactive))
                    .ToList();

            if (string.IsNullOrEmpty(frameworkString))
            {
                foreach (var tfm in msbuildProj.GetTargetFrameworks())
                {
                    foreach (var @ref in refs)
                    {
                        if (!@ref.CanWorkOnFramework(tfm))
                        {
                            Reporter.Error.Write(GetProjectNotCompatibleWithFrameworksDisplayString(
                                                     @ref,
                                                     msbuildProj.GetTargetFrameworks().Select((fx) => fx.GetShortFolderName())));
                            return 1;
                        }
                    }
                }
            }
            else
            {
                var framework = NuGetFramework.Parse(frameworkString);
                if (!msbuildProj.IsTargetingFramework(framework))
                {
                    Reporter.Error.WriteLine(string.Format(
                                                 CommonLocalizableStrings.ProjectDoesNotTargetFramework,
                                                 msbuildProj.ProjectRootElement.FullPath,
                                                 frameworkString));
                    return 1;
                }

                foreach (var @ref in refs)
                {
                    if (!@ref.CanWorkOnFramework(framework))
                    {
                        Reporter.Error.Write(GetProjectNotCompatibleWithFrameworksDisplayString(
                                                 @ref,
                                                 new string[] { frameworkString }));
                        return 1;
                    }
                }
            }

            var relativePathReferences = refs.Select((r) =>
                                                        Path.GetRelativePath(
                                                            msbuildProj.ProjectDirectory,
                                                            r.ProjectRootElement.FullPath)).ToList();

            int numberOfAddedReferences = msbuildProj.AddProjectToProjectReferences(
                frameworkString,
                relativePathReferences);

            if (numberOfAddedReferences != 0)
            {
                msbuildProj.ProjectRootElement.Save();
            }

            return 0;
        }

        private static string GetProjectNotCompatibleWithFrameworksDisplayString(MsbuildProject project, IEnumerable<string> frameworksDisplayStrings)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(CommonLocalizableStrings.ProjectNotCompatibleWithFrameworks, project.ProjectRootElement.FullPath));
            foreach (var tfm in frameworksDisplayStrings)
            {
                sb.AppendLine($"    - {tfm}");
            }

            return sb.ToString();
        }
    }
}
