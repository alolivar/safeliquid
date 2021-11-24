// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredTag.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;

    using SafeLiquid;

    /// <summary>
    /// The required tag.
    /// </summary>
    public class RequiredTag : Tag
    {
        /// <summary>
        /// The required tag name.
        /// </summary>
        public const string RequiredTagName = "required";

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        public RequiredTag(Template template) : base(template) { }

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
            if (!context.HasKey(this.variableName))
            {
                throw new ParseTemplateException($"{this.variableName} is required");
            }
        }
    }
}
