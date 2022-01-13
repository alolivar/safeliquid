
// Type: SafeLiquid.Tags.For




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class For : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.B(R.Q("(\\w+)\\s+in\\s+({0}+)\\s*(reversed)?"), Liquid.QuotedFragment);
        private static string ForTagMaxIterationsExceededException = Liquid.ResourceManager.GetString("ForTagMaximumIterationsExceededException");
        private string _variableName;
        private string _collectionName;
        private string _name;
        private bool _reversed;
        private Dictionary<string, string> _attributes;

        private List<object> ForBlock { get; set; }

        private Condition ElseBlock { get; set; }


        public For(Template template) : base(template) { }
        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = For.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("ForTagSyntaxException"), new string[0]);
            this.NodeList = this.ForBlock = new List<object>();
            this._variableName = match.Groups[1].Value;
            this._collectionName = match.Groups[2].Value;
            this._name = string.Format("{0}-{1}", (object)this._variableName, (object)this._collectionName);
            this._reversed = !string.IsNullOrEmpty(match.Groups[3].Value);
            this._attributes = new Dictionary<string, string>((IEqualityComparer<string>)Template.NamingConvention.StringComparer);
            R.Scan(markup, Liquid.TagAttributes, (Action<string, string>)((key, value) => this._attributes[key] = value));
            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            if (tag == "else")
            {
                this.ElseBlock = (Condition)new ElseCondition(Template);
                this.NodeList = this.ElseBlock.Attach(new List<object>());
            }
            else
            {
                base.UnknownTag(tag, markup, tokens);
            }
        }

        public override void Render(Context context, TextWriter result)
        {
            if (!(context[this._collectionName] is IEnumerable collection))
            {
                if (this.ElseBlock == null)
                    return;
                context.Stack((Action)(() => this.RenderAll(this.ElseBlock.Attachment, context, result)));
            }
            else
            {
                IDictionary<string, object> register = Tag.GetRegister<object>(context, "for");
                int from = this._attributes.ContainsKey("offset") ? (this._attributes["offset"] == "continue" ? Convert.ToInt32(register[this._name]) : Convert.ToInt32(context[this._attributes["offset"]])) : 0;
                int? nullable = this._attributes.ContainsKey("limit") ? new int?(Convert.ToInt32(context[this._attributes["limit"]])) : new int?();
                int? to = nullable.HasValue ? new int?(nullable.Value + from) : new int?();
                List<object> segment = For.SliceCollectionUsingEach(context, collection, from, to);
                if (this._reversed)
                    segment.Reverse();
                int length = segment.Count;
                register[this._name] = (object)(from + length);
                context.Stack((Action)(() =>
               {
                   if (!segment.Any<object>())
                   {
                       if (this.ElseBlock == null)
                           return;
                       this.RenderAll(this.ElseBlock.Attachment, context, result);
                   }
                   else
                   {
                       for (int index = 0; index < segment.Count; ++index)
                       {
                           context.CheckTimeout();
                           object obj = segment[index];
                           if (obj is KeyValuePair<string, object> keyValuePair15)
                               this.BuildContext(context, this._variableName, keyValuePair15.Key, keyValuePair15.Value);
                           else
                               context[this._variableName] = obj;
                           context["forloop"] = (object)new Dictionary<string, object>()
                           {
                               ["name"] = (object)this._name,
                               ["length"] = (object)length,
                               ["index"] = (object)(index + 1),
                               ["index0"] = (object)index,
                               ["rindex"] = (object)(length - index),
                               ["rindex0"] = (object)(length - index - 1),
                               ["first"] = (object)(index == 0),
                               ["last"] = (object)(index == length - 1)
                           };
                           try
                           {
                               this.RenderAll(this.ForBlock, context, result);
                           }
                           catch (BreakInterrupt ex)
                           {
                               break;
                           }
                           catch (ContinueInterrupt ex)
                           {
                           }
                       }
                   }
               }));
            }
        }

        private static List<object> SliceCollectionUsingEach(
          Context context,
          IEnumerable collection,
          int from,
          int? to)
        {
            List<object> objectList = new List<object>();
            int num = 0;
            foreach (object obj in collection)
            {
                context.CheckTimeout();
                if (to.HasValue)
                {
                    if (to.Value <= num)
                        break;
                }
                if (from <= num)
                    objectList.Add(obj);
                ++num;
                if (context.MaxIterations > 0 && num > context.MaxIterations)
                {
                    throw new MaximumIterationsExceededException(For.ForTagMaxIterationsExceededException, new string[1]
          {
            context.MaxIterations.ToString()
          });
                }
            }
            return objectList;
        }

        private void BuildContext(Context context, string parent, string key, object value)
        {
            if (!(value is IDictionary<string, object> source))
                return;
            source["itemName"] = (object)key;
            context[parent] = value;
            foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(entry => entry.Value is IDictionary<string, object>)))
                this.BuildContext(context, parent + "." + key, keyValuePair.Key, keyValuePair.Value);
        }
    }
}
