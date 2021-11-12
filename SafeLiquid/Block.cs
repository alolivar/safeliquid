
// Type: SafeLiquid.Block




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SafeLiquid
{
    public class Block : Tag
    {
        private static readonly Regex IsTag = R.B("^{0}", Liquid.TagStart);
        private static readonly Regex IsVariable = R.B("^{0}", Liquid.VariableStart);
        private static readonly Regex ContentOfVariable = R.B("^{0}(.*){1}$", Liquid.VariableStart, Liquid.VariableEnd);
        internal static readonly Regex FullToken = R.B("^{0}\\s*(\\w+)\\s*(.*)?{1}$", Liquid.TagStart, Liquid.TagEnd);

        public Block(Template template) : base(template) { }


        protected override void Parse(List<string> tokens)
        {
            this.NodeList = this.NodeList ?? new List<object>();
            this.NodeList.Clear();
            string str;
            while ((str = tokens.Shift<string>()) != null)
            {
                if (Block.IsTag.Match(str).Success)
                {
                    Match match = Block.FullToken.Match(str);
                    if (match.Success)
                    {
                        if (this.BlockDelimiter == match.Groups[1].Value)
                        {
                            this.EndTag();
                            return;
                        }
                        Tag tag;
                        if ((tag = Template.CreateTag(match.Groups[1].Value)) != null)
                        {
                            tag.Initialize(match.Groups[1].Value, match.Groups[2].Value, tokens);
                            this.NodeList.Add((object)tag);
                            tag.AssertTagRulesViolation(this.NodeList);
                        }
                        else
                        {
                            this.UnknownTag(match.Groups[1].Value, match.Groups[2].Value, tokens);
                        }
                    }
                    else
                    {
                        throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNotTerminatedException"), new string[2]
            {
              str,
              Liquid.TagEnd
            });
                    }
                }
                else if (Block.IsVariable.Match(str).Success)
                {
                    this.NodeList.Add(this.CreateVariable(str));
                }
                else if (!(string.IsNullOrEmpty(str)))
                {
                    this.NodeList.Add((object)str);
                }
            }
            this.AssertMissingDelimitation();
        }

        public virtual void EndTag()
        {
        }

        public virtual void UnknownTag(string tag, string markup, List<string> tokens)
        {
            if (!(tag == "else"))
            {
                if (tag == "end")
                {
                    throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNoEndException"), new string[2]
          {
            this.BlockName,
            this.BlockDelimiter
          });
                }

                throw new SyntaxException(Liquid.ResourceManager.GetString("BlockUnknownTagException"), new string[1]
        {
          tag
        });
            }
            throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNoElseException"), new string[1]
            {
        this.BlockName
            });
        }

        protected virtual string BlockDelimiter => string.Format("end{0}", (object)this.BlockName);

        private string BlockName => this.TagName;

        public Variable CreateVariable(string token)
        {
            Match match = Block.ContentOfVariable.Match(token);
            if (match.Success)
                return new Variable(Template, match.Groups[1].Value);
            throw new SyntaxException(Liquid.ResourceManager.GetString("BlockVariableNotTerminatedException"), new string[2]
            {
        token,
        Liquid.VariableEnd
            });
        }

        public override void Render(Context context, TextWriter result) => this.RenderAll(this.NodeList, context, result);

        protected virtual void AssertMissingDelimitation() => throw new SyntaxException(Liquid.ResourceManager.GetString("BlockTagNotClosedException"), new string[1]
        {
      this.BlockName
        });

        protected void RenderAll(List<object> list, Context context, TextWriter result)
        {
            foreach (object obj in list)
            {
                context.CheckTimeout();
                try
                {
                    if (obj is IRenderable renderable4)
                        renderable4.Render(context, result);
                    else
                        result.Write(obj.ToString());
                }
                catch (Exception ex1)
                {
                    Exception ex2 = ex1;
                    if (ex2.InnerException is LiquidException)
                        ex2 = ex2.InnerException;
                    result.Write(context.HandleError(ex2));
                }
            }
        }
    }
}
