
// Type: SafeLiquid.ElseCondition




using System;

namespace SafeLiquid
{
    public class ElseCondition : Condition
    {
        public override bool IsElse => true;

        public override bool Evaluate(Context context, IFormatProvider formatProvider) => true;

        public ElseCondition(Template template) : base(template) { }

    }
}
