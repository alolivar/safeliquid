
// Type: SafeLiquid.Tags.Html.TableRow




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags.Html
{
    public class TableRow : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.B(R.Q("(\\w+)\\s+in\\s+({0}+)"), Liquid.VariableSignature);
        private string _variableName;
        private string _collectionName;
        private Dictionary<string, string> _attributes;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = TableRow.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("TableRowTagSyntaxException"), new string[0]);
            this._variableName = match.Groups[1].Value;
            this._collectionName = match.Groups[2].Value;
            this._attributes = new Dictionary<string, string>((IEqualityComparer<string>)Template.NamingConvention.StringComparer);
            R.Scan(markup, Liquid.TagAttributes, (Action<string, string>)((key, value) => this._attributes[key] = value));
            base.Initialize(tagName, markup, tokens);
        }

        public TableRow(Template template) : base(template)
        {

        }
        public override void Render(Context context, TextWriter result)
        {
            object obj = context[this._collectionName];
            if (!(obj is IEnumerable))
                return;
            IEnumerable<object> collection = ((IEnumerable)obj).Cast<object>();
            if (this._attributes.ContainsKey("offset"))
            {
                int int32 = Convert.ToInt32(this._attributes["offset"]);
                collection = collection.Skip<object>(int32);
            }
            if (this._attributes.ContainsKey("limit"))
            {
                int int32 = Convert.ToInt32(this._attributes["limit"]);
                collection = collection.Take<object>(int32);
            }
            collection = (IEnumerable<object>)collection.ToList<object>();
            int length = collection.Count<object>();
            int cols = Convert.ToInt32(context[this._attributes["cols"]]);
            int row = 1;
            int col = 0;
            result.WriteLine("<tr class=\"row1\">");
            context.Stack((Action)(() => collection.EachWithIndex((Action<object, int>)((item, index) =>
          {
              context[this._variableName] = item;
              context["tablerowloop"] = (object)Hash.FromAnonymousObject((object)new
              {
                  length = length,
                  index = (index + 1),
                  index0 = index,
                  col = (col + 1),
                  col0 = col,
                  rindex = (length - index),
                  rindex0 = (length - index - 1),
                  first = (index == 0),
                  last = (index == length - 1),
                  col_first = (col == 0),
                  col_last = (col == cols - 1)
              });
              ++col;
              using (TextWriter result1 = (TextWriter)new StringWriter(result.FormatProvider))
              {
                  this.RenderAll(this.NodeList, context, result1);
                  result.Write("<td class=\"col{0}\">{1}</td>", (object)col, (object)result1.ToString());
              }
              if (col != cols || index == length - 1)
                  return;
              col = 0;
              ++row;
              result.WriteLine("</tr>");
              result.Write("<tr class=\"row{0}\">", (object)row);
          }))));
            result.WriteLine("</tr>");
        }
    }
}
