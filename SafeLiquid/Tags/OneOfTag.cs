// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaxLengthTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SafeLiquid;

    /// <summary>
    /// The one of tag.
    /// </summary>
    public class OneOfTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string OneOfTagName = "oneOf";

        /// <summary>
        /// The argument regex.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(@"^(\s+)?(?<variableName>[^\s]+)\s+""(?<option>[^\s]+)""(\s+""(?<option>[^\s]+)"")+(\s+)?$");

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The max length.
        /// </summary>
        private HashSet<string> validOptions = new HashSet<string>();

        public OneOfTag(Template template)
            : base(template)
        {

        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="markup">
        /// The markup.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            base.Initialize(tagName, markup, tokens);

            var match = ArgumentRegex.Match(markup);
            if (!match.Success)
            {
                throw new ParseTemplateException(
                    $"Invalid {OneOfTagName} tag. Expected format {{% {OneOfTagName} <variable> \"<option1>\" \"<option2>\" %}}");
            }

            this.variableName = match.Groups["variableName"].Value;

            for (var i = 0; i < match.Groups["option"].Captures.Count; i++)
            {
                var capture = match.Groups["option"].Captures[i];
                this.validOptions.Add(capture.Value);
            }
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public override void Render(Context context, TextWriter result)
        {
            if (context.HasKey(this.variableName))
            {
                if (context[this.variableName] is string stringValue)
                {
                    if (!this.validOptions.Contains(stringValue))
                    {
                        var optionsString = string.Join(", ", this.validOptions.OrderBy(option => option));
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} must be one of: {optionsString}");
                    }
                }
            }
        }
    }
}