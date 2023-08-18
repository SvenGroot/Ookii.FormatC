// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Interface for classes that provide information for formatting a programming language.
    /// </summary>
    public interface IFormattingInfo
    {
        /// <summary>
        /// Gets a list of regular expression patterns used to identify elements of the code.
        /// </summary>
        /// <value>
        /// A list of <see cref="CodeElement"/> classes that provide regular expressions for identifying elements of the code.
        /// </value>
        /// <remarks>
        /// <para>
        ///   When implementing this property, you should return a <see cref="CodeElement"/> for each element that needs
        ///   a different formatting style, such as keywords, comments or strings. The <see cref="CodeElement.Name"/> property
        ///   will be used as the CSS class name in the HTML output of the <see cref="CodeFormatter"/>.
        /// </para>
        /// <para>
        ///    When processing the source code, the <see cref="CodeFormatter"/> will process the patterns in the
        ///    order they are provided here.
        /// </para>
        /// </remarks>
        IEnumerable<CodeElement> Patterns { get; }

        /// <summary>
        /// Gets a value that indicates whether the language to be formatted is case sensitive.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the programming language is case sensitive; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// <para>
        ///   When the value of this property is <see langword="false" />, the regular expressions provided
        ///   by the <see cref="Patterns"/> property will be processed with the <see cref="System.Text.RegularExpressions.RegexOptions.IgnoreCase"/>
        ///   option set.
        /// </para>
        /// </remarks>
        bool CaseSensitive { get; }
    }
}
