// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinValueTag.cs" company="Microsoft">
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
    /// The min value tag.
    /// </summary>
    public class MinValueTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string MinValueTagName = "minValue";

        /// <summary>
        /// The argument regex.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(@"^(\s+)?(?<variableName>[^\s]+)\s+(?<value>\d+)(\s+)?$");

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The min value.
        /// </summary>
        private long minValue;

        public MinValueTag(Template template) : base(template) { }

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
                throw new ParseTemplateException($"Invalid {MinValueTagName} tag. Expected format {{% {MinValueTagName} <variable> <value> %}}");
            }

            this.variableName = match.Groups["variableName"].Value;
            this.minValue = long.Parse(match.Groups["value"].Value);
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
                    if (intValue < this.minValue)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} must be greater than or equal to {this.minValue}");
                    }
                }
                else if (context[this.variableName] is long longValue)
                {
                    if (longValue < this.minValue)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} must be greater than or equal to {this.minValue}");
                    }
                }
            }
        }
    }
}