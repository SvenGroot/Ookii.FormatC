// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting info for C# code.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   C# contains several keywords that are contextual keywords. For example, <c>from</c> is a keyword
    ///   in a Linq expression, but not elsewhere, and you can still have variables named <c>from</c>
    ///   without prefixing them with @.
    /// </para>
    /// <para>
    ///   Because of the limitations of regular expressions, the <see cref="CSharpFormattingInfo"/> cannot
    ///   determine when a contextual keyword should be treated as a keyword and when it should be treated
    ///   as a regular identifier. Because of this, it always treats them as keywords.
    /// </para>
    /// <para>
    ///   You can prefix an identifier that is also a contextual keyword with ` (e.g. <c>`from</c>) to prevent it from being
    ///   highlighted as a keyword. The ` character will not appear in the formatted output, and the identifier
    ///   will not be highlighted.
    /// </para>
    /// <para>
    ///   You can specify identifiers that should be colored as type names using the <see cref="Types"/> property.
    ///   These identifiers will then always be formatted as type names (even in contexts where they are not).
    ///   Like with contextual keywords, you can prefix an identifier with ` to prevent it from being highlighted
    ///   as a type name.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="false" />
    public class CSharpFormattingInfo : IFormattingInfo, IFormattingInfoWithTypes
    {
        private List<CodeElement>? _patterns;
        private IEnumerable<string>? _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpFormattingInfo"/> class.
        /// </summary>
        public CSharpFormattingInfo()
        {
        }

        #region IFormattingInfo Members

        /// <summary>
        /// Gets a list of regular expression patterns used to identify elements of the code.
        /// </summary>
        /// <value>
        /// A list of <see cref="CodeElement"/> classes that provide regular expressions for identifying elements of the code.
        /// </value>
        public IEnumerable<CodeElement> Patterns
        {
            get
            {
                if (_patterns == null)
                {
                    _patterns = new List<CodeElement>() {
                        new CodeElement("comment", @"(/\*(.|\n)*?\*/)|(//.*?(?=$))"),
                        new CodeElement("string", @"(@("".*?"")*"".*?"")|("".*?(?<![^\\](\\\\)*\\)"")|('.*?(?<![^\\](\\\\)*\\)')"),
                        new CodeElement("escapedContextualKeyword", @"`[a-zA-Z0-9_]+") { ElementNameIsCssClass = false, MatchValueProcessor = value => value.Substring(1) },
                        new CodeElement("escapedIdentifier", @"@[a-zA-Z0-9_]+") { ElementNameIsCssClass = false },
                        new CodeElement("keyword", new[] { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
                            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
                            "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in",
                            "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out",
                            "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
                            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
                            "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
                            // The remainder are contextual keywords
                            // Note: "args" is deliberately excluded because it provides access to the "args" variable
                            // in top-level statements, and I don't think it should be colored as a keyword even in
                            // that context.
                            "add", "and", "alias", "ascending", /* "args", */ "async", "await", "by", "descending", "dynamic", "equals", "from", "get", "global",
                            "group", "init", "into", "join", "let", "managed", "nameof", "nint", "not", "notnull", "nuint", "on", "or", "orderby",
                            "partial", "record", "remove", "required", "select", "set", "unmanaged", "value", "var", "when", "where", "with", "yield",
                        }),
                        new CodeElement("preprocessor", new string[] { "#if", "#else", "#elif", "#endif", "#define", "#undef", "#warning", "#error",
                        "#line", "#region", "#endregion", "#pragma", "#nullable" })
                    };

                    if (_types != null)
                    {
                        _patterns.Add(new CodeElement("type", _types));
                    }
                }

                return _patterns;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the language to be formatted is case sensitive.
        /// </summary>
        /// <value>
        /// Returns <see langword="true" />.
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return true;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets a list of identifiers that should be treated as type names.
        /// </summary>
        /// <value>
        /// A list of identifiers that should be treated as type names.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The context in which these names occur is not considered, so they will be formatted as type names
        ///   in whatever context they occur.
        /// </para>
        /// </remarks>
        public IEnumerable<string>? Types
        {
            get { return _types; }
            set
            {
                _types = value;
                _patterns = null;
            }
        }
    }
}
