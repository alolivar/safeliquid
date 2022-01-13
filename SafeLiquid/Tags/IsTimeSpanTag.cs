// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsTimeSpanTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SafeLiquid;

    /// <summary>
    /// The is time span tag.
    /// </summary>
    public class IsTimeSpanTag : Tag
    {
        /// <summary>
        /// The is time span tag name.
        /// </summary>
        public const string IsTimeSpanTagName = "isTimeSpan";

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        public IsTimeSpanTag(Template template) : base(template) { }

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
            this.variableName = markup?.Trim();
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
            if (context.HasKey(this.variableName) && (!(context[this.variableName] is string stringValue) || !TimeSpan.TryParse(stringValue, out _)))
            {
                throw new ParseTemplateException($"<% {this.TagName} {this.variableName} %>: {this.variableName} must be a valid time span (e.g 1.00:00:00)");
            }
        }
    }
}