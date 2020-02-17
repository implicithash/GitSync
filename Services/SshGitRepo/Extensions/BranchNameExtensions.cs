using System;

namespace EW.Navigator.SCM.GitRepo.Sync.Extensions
{
    public static class BranchNameExtensions
    {
        public static bool HasPrefix(this BranchName branchName, string prefix) => StringComparer.InvariantCultureIgnoreCase.Equals(branchName?.Prefix, prefix);
    }
}
