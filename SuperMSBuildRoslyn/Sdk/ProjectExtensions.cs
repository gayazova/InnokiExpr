using Microsoft.Build.Evaluation;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class ProjectExtensions
    {
        public static IEnumerable<string> GetRuntimeIdentifiers(this Project project)
        {
            return project
                .GetPropertyCommaSeparatedValues("RuntimeIdentifier")
                .Concat(project.GetPropertyCommaSeparatedValues("RuntimeIdentifiers"))
                .Select(value => value.ToLower())
                .Distinct();
        }

        public static IEnumerable<NuGetFramework> GetTargetFrameworks(this Project project)
        {
            var targetFrameworksStrings = project
                    .GetPropertyCommaSeparatedValues("TargetFramework")
                    .Union(project.GetPropertyCommaSeparatedValues("TargetFrameworks"))
                    .Select((value) => value.ToLower());

            var uniqueTargetFrameworkStrings = new HashSet<string>(targetFrameworksStrings);

            return uniqueTargetFrameworkStrings
                .Select((frameworkString) => NuGetFramework.Parse(frameworkString));
        }

        public static IEnumerable<string> GetConfigurations(this Project project)
        {
            return project.GetPropertyCommaSeparatedValues("Configurations");
        }

        public static IEnumerable<string> GetPropertyCommaSeparatedValues(this Project project, string propertyName)
        {
            return project.GetPropertyValue(propertyName)
                .Split(';')
                .Select((value) => value.Trim())
                .Where((value) => !string.IsNullOrEmpty(value));
        }
    }
}
