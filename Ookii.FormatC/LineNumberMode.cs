﻿namespace Ookii.FormatC
{
    /// <summary>
    /// Indicates if and how line numbers are added to the formatted code.
    /// </summary>
    public enum LineNumberMode
    {
        /// <summary>
        /// No line numbers are added.
        /// </summary>
        None,
        /// <summary>
        /// Line numbers are added inline in front of every line.
        /// </summary>
        Inline,
        /// <summary>
        /// The result is encapsulated in a table with the line numbers in a seperate cell.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This used to have the advantage of allowing the entire code to be selectable without selecting the
        ///   line numbers. Nowadays, the same is accomplished for <see cref="Inline"/> using the CSS <c>user-select: none</c>.
        /// </para>
        /// <para>
        ///   For this reason, using <see cref="Inline"/> is preferred.
        /// </para>
        /// </remarks>
        Table
    }
}
