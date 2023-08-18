// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Interface for formatters that allow a custom set of identifiers to be treated as types.
    /// </summary>
    public interface IFormattingInfoWithTypes
    {
        /// <summary>
        /// Gets or sets a list of identifiers that should be treated as type names.
        /// </summary>
        /// <value>A list of identifiers that should be treated as type names.</value>
        IEnumerable<string>? Types { get; set; }
    }
}
