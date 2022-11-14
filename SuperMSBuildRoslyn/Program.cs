using Microsoft.CodeAnalysis;
using System;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using Microsoft.Build.Locator;
using System.IO;

namespace SuperMsBuildRoslyn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateProject();
        }

        private void PrintProjects()
        {
            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances.Length == 1
                // If there is only one instance of MSBuild on this machine, set that as the one to use.
                ? visualStudioInstances[0]
                // Handle selecting the version of MSBuild you want to use.
                : SelectVisualStudioInstance(visualStudioInstances);
            Console.WriteLine($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");
            MSBuildLocator.RegisterInstance(instance);

            var solutionPath = @"D:\FromC\source\innostage\Innokit.sln";
            using (var msWorkspace = MSBuildWorkspace.Create())
            {
                msWorkspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

                msWorkspace.LoadMetadataForReferencedProjects = true;
                var solution = msWorkspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter()).GetAwaiter().GetResult();
                Console.WriteLine($"Finished loading solution '{solutionPath}'");

                var projects = solution.Projects.ToList();
                var docs = solution.Projects.SelectMany(x => x.Documents).ToList();
                foreach (var project in solution.Projects)
                {
                    foreach (var document in project.Documents)
                    {
                        Console.WriteLine(project.Name + "\t\t\t" + document.Name);
                    }
                }
            }
            Console.WriteLine("Heheheh");
        }

        private static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
        {
            Console.WriteLine("Multiple installs of MSBuild detected please select one:");
            for (int i = 0; i < visualStudioInstances.Length; i++)
            {
                Console.WriteLine($"Instance {i + 1}");
                Console.WriteLine($"    Name: {visualStudioInstances[i].Name}");
                Console.WriteLine($"    Version: {visualStudioInstances[i].Version}");
                Console.WriteLine($"    MSBuild Path: {visualStudioInstances[i].MSBuildPath}");
            }

            while (true)
            {
                var userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out int instanceNumber) &&
                    instanceNumber > 0 &&
                    instanceNumber <= visualStudioInstances.Length)
                {
                    return visualStudioInstances[instanceNumber - 1];
                }
                Console.WriteLine("Input not accepted, try again.");
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }

        private static void CreateProject()
        {
            using (var workspace = new AdhocWorkspace())
            {
                var projectName = "CoolHelloWorldProject";
                var projectId = ProjectId.CreateNewId();
                var versionStamp = VersionStamp.Create();
                var helloWorldProject = ProjectInfo.Create(
                    projectId,
                    versionStamp,
                    projectName,
                    projectName,
                    LanguageNames.CSharp);
                var sourceText =
                    SourceText.From(
                        "class Program { static void Main() { System.Console.WriteLine(\"HelloWorld\"); } }");
                var newProject = workspace.AddProject(helloWorldProject);
                var newDocument = workspace.AddDocument(newProject.Id, "Program.cs", sourceText);
            }
            Console.WriteLine("Heheheh");
        }
    }
}