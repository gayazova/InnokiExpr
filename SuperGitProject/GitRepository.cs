using LibGit2Sharp;
using System;

namespace SuperGitProject
{
    public static class GitRepository
    {
        private static string Path = @"D:\\source\\innokitgit";
        public static void Pull()
        {
            using (var repo = new Repository(Path))
            {
                var pullOptions = new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials
                            {
                                Username = "Aliya.Gayazova",
                                Password = "WangYibo05"
                            }
                    }
                };
                var signature = new Signature(
                    new Identity("Aliya.Gayazova", "Aliya.Gayazova@innostage-group.ru"), DateTimeOffset.Now);
                Commands.Pull(repo, signature, pullOptions);
            }
        }

        public static void Commit(string commitText)
        {
            using (var repo = new Repository(Path))
            {
                var sign = new Signature(
                    new Identity("Aliya.Gayazova", "Aliya.Gayazova@innostage-group.ru"), DateTimeOffset.Now);
                Commands.Stage(repo, "*");
                var commit = repo.Commit(commitText, sign, sign);
            }
        }

        #region push
        public static void Push(string branch = "develop")
        {
            using (var repo = new Repository(Path))
            {
                var remote = repo.Network.Remotes["origin"];

                var pushRefSpec = @"refs/heads/" + branch;
                var pushOptions = new PushOptions
                {
                    CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                    {
                        Username = "Aliya.Gayazova",
                        Password = "WangYibo05"
                    }
                };

                repo.Network.Push(remote, pushRefSpec, pushOptions);
            }
        }
        #endregion

        public static void Reset(ResetMode resetMode = ResetMode.Hard)
        {
            using (var repo = new Repository(Path))
            {
                repo.Reset(resetMode);
            }
        }

        public static void SwitchBranch(string branchName)
        {
            Pull();
            using (var repo = new Repository(Path))
            {

                var branch = repo.Branches[$"{branchName}"];
                var origin = repo.Branches[$"origin/{branchName}"];

                if (branch == null)
                {
                    if (origin == null)
                        throw new Exception("branch not exists");
                    branch = repo.Branches.Add(branchName, "HEAD");
                }

                if (!branch.IsTracking)
                    branch = repo.Branches.Update(branch, x => x.TrackedBranch = origin.CanonicalName);

                Commands.Checkout(repo, branch);
            }
        }

        public static void CreateBranch(string branchName, string sourceBranchName = null)
        {
            using var repo = new Repository(Path);
            var remote = repo.Network.Remotes["origin"];
            if (!string.IsNullOrEmpty(sourceBranchName))
            {
                var sourceBranch = repo.Branches[$"{sourceBranchName}"];
                var sourceOrigin = repo.Branches[$"origin/{sourceBranchName}"];

                if (sourceBranch == null)
                {
                    if (sourceOrigin == null)
                        throw new Exception($"Branch {sourceBranchName} not exists");
                    sourceBranch = repo.Branches.Add(sourceBranchName, "HEAD");
                }

                if (!sourceBranch.IsTracking && sourceOrigin != null)
                    sourceBranch = repo.Branches.Update(sourceBranch, x => x.TrackedBranch = sourceOrigin.CanonicalName);
                if (!sourceBranch.IsTracking && sourceOrigin == null)
                    sourceBranch = repo.Branches.Update(sourceBranch,
                                                        x => x.Remote = remote.Name,
                                                        x => x.UpstreamBranch = sourceBranch.CanonicalName,
                                                        x => x.TrackedBranch = @"refs/remotes/origin/" + sourceBranchName);

                Commands.Checkout(repo, sourceBranch);
            }
            var newBranch = repo.CreateBranch(branchName);

            var remoteBranch = repo.Network.Remotes["origin"];

            var pushRefSpec = @"refs/heads/" + branchName;
            var pushOptions = new PushOptions
            {
                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                {
                    Username = "Aliya.Gayazova",
                    Password = "WangYibo05"
                }
            };

            repo.Network.Push(remote, pushRefSpec, pushOptions);

            var remoooteBranch = repo.Branches[$"origin/{branchName}"];
            if (!newBranch.IsTracking)
                newBranch = repo.Branches.Update(newBranch,
                                                x => x.TrackedBranch = remoooteBranch.CanonicalName);
            Commands.Checkout(repo, newBranch);
        }

    }
}
