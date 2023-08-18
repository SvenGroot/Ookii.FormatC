// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Interface for formatters that support multiple languages besides the primary one.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Some languages might support embedding regions of different languages. Examples of this
    ///   include XML literal in Visual Basic, or script regions in HTML.
    /// </para>
    /// <para>
    ///   The built-in <see cref="VisualBasicFormattingInfo"/> uses this to support XML literals.
    /// </para>
    /// </remarks>
    public interface IMultilanguageFormattingInfo
    {
        /// <summary>
        /// Splits the source code into language regions.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="index">The index in the code to start at.</param>
        /// <param name="length">The number of characters from <paramref name="index"/> to process.</param>
        /// <returns>A list of language regions.</returns>
        /// <remarks>
        /// 	<para>
        /// The identified regions may themselves contain multiple languages if the formatter
        /// specified for that region supports <see cref="IMultilanguageFormattingInfo"/>. The
        /// exception is if the <see cref="LanguageRegion.FormattingInfo"/> property is <see langword="null"/>
        /// in which case the current formatter will be used, and the region will not be split again.
        /// </para>
        /// 	<para>
        /// For example, an XML literal in Visual Basic can contain embedded expressions with VB code.
        /// The VB formatter doesn't need to identify the embedded expressions; the XML formatter will
        /// do that.
        /// </para>
        /// </remarks>
        IEnumerable<LanguageRegion> SplitRegions(string code, int index, int length);
    }
}
