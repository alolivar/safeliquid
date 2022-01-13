
// Type: SafeLiquid.Tags.Literal




using SafeLiquid.Util;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Literal : SafeLiquid.Block
    {
        private static readonly Regex LiteralRegex = R.C(Liquid.LiteralShorthand);


        public Literal(Template template) : base(template)
        {

        }

        public static string FromShortHand(string theString)
        {
            if (theString == null)
                return theString;
            Match match = Literal.LiteralRegex.Match(theString);
            return !match.Success ? theString : string.Format("{{% literal %}}{0}{{% endliteral %}}", (object)match.Groups[1].Value);
        }

        protected override void Parse(List<string> tokens)
        {
            this.NodeList = this.NodeList ?? new List<object>();
            this.NodeList.Clear();
            string input;
            while ((input = tokens.Shift<string>()) != null)
            {
                Match match = SafeLiquid.Block.FullToken.Match(input);
                if (match.Success && this.BlockDelimiter == match.Groups[1].Value)
                {
                    this.EndTag();
                    return;
                }
                this.NodeList.Add((object)input);
            }
            this.AssertMissingDelimitation();
        }
    }
}
