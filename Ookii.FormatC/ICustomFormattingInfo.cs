// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Interface for classes that provide custom, not regex-based formatting.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Implement this interface in addition to <see cref="IFormattingInfo"/> if you can do custom formatting, e.g.
    ///   using a parser.
    /// </para>
    /// <para>
    ///   If it's possible for parsing to fail (keep in mind that code formatters might be asked to process
    ///   incomplete fragments of code), make sure you return <see langword="null"/> from the <see cref="FormatCode"/>
    ///   method (do not throw an exception), in which case the <see cref="CodeFormatter"/> will fall back
    ///   to regular regex-based formatting.
    /// </para>
    /// <para>
    ///   If your <see cref="FormatCode"/> method does not return <see langword="null"/>, the members of
    ///   <see cref="IFormattingInfo"/> will not be used.
    /// </para>
    /// </remarks>
    public interface ICustomFormattingInfo
    {
        /// <summary>
        /// Formats the specified source code.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the formatted code too.</param>
        /// <returns><see langword="true"/> if formatting succeeded, or <see langword="false"/> if custom formatting 
        /// failed and the <see cref="CodeFormatter"/> should fall back to regular formatting.</returns>
        bool FormatCode(string code, TextWriter writer);
    }
}
