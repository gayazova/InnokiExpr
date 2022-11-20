using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class SlnProjectCollectionExtensions
    {
        public static IEnumerable<SlnProject> GetProjectsByType(
            this SlnProjectCollection projects,
            string typeGuid)
        {
            return projects.Where(p => p.TypeGuid == typeGuid);
        }

        public static IEnumerable<SlnProject> GetProjectsNotOfType(
            this SlnProjectCollection projects,
            string typeGuid)
        {
            return projects.Where(p => p.TypeGuid != typeGuid);
        }
    }
}
