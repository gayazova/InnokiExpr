using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;

namespace SuperMSBuildRoslyn
{
    public class MsBuildAttempt
    {
        public static void GetSolutionFile(string slnPath)
        {
            RoslynMsBuildAttempt.RegisterLocatorInstance();

            DoThings();
        }

        private static void DoThings()
        {
            var propertyBag = new Dictionary<string, string>();
            var projPath = @"D:\source\MsBuildGen\EmptyProject\EmptyProject.csproj";
            var solutionDir = @"D:\source\MsBuildGen";
            propertyBag.Add("SolutionDir", solutionDir);
            using (var projectCollection = new ProjectCollection(propertyBag))
            {
                projectCollection.RegisterLogger(new ConsoleLogger());
                var project = projectCollection.LoadProject(projPath);
                //Import
                //Import(project, @"$(SolutionDir)\EmbeddedPaths.props");

                project.Save();
            }
            //project.Build();
            Console.ReadKey();
        }

        private static void Import(Project project, string importProject, string condition = null, string label = null, string sdk = null)
        {
            var importElement = project.Xml.CreateImportElement(importProject);
            if (!string.IsNullOrEmpty(condition))
                importElement.Condition = condition;
            if (!string.IsNullOrEmpty(label))
                importElement.Label = label;
            if(!string.IsNullOrEmpty(sdk))
                importElement.Sdk = sdk;
            project.Xml.AppendChild(importElement);
        }

        private static void SetSdk(Project project, string sdk)
            => project.Xml.Sdk = sdk;

        private static void SetDefaultTargets(Project project, string defaultTarget)
            => project.Xml.DefaultTargets = defaultTarget;

        private static void SetInitialTargets(Project project, string initialTargets)
            => project.Xml.InitialTargets = initialTargets;
    }
}
