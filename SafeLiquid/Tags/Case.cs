
// Type: SafeLiquid.Tags.Case




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Case : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.B("({0})", Liquid.QuotedFragment);
        private static readonly Regex WhenSyntax = R.B("({0})(?:(?:\\s+or\\s+|\\s*\\,\\s*)({0}.*))?", Liquid.QuotedFragment);
        private List<Condition> _blocks;
        private string _left;


        public Case(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            this._blocks = new List<Condition>();
            Match match = Case.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagSyntaxException"), new string[0]);
            this._left = match.Groups[1].Value;
            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            this.NodeList = new List<object>();
            if (!(tag == "when"))
            {
                if (tag == "else")
                    this.RecordElseCondition(markup);
                else
                    base.UnknownTag(tag, markup, tokens);
            }
            else
            {
                this.RecordWhenCondition(markup);
            }
        }

        public override void Render(Context context, TextWriter result) => context.Stack((Action)(() =>
       {
           bool executeElseBlock = true;
           this._blocks.ForEach((Action<Condition>)(block =>
       {
             if (block.IsElse)
             {
                 if (!executeElseBlock)
                     return;
                 this.RenderAll(block.Attachment, context, result);
             }
             else
             {
                 if (!block.Evaluate(context, result.FormatProvider))
                     return;
                 executeElseBlock = false;
                 this.RenderAll(block.Attachment, context, result);
             }
         }));
       }));

        private void RecordWhenCondition(string markup)
        {
            while (markup != null)
            {
                Match match = Case.WhenSyntax.Match(markup);
                if (!match.Success)
                    throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagWhenSyntaxException"), new string[0]);
                markup = match.Groups[2].Value;
                if (string.IsNullOrEmpty(markup))
                    markup = (string)null;
                Condition condition = new Condition(Template, this._left, "==", match.Groups[1].Value);
                condition.Attach(this.NodeList);
                this._blocks.Add(condition);
            }
        }

        private void RecordElseCondition(string markup)
        {
            if (!string.IsNullOrEmpty(markup.Trim()))
                throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagElseSyntaxException"), new string[0]);
            ElseCondition elseCondition = new ElseCondition(Template);
            elseCondition.Attach(this.NodeList);
            this._blocks.Add((Condition)elseCondition);
        }
    }
}
