// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides information for formatting C++ code.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class CPlusPlusFormattingInfo : IFormattingInfo
    {
        private static readonly CodeElement[] _patterns = new CodeElement[] { new CodeElement("comment", @"(/\*(.|\n)*?\*/)|(//.*?(?=$))"),
                        new CodeElement("string", @"("".*?(?<![^\\](\\\\)*\\)"")|('.*?(?<![^\\](\\\\)*\\)')|((?<=#(include|using|import)[ \t]+)<.*?>)"),
                        new CodeElement("keyword", new string[] { "abstract", "__alignof", "array", "__asm", "__assume", "__based",
                            "bool", "break", "case", "catch", "__cdecl", "char", "class", "const", "const_cast", "continue",
                            "__declspec", "default", "delegate", "delete", "deprecated", "dllexport", "dllimport", "do", "double",
                            "dynamic_cast", "else", "enum", "enum class", "enum struct", "event", "__event", "__except", "explicit",
                            "extern", "false", "__fastcall", "__finally", "finally", "float", "for", "__forceinline", "friend",
                            "friend_as", "gcnew", "generic", "goto", "__hook", "__identifier", "if", "__if_exists", "__if_not_exists",
                            "initonly", "__inline", "inline", "int", "__int8", "__int16", "__int32", "__int64", "__interface",
                            "interface class", "interface struct", "interior_ptr", "__leave", "literal", "long", "__m64", "__m128",
                            "__m128d", "__m128i", "__multiple_inheritance", "mutable", "naked", "namespace", "new", "new", "noinline",
                            "__noop", "noreturn", "nothrow", "novtable", "nullptr", "operator", "private", "property", "protected",
                            "public", "__raise", "ref struct", "ref class", "register", "reinterpret_cast", "return", "safecast",
                            "sealed", "selectany", "short", "signed", "__single_inheritance", "sizeof", "static", "static_cast",
                            "__stdcall", "struct", "__super", "switch", "template", "this", "thread", "throw", "true", "try", "__try",
                            "typedef", "typeid", "typeid", "typename", "__unaligned", "__unhook", "union", "unsigned", "using", "uuid",
                            "__uuidof", "value struct", "value class", "virtual", "__virtual_inheritance", "void", "volatile", "__w64 ",
                            "__wchar_t", "wchar_t", "while" }),
                        new CodeElement("preprocessor", new string[] { "#define", "#error", "#import", "#undef", "#elif", "#if",
                            "#include", "#using", "#else", "#ifdef", "#line", "#endif", "#ifndef", "#pragma" }) };

        /// <summary>
        /// Initializes a new instance of the <see cref="CPlusPlusFormattingInfo"/> class.
        /// </summary>
        public CPlusPlusFormattingInfo()
        {
        }

        #region IFormattingInfo Members

        /// <summary>
        /// Gets a list of regular expression patterns used to identify elements of the code.
        /// </summary>
        /// <value>
        /// A list of <see cref="CodeElement"/> classes that provide regular expressions for identifying elements of the code.
        /// </value>
        public virtual IEnumerable<CodeElement> Patterns
        {
            get { return _patterns; }
        }

        /// <summary>
        /// Gets a value that indicates whether the language to be formatted is case sensitive.
        /// </summary>
        /// <value>
        /// Returns <see langword="true" />.
        /// </value>
        public bool CaseSensitive
        {
            get { return true; }
        }

        #endregion
    }
}
