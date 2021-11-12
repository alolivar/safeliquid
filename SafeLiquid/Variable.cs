
// Type: SafeLiquid.Variable




using SafeLiquid.Exceptions;
using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid
{
    public class Variable : IRenderable
    {
        private static readonly Regex FilterParserRegex = R.B(R.Q("(?:\\s+|{0}|{1})+"), Liquid.QuotedFragment, Liquid.ArgumentSeparator);
        private static readonly Regex FilterArgRegex = R.B(R.Q("(?:{0}|{1})\\s*({2})"), Liquid.FilterArgumentSeparator, Liquid.ArgumentSeparator, Liquid.QuotedFragment);
        private static readonly Regex QuotedAssignFragmentRegex = R.B(R.Q("\\s*({0})(.*)"), Liquid.QuotedAssignFragment);
        private static readonly Regex FilterSeparatorRegex = R.B(R.Q("{0}\\s*(.*)"), Liquid.FilterSeparator);
        private static readonly Regex FilterNameRegex = R.B(R.Q("\\s*(\\w+)"));
        private readonly string _markup;

        public List<Variable.Filter> Filters { get; set; }

        public string Name { get; set; }


        public Template Template { get; set; }

        public Variable(Template template, string markup)
        {
            this.Template = template;
            this._markup = markup;
            this.Name = (string)null;
            this.Filters = new List<Variable.Filter>();
            Match match1 = Variable.QuotedAssignFragmentRegex.Match(markup);
            if (!match1.Success)
                return;
            this.Name = match1.Groups[1].Value;
            Match match2 = Variable.FilterSeparatorRegex.Match(match1.Groups[2].Value);
            if (!match2.Success)
                return;
            foreach (string input in R.Scan(match2.Value, Variable.FilterParserRegex))
            {
                Match match3 = Variable.FilterNameRegex.Match(input);
                if (match3.Success)
                    this.Filters.Add(new Variable.Filter(match3.Groups[1].Value, R.Scan(input, Variable.FilterArgRegex).ToArray()));
            }
        }

        public void Render(Context context, TextWriter result)
        {
            object obj = this.RenderInternal(context);
            if (obj is ILiquidizable)
                obj = (object)null;
            if (obj == null)
                return;
            Func<object, object> valueTypeTransformer = Template.GetValueTypeTransformer(obj.GetType());
            if (valueTypeTransformer != null)
                obj = valueTypeTransformer(obj);

            if(obj is string)
            {
                result.Write(obj);
            }
            else if(obj is IEnumerable)
            {
                var str = string.Join(string.Empty, (obj as IEnumerable).Cast<object>().Select<object, string>((Func<object, string>)(o => ToFormattedString(o, result.FormatProvider))).ToArray<string>());
                result.Write(str);
            }else if(obj is bool)
            {
                var str = obj.ToString().ToLower();
                result.Write(str);
            }
            else
            {
                var str = ToFormattedString(obj, result.FormatProvider);
                result.Write(str);
            }
        }


        static string ToFormattedString(object obj, IFormatProvider formatProvider)
        {
            string str;
            switch (obj)
            {
                case Decimal num:
                    return num.ToString("0.#############################", formatProvider);
                case IFormattable formattable:
                    return formattable.ToString((string)null, formatProvider);
                case null:
                    str = (string)null;
                    break;
                default:
                    str = obj.ToString();
                    break;
            }
            return str ?? "";
        }


        private object RenderInternal(Context context)
        {
            if (this.Name == null)
                return (object)null;
            object obj = context[this.Name];
            foreach (Variable.Filter filter in this.Filters.ToList<Variable.Filter>())
            {
                List<object> list = ((IEnumerable<string>)filter.Arguments).Select<string, object>((Func<string, object>)(a => context[a])).ToList<object>();
                try
                {
                    list.Insert(0, obj);
                    obj = context.Invoke(filter.Name, list);
                }
                catch (FilterNotFoundException ex)
                {
                    throw new FilterNotFoundException(string.Format(Liquid.ResourceManager.GetString("VariableFilterNotFoundException"), (object)filter.Name, (object)this._markup.Trim()), ex);
                }
            }
            if (obj is IValueTypeConvertible valueTypeConvertible)
                obj = valueTypeConvertible.ConvertToValueType();
            return obj;
        }

        internal object Render(Context context) => this.RenderInternal(context);

        public class Filter
        {
            public Filter(string name, string[] arguments)
            {
                this.Name = name;
                this.Arguments = arguments;
            }

            public string Name { get; set; }

            public string[] Arguments { get; set; }
        }
    }
}
