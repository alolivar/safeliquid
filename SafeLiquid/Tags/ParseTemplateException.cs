// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseTemplateException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeLiquid.Tags
{
    using System;

    /// <summary>
    /// The template not found exception.
    /// </summary>
    public class ParseTemplateException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseTemplateException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ParseTemplateException(string message)
            : base(message)
        {
        }
    }
}
