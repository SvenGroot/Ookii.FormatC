// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Represents a region of multi-language source code that should be formatted by a specific region.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The specified region may itself contain multiple language regions if the formatter specified
    ///   is not <see langword="null"/> and implements <see cref="IMultilanguageFormattingInfo"/>.
    /// </para>
    /// </remarks>
    public sealed class LanguageRegion
    {
        /// <summary>
        /// Gets or sets the index in the source code of the first character of the language region.
        /// </summary>
        /// <value>The zero-based index in the source code of the first character of the language region.</value>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the length of the language region.
        /// </summary>
        /// <value>The length of the language region.</value>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the formatting info used to highlight the language region.
        /// </summary>
        /// <value>The formatting info, or <see langword="null"/> to use the formatter that split the code into regions.</value>
        public IFormattingInfo FormattingInfo { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the entire language region.
        /// </summary>
        /// <value>The CSS class for the language region, or <see langword="null"/> if the region doesn't need to be surrounded by an element with a CSS class.</value>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the formatter needs the full source for context, not just the region itself.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if the formatter needs the full source for context; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This only applies if <see cref="FormattingInfo"/> is <see langword="null"/>.
        /// </para>
        /// <para>
        ///   If this property is <see langword="true" /> (and <see cref="FormattingInfo"/> is <see langword="null"/>), the formatter will use the
        ///   entire string, but with all regions that use different formatters removed.
        /// </para>
        /// </remarks>
        public bool NeedsFullContext { get; set; }
    }
}
