// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaxValueTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using SafeLiquid;

    /// <summary>
    /// The max value tag.
    /// </summary>
    public class MaxValueTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string MaxValueTagName = "maxValue";

        /// <summary>
        /// The argument regex.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(@"^(\s+)?(?<variableName>[^\s]+)\s+(?<value>\d+)(\s+)?$");

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The max value.
        /// </summary>
        private long maxValue;

        public MaxValueTag(Template template) : base(template) { }

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
                throw new ParseTemplateException($"Invalid {MaxValueTagName} tag. Expected format {{% {MaxValueTagName} <variable> <value> %}}");
            }

            this.variableName = match.Groups["variableName"].Value;
            this.maxValue = long.Parse(match.Groups["value"].Value);
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
                if (context[this.variableName] is int intValue)
                {
                    if (intValue > this.maxValue)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} must be less than or equal to {this.maxValue}");
                    }
                }
                else if (context[this.variableName] is long longValue)
                {
                    if (longValue > this.maxValue)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} must be less than or equal to {this.maxValue}");
                    }
                }
            }
        }
    }
}