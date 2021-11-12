
// Type: SafeLiquid.Tags.Continue




using SafeLiquid.Exceptions;
using System.IO;

namespace SafeLiquid.Tags
{
    public class Continue : Tag
    {
        public override void Render(Context context, TextWriter result) => throw new ContinueInterrupt();

        public Continue(Template template) : base(template) { }

    }
}
