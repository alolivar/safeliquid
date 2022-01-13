using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Assign : Tag
    {
        private static readonly Regex Syntax = R.B(R.Q("({0}+)\\s*=\\s*(.*)\\s*"), Liquid.VariableSignature);
        private string _to;
        private Variable _from;

        public Assign(Template template)
            : base(template)
        {

        }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Assign.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("AssignTagSyntaxException"), new string[0]);
            this._to = match.Groups[1].Value;
            this._from = new Variable(Template, match.Groups[2].Value);
            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result) => context.Scopes.Last<Hash>()[this._to] = this._from.Render(context);
    }
}
