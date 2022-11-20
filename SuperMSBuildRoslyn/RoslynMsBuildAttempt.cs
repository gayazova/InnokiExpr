using Microsoft.CodeAnalysis;
using System;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using Microsoft.Build.Locator;
using System.IO;

namespace SuperMSBuildRoslyn
{
    public class RoslynMsBuildAttempt
    {
        public void Test()
        {
            var solutionPath = @"D:\source\MsBuildGen\slnPath.sln";
            RegisterLocatorInstance();
            using (var msWorkS = MSBuildWorkspace.Create())
            {
                var canApplyChange = msWorkS.CanApplyChange(ApplyChangesKind.AddProject);
                var solution = msWorkS.OpenSolutionAsync(solutionPath).GetAwaiter().GetResult();
                var sln = CreateProject(solution);
                if (!msWorkS.TryApplyChanges(sln))
                    Console.WriteLine
                    ("Failed to add the generated code to the project");

            }
        }
        private static Document SafeAddDocument(Project project, SyntaxTree tree, string documentName)
        {
            var targetDocument = project.Documents
            .FirstOrDefault(d => d.Name.StartsWith(documentName));
            if (targetDocument != null)
            {
                project = project.RemoveDocument(targetDocument.Id);
            }
            var doc = project.AddDocument(documentName, tree.GetRoot()
            .NormalizeWhitespace());
            return doc;
        }

        private static Project LoadEmptyProject(string slnPath, MSBuildWorkspace work)
        {
            var solution = work.OpenSolutionAsync(slnPath).GetAwaiter().GetResult();
            var project = solution.Projects.FirstOrDefault
            (p => p.Name == "EmptyProject");
            if (project == null)
                throw new Exception("Could not find the empty project");
            return project;
        }

        public static void RegisterLocatorInstance()
        {
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            MSBuildLocator.RegisterInstance(visualStudioInstances.OrderByDescending(x => x.Version).FirstOrDefault());
        }

        private static void PrintProjects()
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

            var solutionPath = @"D:\source\leetcode\leetcode.sln";
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

        public static VisualStudioInstance SelectVisualStudioInstance(VisualStudioInstance[] visualStudioInstances)
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
                Console.WriteLine("Input not accepted................... try again.");
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

        private static Solution CreateProject(Solution sln)
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
            var newProject = sln.AddProject(helloWorldProject);
            var newDocument = newProject.AddDocument(DocumentId.CreateNewId(helloWorldProject.Id), "Program.cs", sourceText);
            return newDocument;
        }
    }
}
