using Microsoft.Build.Construction;
using SuperMSBuildRoslyn;
using SuperMSBuildRoslyn.Sdk;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SuperMsBuildRoslyn
{
    public class Program
    {
        private static string slnPath = @"C:\Users\ASUS\source\repos\SlnExpr\SlnExpr.sln";

        private static string slnDirectory => Path.GetDirectoryName(slnPath);

        public static void Main(string[] args)
        {
            //CreateProject("newProj");
            //AddToSln();
            //AddDirectBuildProps();
            Console.ReadKey();
        }

        private static void AddToSln()
        {
            try
            {
                RoslynMsBuildAttempt.RegisterLocatorInstance();
                var slnFolder = @"C:\Users\ASUS\source\repos\SlnExpr";
                var projName = @"C:\Users\ASUS\source\repos\SlnExpr\newProj\newProj.csproj";
                var aa = new AddProjectToSolutionCommand(slnFolder, true, null, projName);
                aa.Execute();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static XElement CreateCsProj(string sdk, string targetFramework)
        {
            return new XElement("Project", new XAttribute("Sdk", sdk),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", targetFramework)));

        }

        private static void CreateProject(string projectName, string folder = default)
        {
            var proj = CreateCsProj("Microsoft.NET.Sdk", "net6.0");
            var slnDirectory = Path.GetDirectoryName(slnPath);
            
            var fileDirectory = Path.Combine(slnDirectory, projectName);
            var fileFullPath = Path.Combine(fileDirectory, $"{projectName}.csproj");
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);
            File.WriteAllText(fileFullPath, proj.ToString());
        }

        private static void AddDirectBuildProps()
        {
            var props =  new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement("Version", "0.1.0"),
                    new XElement("AssemblyVersion", "0.1.0"),
                    new XElement("FileVersion", "0.1.0"),
                    new XElement("Authors", "Innostage"),
                    new XElement("Company", "Innostage"),
                    new XElement("Product", "Innostage")),
                new XElement("PropertyGroup", new XAttribute("Label", "BasePath"),
                    new XElement("SolutionDir", new XAttribute("Condition", "'$(SolutionDir)'==''"), "$(MSBuildThisFileDirectory)")));
            var directBuildPropsPath = Path.Combine(slnDirectory, "Directory.Build.props");
            using(var strWriter = new Utf8StringWriter())
            {
                props.Save(strWriter);
                File.WriteAllText(directBuildPropsPath, strWriter.ToString());
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }
}