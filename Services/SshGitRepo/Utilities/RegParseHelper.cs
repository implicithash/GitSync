using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EW.Navigator.SCM.GitRepo.Sync.Utilities
{
    public static class RegParseHelper
    {
        public static IDictionary<string, string> Parse(string s, string pattern)
        {
            var dic = new Dictionary<string, string>();
            var regex = new Regex(pattern);
            if (!regex.IsMatch(s)) return dic;
            var groups = regex.Match(s).Groups;

            foreach (var groupName in regex.GetGroupNames())
            {
                dic.Add(groupName, groups[groupName].Value);
            }
            return dic;
        }
    }
}
