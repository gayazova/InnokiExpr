using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class ProjectInstanceExtensions
    {
        public static string GetProjectId(this ProjectInstance projectInstance)
        {
            var projectGuidProperty = projectInstance.GetPropertyValue("ProjectGuid");
            var projectGuid = string.IsNullOrEmpty(projectGuidProperty)
                ? Guid.NewGuid()
                : new Guid(projectGuidProperty);
            return projectGuid.ToString("B").ToUpper();
        }

        public static string GetDefaultProjectTypeGuid(this ProjectInstance projectInstance)
        {
            return projectInstance.GetPropertyValue("DefaultProjectTypeGuid");
        }

        public static IEnumerable<string> GetPlatforms(this ProjectInstance projectInstance)
        {
            return (projectInstance.GetPropertyValue("Platforms") ?? "")
                .Split(
                    new char[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .DefaultIfEmpty("AnyCPU");
        }

        public static IEnumerable<string> GetConfigurations(this ProjectInstance projectInstance)
        {
            return (projectInstance.GetPropertyValue("Configurations") ?? "Debug;Release")
                .Split(
                    new char[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .DefaultIfEmpty("Debug");
        }
    }
}
