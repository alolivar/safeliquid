using SafeLiquid.Exceptions;
using SafeLiquid.FileSystems;
using SafeLiquid.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SafeLiquid.Tags
{
    public class Include : SafeLiquid.Block
    {
        private static readonly Regex Syntax = R.B("({0}+)(\\s+(?:with|for)\\s+({0}+))?", Liquid.QuotedFragment);
        private string _templateName;
        private string _variableName;
        private Dictionary<string, string> _attributes;

        public Include(Template template) : base(template) { }

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = Include.Syntax.Match(markup);
            if (!match.Success)
                throw new SyntaxException(Liquid.ResourceManager.GetString("IncludeTagSyntaxException"), new string[0]);
            this._templateName = match.Groups[1].Value;
            this._variableName = match.Groups[3].Value;
            if (string.IsNullOrEmpty(_variableName))
                this._variableName = (string)null;
            this._attributes = new Dictionary<string, string>((IEqualityComparer<string>)Template.NamingConvention.StringComparer);
            R.Scan(markup, Liquid.TagAttributes, (Action<string, string>)((key, value) => this._attributes[key] = value));
            base.Initialize(tagName, markup, tokens);
        }

        protected override void Parse(List<string> tokens)
        {
        }

        public override void Render(Context context, TextWriter result)
        {
            if (!(context.Registers["file_system"] is IFileSystem fileSystem1))
                fileSystem1 = Template.FileSystem;
            IFileSystem fileSystem2 = fileSystem1;
            ITemplateFileSystem templateFileSystem = fileSystem2 as ITemplateFileSystem;
            Template partial = (Template)null;
            if (templateFileSystem != null)
                partial = templateFileSystem.GetTemplate(context, this._templateName);
            if (partial == null)
                partial = Template.Parse(fileSystem2.ReadTemplateFile(context, this._templateName), Template.PreStrainer);

            // force passing down the file system.
            if(partial.FileSystem is BlankFileSystem)
            {
                partial.FileSystem = this.Template.FileSystem;
            }

            string shortenedTemplateName = this._templateName.Substring(1, this._templateName.Length - 2);
            object variable = context[this._variableName ?? shortenedTemplateName, this._variableName != null];
            context.Stack((Action)(() =>
           {
               foreach (KeyValuePair<string, string> attribute in this._attributes)
                   context[attribute.Key] = context[attribute.Value];
               if (variable is IEnumerable)
               {
                   ((IEnumerable)variable).Cast<object>().ToList<object>().ForEach((Action<object>)(v =>
            {
                   context[shortenedTemplateName] = v;
                   partial.Render(result, RenderParameters.FromContext(context, result.FormatProvider));
               }));
               }
               else
               {
                   context[shortenedTemplateName] = variable;
                   partial.Render(result, RenderParameters.FromContext(context, result.FormatProvider));
               }
           }));
        }
    }
}
