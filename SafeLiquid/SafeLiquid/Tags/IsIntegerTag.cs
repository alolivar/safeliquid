// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsIntegerTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;

    using SafeLiquid;

    /// <summary>
    /// The is integer tag.
    /// </summary>
    public class IsIntegerTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string IsIntTagName = "isInteger";

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;


        public IsIntegerTag(Template template) : base(template) { }

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
            if (context.HasKey(this.variableName) && !(context[this.variableName] is int || context[this.variableName] is long || (context[this.variableName] is string stringValue && long.TryParse(stringValue, out _))))
            {
                throw new ParseTemplateException($"<% {this.TagName} {this.variableName} %>: {this.variableName} must be an integer");
            }
        }
    }
}