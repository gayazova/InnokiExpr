using Microsoft.Build.Construction;
using SuperMSBuildRoslyn;
using SuperMSBuildRoslyn.Sdk;
using System;
using System.Xml.Linq;

namespace SuperMsBuildRoslyn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var slnFolder = @"D:\source\SlnExperiments";
                var folder = @"D:\source\SlnExperiments\NewProj";
                var projName = @"NewProjName";
                var aa = new AddProjectToSolutionCommand(slnFolder, false, folder, projName);
                aa.Execute();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            Console.ReadKey();
        }

        private void CreateCsProj(string sdk, string targetFramework, string projName, string folders)
        {
            var proj = new XElement("Project", new XAttribute("Sdk", sdk),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", targetFramework)));

        }
    }
}