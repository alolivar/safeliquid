
// Type: SafeLiquid.Tags.Break




using SafeLiquid.Exceptions;
using System.IO;

namespace SafeLiquid.Tags
{
    public class Break : Tag
    {

        public Break(Template template) : base(template) { }
        public override void Render(Context context, TextWriter result) => throw new BreakInterrupt();
    }
}
