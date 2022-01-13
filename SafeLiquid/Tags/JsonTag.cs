// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;

    using SafeLiquid;

    using Newtonsoft.Json;

    /// <summary>
    /// The json tag.
    /// </summary>
    public class JsonTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string JsonTagName = "json";

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        public JsonTag(Template template) : base(template) { }

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
                throw new ParseTemplateException($"<% {this.TagName} {this.variableName} %>: {this.variableName} not found");
            }

            result.WriteLine(JsonConvert.SerializeObject(context[this.variableName], Formatting.Indented));
        }
    }
}
