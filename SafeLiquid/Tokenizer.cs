
// Type: SafeLiquid.Tokenizer




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SafeLiquid
{
    public class Tokenizer
    {

        public static readonly TimeSpan RegexTimeOut = TimeSpan.FromSeconds(10.0);


        private static readonly HashSet<char> SearchSingleQuoteEnd = new HashSet<char>()
    {
      '\''
    };
        private static readonly HashSet<char> SearchDoubleQuoteEnd = new HashSet<char>()
    {
      '"'
    };
        private static readonly HashSet<char> SearchQuoteOrVariableEnd = new HashSet<char>()
    {
      '}',
      '\'',
      '"'
    };
        private static readonly HashSet<char> SearchQuoteOrTagEnd = new HashSet<char>()
    {
      '%',
      '\'',
      '"'
    };
        private static readonly Regex LiquidAnyStartingTagRegex = R.B(R.Q("({0})([-])?"), Liquid.AnyStartingTag);
        private static readonly Regex TagNameRegex = R.B(R.Q("{0}\\s*(\\w+)"), Liquid.AnyStartingTag);
        private static readonly ConcurrentDictionary<string, Regex> EndTagRegexes = new ConcurrentDictionary<string, Regex>();


        public Template Template { get; set; }

        public Tokenizer(Template template)
        {
            this.Template = template;
        }

        internal List<string> Tokenize(string source)
        {
            if (string.IsNullOrEmpty(source))
                return new List<string>();
            source = Regex.Replace(source, string.Format("-({0}|{1})(\\n|\\r\\n|[ \\t]+)?", (object)Liquid.VariableEnd, (object)Liquid.TagEnd), "$1", RegexOptions.None, RegexTimeOut);
            List<string> stringList = new List<string>();
            using (SafeLiquid.Util.CharEnumerator markupEnumerator = new SafeLiquid.Util.CharEnumerator(source))
            {
                for (Match match1 = Tokenizer.LiquidAnyStartingTagRegex.Match(source, markupEnumerator.Position); match1.Success; match1 = Tokenizer.LiquidAnyStartingTagRegex.Match(source, markupEnumerator.Position))
                {
                    if (match1.Index > markupEnumerator.Position)
                    {
                        string str = Tokenizer.ReadChars(markupEnumerator, match1.Index - markupEnumerator.Position);
                        if (match1.Groups[2].Success)
                            str = str.TrimEnd('\t', ' ');
                        if (!string.IsNullOrEmpty(str))
                            stringList.Add(str);
                    }
                    bool flag = match1.Groups[1].Value == "{%";
                    StringBuilder sb = new StringBuilder(markupEnumerator.Remaining);
                    sb.Append(match1.Groups[1].Value);
                    Tokenizer.ReadChars(markupEnumerator, match1.Length);
                    Tokenizer.ReadToEnd(sb, markupEnumerator, flag ? Tokenizer.SearchQuoteOrTagEnd : Tokenizer.SearchQuoteOrVariableEnd);
                    string input = sb.ToString();
                    stringList.Add(input);
                    if (flag)
                    {
                        Match match2 = Tokenizer.TagNameRegex.Match(input);
                        if (match2.Success)
                        {
                            string str = match2.Groups[1].Value;
                            if (Template.IsRawTag(str))
                            {
                                Match match3 = Tokenizer.EndTagRegexes.GetOrAdd(str, (Func<string, Regex>)(key => R.B("{0}-?\\s*end{1}\\s*-?{2}", Liquid.TagStart, key, Liquid.TagEnd))).Match(source, markupEnumerator.Position);
                                if (!match3.Success)
                                {
                                    throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNotClosedException"), new string[1]
                                    {
                    str
                                    });
                                }

                                if (match3.Index > markupEnumerator.Position)
                                    stringList.Add(Tokenizer.ReadChars(markupEnumerator, match3.Index - markupEnumerator.Position));
                            }
                        }
                    }
                }
                if (markupEnumerator.Remaining > 0)
                    stringList.Add(Tokenizer.ReadChars(markupEnumerator, markupEnumerator.Remaining));
            }
            return stringList;
        }

        private static string ReadChars(SafeLiquid.Util.CharEnumerator markupEnumerator, int markupLength)
        {
            StringBuilder sb = new StringBuilder(markupLength);
            for (int index = 0; index < markupLength; ++index)
                markupEnumerator.AppendNext(sb);
            return sb.ToString();
        }

        private static void ReadToEnd(
          StringBuilder sb,
          SafeLiquid.Util.CharEnumerator markupEnumerator,
          HashSet<char> initialSearchChars)
        {
            HashSet<char> charSet = initialSearchChars;
            bool flag = false;
            while (markupEnumerator.AppendNext(sb))
            {
                char current = markupEnumerator.Current;
                if (charSet.Contains(current))
                {
                    switch (current)
                    {
                        case '"':
                            flag = !flag;
                            charSet = flag ? Tokenizer.SearchDoubleQuoteEnd : initialSearchChars;
                            continue;
                        case '%':
                        case '}':
                            if (markupEnumerator.Remaining > 0 && markupEnumerator.Next == '}')
                            {
                                markupEnumerator.AppendNext(sb);
                                return;
                            }
                            continue;
                        case '\'':
                            flag = !flag;
                            charSet = flag ? Tokenizer.SearchSingleQuoteEnd : initialSearchChars;
                            continue;
                        default:
                            continue;
                    }
                }
            }
            if (markupEnumerator.Remaining != 0)
                return;
            if (initialSearchChars == Tokenizer.SearchQuoteOrTagEnd)
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNotTerminatedException"), new string[2]
                {
          sb.ToString(),
          Liquid.TagEnd
                });
            }

            throw new SyntaxException(Liquid.ResourceManager.GetString("BlockVariableNotTerminatedException"), new string[2]
            {
        sb.ToString(),
        Liquid.VariableEnd
            });
        }
    }
}
