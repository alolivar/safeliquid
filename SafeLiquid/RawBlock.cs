using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SafeLiquid
{
    public class RawBlock : Block
    {
        protected override sealed string BlockDelimiter => base.BlockDelimiter;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            if (!markup.IsNullOrWhiteSpace())
                throw new SyntaxException(string.Format(Liquid.ResourceManager.GetString("SimpleTagSyntaxException"), (object)tagName), new string[0]);
            base.Initialize(tagName, markup, tokens);
        }


        public RawBlock(Template template) : base(template) { }
        protected override void Parse(List<string> tokens)
        {
            this.NodeList = this.NodeList ?? new List<object>();
            this.NodeList.Clear();
            string input;
            while ((input = tokens.Shift<string>()) != null)
            {
                Match match = Block.FullToken.Match(input);
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
