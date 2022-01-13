
using SafeLiquid.Exceptions;
using System.Collections.Generic;
using System.IO;

namespace SafeLiquid
{
    public class Document : Block
    {
        public override void Initialize(string tagName, string markup, List<string> tokens) => this.Parse(tokens);

        protected override string BlockDelimiter => string.Empty;

        public Document(Template template):base(template){ }

        protected override void AssertMissingDelimitation()
        {
        }

        public override void Render(Context context, TextWriter result)
        {
            try
            {
                base.Render(context, result);
            }
            catch (BreakInterrupt ex)
            {
            }
            catch (ContinueInterrupt ex)
            {
            }
        }
    }
}
