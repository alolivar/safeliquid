
// Type: SafeLiquid.Util.R




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Util
{
    public static class R
    {

        public static readonly TimeSpan RegexTimeOut = TimeSpan.FromSeconds(10.0);

        public static string Q(string regex) => string.Format("(?-mix:{0})", (object)regex);

        public static Regex B(string format, params string[] args) => R.C(string.Format(format, (object[])args));

        public static Regex C(string pattern, RegexOptions options = RegexOptions.Compiled)
        {
            Regex regex = new Regex(pattern, options, RegexTimeOut);
            regex.IsMatch(string.Empty);
            return regex;
        }

        public static List<string> Scan(string input, Regex regex) => regex.Matches(input).Cast<Match>().Select<Match, string>((Func<Match, string>)(m => m.Groups.Count != 2 ? m.Value : m.Groups[1].Value)).ToList<string>();

        [Obsolete("Use Scan(string, Regex) instead.")]
        public static List<string> Scan(string input, string pattern) => R.Scan(input, new Regex(pattern, RegexOptions.None, RegexTimeOut));

        public static void Scan(string input, string pattern, Action<string, string> callback)
        {
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.None, RegexTimeOut))
                callback(match.Groups[1].Value, match.Groups[2].Value);
        }
    }
}
