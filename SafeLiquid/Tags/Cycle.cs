

using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Cycle : Tag
    {
        private static readonly Regex SimpleSyntax = R.B(R.Q("^{0}+"), Liquid.QuotedFragment);
        private static readonly Regex NamedSyntax = R.B(R.Q("^({0})\\s*\\:\\s*(.*)"), Liquid.QuotedFragment);
        private static readonly Regex QuotedFragmentRegex = R.B(R.Q("\\s*({0})\\s*"), Liquid.QuotedFragment);
        private string[] _variables;
        private string _name;

        public Cycle(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Cycle.NamedSyntax.Match(markup);
            if (match.Success)
            {
                this._variables = Cycle.VariablesFromString(match.Groups[2].Value);
                this._name = match.Groups[1].Value;
            }
            else
            {
                if (!Cycle.SimpleSyntax.Match(markup).Success)
                    throw new SyntaxException(Liquid.ResourceManager.GetString("CycleTagSyntaxException"), new string[0]);
                this._variables = Cycle.VariablesFromString(markup);
                this._name = "'" + string.Join(string.Empty, this._variables) + "'";
            }
            base.Initialize(tagName, markup, tokens);
        }

        private static string[] VariablesFromString(string markup) => ((IEnumerable<string>)markup.Split(',')).Select<string, string>((Func<string, string>)(var =>
      {
          Match match = Cycle.QuotedFragmentRegex.Match(var);
          return !match.Success || string.IsNullOrEmpty(match.Groups[1].Value) ? (string)null : match.Groups[1].Value;
      })).ToArray<string>();

        public override void Render(Context context, TextWriter result) => context.Stack((Action)(() =>
       {
           string key = context[this._name].ToString();
           IDictionary<string, int> register = Tag.GetRegister<int>(context, "cycle");
           int index = register.ContainsKey(key) ? register[key] : 0;
           result.Write(context[this._variables[index]].ToString());
           int num = index + 1;
           if (num >= this._variables.Length)
               num = 0;
           register[key] = num;
       }));
    }
}
