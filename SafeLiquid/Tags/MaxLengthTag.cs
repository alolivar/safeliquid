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
    /// The max length tag.
    /// </summary>
    public class MaxLengthTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string MaxLengthTagName = "maxLength";

        /// <summary>
        /// The argument regex.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(@"^(\s+)?(?<variableName>[^\s]+)\s+(?<length>\d+)(\s+)?$");

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The max length.
        /// </summary>
        private int maxLength = 1;

        public MaxLengthTag(Template template) : base(template) { }

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
                throw new ParseTemplateException($"Invalid {MaxLengthTagName} tag. Expected format {{% {MaxLengthTagName} <variable> <length> %}}");
            }

            this.variableName = match.Groups["variableName"].Value;
            this.maxLength = int.Parse(match.Groups["length"].Value);
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
                if (context[this.variableName] is IEnumerable<object> arrayValue)
                {
                    var length = arrayValue.ToArray().Length;
                    if (length > this.maxLength)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} too long. Current length {length}. Max length {this.maxLength}");
                    }
                }
                else if (context[this.variableName] is string stringValue)
                {
                    var length = stringValue.Length;
                    if (length > this.maxLength)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} too long. Current length {length}. Max length {this.maxLength}");
                    }
                }
            }
        }
    }
}