using Microsoft.DotNet.Cli.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMSBuildRoslyn.Sdk
{
    public static class SlnFileFactory
    {
        public static SlnFile CreateFromFileOrDirectory(string fileOrDirectory)
        {
            if (File.Exists(fileOrDirectory))
            {
                return FromFile(fileOrDirectory);
            }
            else
            {
                return FromDirectory(fileOrDirectory);
            }
        }

        private static SlnFile FromFile(string solutionPath)
        {
            SlnFile slnFile = null;
            try
            {
                slnFile = SlnFile.Read(solutionPath);
            }
            catch (InvalidSolutionFormatException e)
            {
                throw new GracefulException(
                    CommonLocalizableStrings.InvalidSolutionFormatString,
                    solutionPath,
                    e.Message);
            }
            return slnFile;
        }

        private static SlnFile FromDirectory(string solutionDirectory)
        {
            DirectoryInfo dir;
            try
            {
                dir = new DirectoryInfo(solutionDirectory);
                if (!dir.Exists)
                {
                    throw new GracefulException(
                        CommonLocalizableStrings.CouldNotFindSolutionOrDirectory,
                        solutionDirectory);
                }
            }
            catch (ArgumentException)
            {
                throw new GracefulException(
                    CommonLocalizableStrings.CouldNotFindSolutionOrDirectory,
                    solutionDirectory);
            }

            FileInfo[] files = dir.GetFiles("*.sln");
            if (files.Length == 0)
            {
                throw new GracefulException(
                    CommonLocalizableStrings.CouldNotFindSolutionIn,
                    solutionDirectory);
            }

            if (files.Length > 1)
            {
                throw new GracefulException(
                    CommonLocalizableStrings.MoreThanOneSolutionInDirectory,
                    solutionDirectory);
            }

            FileInfo solutionFile = files.Single();
            if (!solutionFile.Exists)
            {
                throw new GracefulException(
                    CommonLocalizableStrings.CouldNotFindSolutionIn,
                    solutionDirectory);
            }

            return FromFile(solutionFile.FullName);
        }
    }
}
