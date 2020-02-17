using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace EW.Navigator.SCM.GitRepo.Sync.Extensions
{
    public static class RepositoryExtensions
    {
        public static IEnumerable<Commit> GetAllCommits(this Repository repository)
        {
            var commits = repository.Commits.QueryBy(new CommitFilter() { IncludeReachableFrom = repository.Refs });
            return commits;
        }
        public static IEnumerable<Branch> GetRemoteBranches(this Repository repository) => repository.Branches.GetRemoteBranches();

        public static IEnumerable<Branch> GetLocalBranches(this Repository repository) => repository.Branches.GetLocalBranches();

        public static Branch GetLocalBranch(this Repository repository, string branchName)
        {
            return repository.GetLocalBranches().FirstOrDefault(b => b.FriendlyName.Equals(branchName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static Branch GetLocalBranch(this Repository repository, BranchName branchName)
        {
            return repository.GetLocalBranches().FirstOrDefault(b => b.FriendlyName.Equals(branchName.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool LocalBranchExists(this Repository repository, string branchName)
        {
            return repository.GetLocalBranches().Any(b => b.FriendlyName.Equals(branchName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool LocalBranchExists(this Repository repository, BranchName branchName) => repository.LocalBranchExists(branchName.ToString());

        public static Branch CreateBranch(this Repository repository, BranchName branchName, Commit commit) => repository.CreateBranch(branchName.ToString(), commit);

        public static Branch GetBranch(this Repository repository, BranchName branchName) => repository.Branches.FirstOrDefault(b => BranchName.Parse(b.FriendlyName).Equals(branchName));

        public static bool IsCommitAncestor(this Repository repository, string ancestorId, string descendantId)
        {
            var ancestor = repository.Lookup<Commit>(ancestorId);
            var descendant = repository.Lookup<Commit>(descendantId);

            var mergeBase = repository.ObjectDatabase.FindMergeBase(ancestor, descendant);

            return mergeBase != null && mergeBase.Sha == ancestor.Sha;
        }

        public static void FetchOrigin(this Repository repository, FetchOptions options = null)
        {
            var remote = repository.Network.Remotes["origin"];
            Commands.Fetch(
                repository,
                "origin",
                remote.FetchRefSpecs.Select(x => x.Specification),
                options,
                "");

        }
    }
}

