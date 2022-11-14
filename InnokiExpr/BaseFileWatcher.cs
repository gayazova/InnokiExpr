using Mono.Unix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace InnokiExpr
{
    public class BaseFileWatcher : IDisposable
    {
        public BaseFileWatcher(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        protected FileSystemWatcher Watcher { get; set; }

        protected string DirectoryPath { get; set; }

        public void Dispose()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Changed -= OnChanged;
                Watcher.Created -= OnCreated;
                Watcher.Deleted -= OnDeleted;
                Watcher.Renamed -= OnRenamed;
                Watcher.Error -= OnError;
                Watcher.Dispose();
            }
        }

        public void ExecuteAsync(CancellationToken cancellationToken)
        {
            LogWatcherDbg("running...");
            try
            {
                LogWatcherDbg("doing work...");
                Execute(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Background task {GetType()} error: {ex}");
            }
        }

        protected void Execute(CancellationToken cancellationToken)
        {
            CheckDirectory();

            Watcher = new FileSystemWatcher(DirectoryPath)
            {
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime
                                             | NotifyFilters.DirectoryName | NotifyFilters.FileName
                                             | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                             | NotifyFilters.Security | NotifyFilters.Size
            };
            Watcher.Changed += OnChanged;
            Watcher.Created += OnCreated;
            Watcher.Deleted += OnDeleted;
            Watcher.Renamed += OnRenamed;
            Watcher.Error += OnError;

            Watcher.IncludeSubdirectories = false;
            Watcher.EnableRaisingEvents = true;

            LogWatcherDbg($"File monitoring for directory: '{DirectoryPath}' is configured.");
        }

        protected virtual void OnChanged(object sender, FileSystemEventArgs eventArgs)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Changed) return;
            LogWatcherDbg($"Changed: {eventArgs.FullPath}");
        }

        protected virtual void OnCreated(object sender, FileSystemEventArgs e)
        {
            LogWatcherDbg($"Created: {e.FullPath}");
        }

        protected virtual void OnDeleted(object sender, FileSystemEventArgs e)
        {
            LogWatcherDbg($"Deleted: {e.FullPath}");
        }

        protected virtual void OnRenamed(object sender, RenamedEventArgs e)
        {
            var message = $"Renamed: Old: {e.OldFullPath} | New: {e.FullPath}";
            LogWatcherDbg(message);
        }

        protected virtual void OnError(object sender, ErrorEventArgs e) =>
            Console.WriteLine($"Error: {e.GetException()}");

        protected void LogWatcherDbg(string info) => Console.WriteLine($"Background task {GetType()}: {info}");

        protected void CheckDirectory()
        {
            Console.WriteLine("Checking directory...");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    var errorMessage = $"Directory {DirectoryPath} doesn't exist";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }

                if (!CheckDirectoryPermissionsForWindows(DirectoryPath, FileSystemRights.Read))
                {
                    var errorMessage = $"Permission for directory '{DirectoryPath}' is denied";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Checking Linux..");
                if (!CheckDirectoryPermissionsForLinux(DirectoryPath))
                {
                    var errorMessage = $"Permission for directory '{DirectoryPath}' is denied";
                    Console.WriteLine(errorMessage);
                    throw new Exception(errorMessage);
                }
            }
        }

        protected bool CheckDirectoryPermissionsForWindows(string directoryPath, FileSystemRights accessType)
        {
            try
            {
                var security = new FileSecurity(directoryPath, AccessControlSections.Owner | AccessControlSections.Group | AccessControlSections.Access);
                var authorizationRules = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (FileSystemAccessRule rule in authorizationRules)
                {
                    if (rule.FileSystemRights == FileSystemRights.FullControl
                        && (accessType.Equals(FileSystemRights.Read) || accessType.Equals(FileSystemRights.Write)
                            || accessType.Equals(FileSystemRights.Delete)))
                        return true;
                    if (rule.FileSystemRights == FileSystemRights.Modify
                        && (accessType.Equals(FileSystemRights.Read) || accessType.Equals(FileSystemRights.Write)
                            || accessType.Equals(FileSystemRights.Delete)))
                        return true;

                    if ((rule.FileSystemRights & accessType) > 0)
                        return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        protected bool CheckDirectoryPermissionsForLinux(string directoryPath)
        {
            try
            {
                try
                {
                    Console.WriteLine("Check directoryyy");
                    if (!Directory.Exists(DirectoryPath))
                    {
                        var errorMessage = $"Directory {DirectoryPath} doesn't exist";
                        Console.WriteLine(errorMessage);
                        throw new Exception(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.WriteLine("Check permissions....");
                var unixFileInfo = new UnixFileInfo(directoryPath);

                if (unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherRead)
                    || unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.OtherReadWriteExecute))
                {
                    Console.WriteLine("OtherRead or OtherReadWriteExecute");
                    return true;
                }

                var currentUserName = Environment.UserName;
                if (string.Equals(currentUserName, "root"))
                {
                    Console.WriteLine("root");
                    return true;
                }

                var fileUserOwner = unixFileInfo.OwnerUser;

                if (string.Equals(fileUserOwner.UserName, currentUserName)
                    && (unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserRead)
                    || unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.UserReadWriteExecute)))
                {
                    Console.WriteLine("root");
                    return true;
                }

                var fileGroup = unixFileInfo.OwnerGroup;
                var userGroup = new UnixUserInfo(Environment.UserName).Group;

                var userBelongsToFileGroupOwner = fileGroup.GetMembers()?.Select(x => x.UserName).Contains(currentUserName) ?? false;

                if ((string.Equals(fileGroup.GroupName, userGroup.GroupName) || userBelongsToFileGroupOwner)
                    && (unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupRead)
                    || unixFileInfo.FileAccessPermissions.HasFlag(FileAccessPermissions.GroupReadWriteExecute)))
                {
                    Console.WriteLine("Group...");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }
    }
}
