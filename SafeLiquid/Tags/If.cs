using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class If : SafeLiquid.Block
    {
        private string SyntaxHelp = Liquid.ResourceManager.GetString("IfTagSyntaxException");
        private string TooMuchConditionsHelp = Liquid.ResourceManager.GetString("IfTagTooMuchConditionsException");
        private static readonly Regex Syntax = R.B(R.Q("({0})\\s*([=!<>a-zA-Z_]+)?\\s*({0})?"), Liquid.QuotedFragment);
        private static readonly string ExpressionsAndOperators = string.Format(R.Q("(?:\\b(?:\\s?and\\s?|\\s?or\\s?)\\b|(?:\\s*(?!\\b(?:\\s?and\\s?|\\s?or\\s?)\\b)(?:{0}|\\S+)\\s*)+)"), (object)Liquid.QuotedFragment);
        private static readonly Regex ExpressionsAndOperatorsRegex = R.C(If.ExpressionsAndOperators);

        protected List<Condition> Blocks { get; private set; }

        public If(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            this.Blocks = new List<Condition>();
            this.PushBlock("if", markup);
            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            if (tag == "elsif" || tag == "elseif" || tag == "else")
                this.PushBlock(tag, markup);
            else
                base.UnknownTag(tag, markup, tokens);
        }

        public override void Render(Context context, TextWriter result) => context.Stack((Action)(() =>
       {
           foreach (Condition block in this.Blocks)
           {
               if (block.Evaluate(context, result.FormatProvider))
               {
                   this.RenderAll(block.Attachment, context, result);
                   break;
               }
           }
       }));

        private void PushBlock(string tag, string markup)
        {
            Condition condition1;
            if (tag == "else")
            {
                condition1 = (Condition)new ElseCondition(Template);
            }
            else
            {
                List<string> list = R.Scan(markup, If.ExpressionsAndOperatorsRegex);
                string atIndexReverse = list.TryGetAtIndexReverse<string>(0);
                Match match1 = !string.IsNullOrEmpty(atIndexReverse) ? If.Syntax.Match(atIndexReverse) : throw new SyntaxException(this.SyntaxHelp, new string[0]);
                if (!match1.Success)
                    throw new SyntaxException(this.SyntaxHelp, new string[0]);
                Condition condition2 = new Condition(Template, match1.Groups[1].Value, match1.Groups[2].Value, match1.Groups[3].Value);
                int num = 1;
                for (int rindex = 1; rindex < list.Count; rindex += 2)
                {
                    string str = list.TryGetAtIndexReverse<string>(rindex).Trim();
                    Match match2 = If.Syntax.Match(list.TryGetAtIndexReverse<string>(rindex + 1));
                    if (!match2.Success)
                        throw new SyntaxException(this.SyntaxHelp, new string[0]);
                    if (++num > 500)
                        throw new SyntaxException(this.TooMuchConditionsHelp, new string[0]);
                    Condition condition3 = new Condition(Template, match2.Groups[1].Value, match2.Groups[2].Value, match2.Groups[3].Value);
                    if (!(str == "and"))
                    {
                        if (str == "or")
                            condition3.Or(condition2);
                    }
                    else
                    {
                        condition3.And(condition2);
                    }

                    condition2 = condition3;
                }
                condition1 = condition2;
            }
            this.Blocks.Add(condition1);
            this.NodeList = condition1.Attach(new List<object>());
        }
    }
}
