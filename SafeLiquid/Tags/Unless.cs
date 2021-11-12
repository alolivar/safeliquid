
using System;
using System.IO;
using System.Linq;

namespace SafeLiquid.Tags
{
    public class Unless : If
    {

        public Unless(Template template) : base(template) { }

        public override void Render(Context context, TextWriter result) => context.Stack((Action)(() =>
       {
           Condition condition1 = this.Blocks.First<Condition>();
           if (!condition1.Evaluate(context, result.FormatProvider))
           {
               this.RenderAll(condition1.Attachment, context, result);
           }
           else
           {
               foreach (Condition condition2 in this.Blocks.Skip<Condition>(1))
               {
                   if (condition2.Evaluate(context, result.FormatProvider))
                   {
                       this.RenderAll(condition2.Attachment, context, result);
                       break;
                   }
               }
           }
       }));
    }
}
