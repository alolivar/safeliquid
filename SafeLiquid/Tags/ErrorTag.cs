// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorTag.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;

    using SafeLiquid;

    /// <summary>
    /// The error tag.
    /// </summary>
    public class ErrorTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string ErrorTagName = "error";

        /// <summary>
        /// The error message.
        /// </summary>
        private string errorMessage;

        public ErrorTag(Template template) : base(template){ }

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
            this.errorMessage = markup?.Trim().Trim('"', '\'');
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
            throw new ParseTemplateException(
                string.IsNullOrWhiteSpace(this.errorMessage) ? $"Check <% {this.TagName} %> tags in template" : this.errorMessage);
        }
    }
}
