using Microsoft.Build.Construction;
using System;
using System.Linq;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class ProjectRootElementExtensions
    {
        public static string GetProjectTypeGuid(this ProjectRootElement rootElement)
        {
            return rootElement
                .Properties
                .FirstOrDefault(p => string.Equals(p.Name, "ProjectTypeGuids", StringComparison.OrdinalIgnoreCase))
                ?.Value
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .LastOrDefault(g => !string.IsNullOrWhiteSpace(g));
        }
    }
}
