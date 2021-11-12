using System;
using System.IO;

namespace SafeLiquid.Tags
{
    public class IfChanged : SafeLiquid.Block
    {

        public IfChanged(Template template) : base(template) { }

        public override void Render(Context context, TextWriter result) => context.Stack((Action)(() =>
       {
           string str;
           using (TextWriter result1 = (TextWriter)new StringWriter(result.FormatProvider))
           {
               this.RenderAll(this.NodeList, context, result1);
               str = result1.ToString();
           }
           if (!(str != context.Registers["ifchanged"] as string))
               return;
           context.Registers["ifchanged"] = (object)str;
           result.Write(str);
       }));
    }
}
