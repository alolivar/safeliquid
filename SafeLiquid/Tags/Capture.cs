
// Type: SafeLiquid.Tags.Capture




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Capture : SafeLiquid.Block
    {


        public Capture(Template template) : base(template)
        {

        }
        private static readonly Regex Syntax = R.C("(\\w+)");
        private string _to;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Capture.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("CaptureTagSyntaxException"), new string[0]);
            this._to = match.Groups[1].Value;
            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            using (TextWriter result1 = (TextWriter)new StringWriter(result.FormatProvider))
            {
                base.Render(context, result1);
                context.Scopes.Last<Hash>()[this._to] = (object)result1.ToString();
            }
        }
    }
}
