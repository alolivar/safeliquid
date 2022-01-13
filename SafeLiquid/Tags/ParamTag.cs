// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParamTag.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    using SafeLiquid;
    using SafeLiquid.Exceptions;
    using SafeLiquid.Util;

    /// <summary>
    /// The param.
    /// </summary>
    // ReSharper disable once StyleCop.SA1650
    public class ParamTag : Tag
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public const string ParamTagName = "param";

        /// <summary>
        /// The syntax.
        /// </summary>
        private static readonly Regex Syntax = R.B(R.Q(@"({0}+)\s*=\s*(.*)\s*"), Liquid.VariableSignature);

        /// <summary>
        /// The variable name.
        /// </summary>
        private string variableName;

        /// <summary>
        /// The variable.
        /// </summary>
        private Variable variable;

        public ParamTag(Template template) : base(template) { }

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
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                this.variableName = syntaxMatch.Groups[1].Value;
                this.variable = new Variable(Template, syntaxMatch.Groups[2].Value);
            }
            else
            {
                throw new SyntaxException("Error");
            }

            base.Initialize(tagName, markup, tokens);
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
            switch (this.variableName.ToLower())
            {
                case "syntax":
                    string fromValue;
                    using (TextWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                    {
                        this.variable.Render(context, writer);
                        fromValue = writer.ToString();
                    }

                    if (Enum.TryParse<SyntaxCompatibility>(fromValue, out var syntax))
                    {
                        context.SyntaxCompatibilityLevel = syntax;
                    }

                    break;
            }
        }
    }
}
