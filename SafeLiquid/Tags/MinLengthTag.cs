// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinLengthTag.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SafeLiquid;

    /// <summary>
    /// The min length tag.
    /// </summary>
    public class MinLengthTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string MinLengthTagName = "minLength";

        /// <summary>
        /// The argument regex.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(@"^(\s+)?(?<variableName>[^\s]+)\s+(?<length>\d+)(\s+)?$");

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The min length.
        /// </summary>
        private int minLength;

        public MinLengthTag(Template template) : base(template) { }

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
                throw new ParseTemplateException($"Invalid {MinLengthTagName} tag. Expected format {{% {MinLengthTagName} <variable> <length> %}}");
            }

            this.variableName = match.Groups["variableName"].Value;
            this.minLength = int.Parse(match.Groups["length"].Value);
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
                    if (length < this.minLength)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} too short. Current length {length}. Min length {this.minLength}");
                    }
                }
                else if (context[this.variableName] is string stringValue)
                {
                    var length = stringValue.Length;
                    if (length < this.minLength)
                    {
                        throw new ParseTemplateException(
                            $"<% {this.TagName} {this.variableName} %>: {this.variableName} too short. Current length {length}. Min length {this.minLength}");
                    }
                }
            }
        }
    }
}