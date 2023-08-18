// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System.Collections.Generic;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting info for Visual Basic code.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Visual Basic contains several keywords that are contextual keywords. For example, <c>From</c> is a keyword
    ///   in a Linq expression, but not elsewhere, and you can still have variables named <c>From</c>
    ///   without enclosing them in [].
    /// </para>
    /// <para>
    ///   Because of the limitations of regular expressions, the <see cref="VisualBasicFormattingInfo"/> cannot
    ///   determine when a contextual keyword should be treated as a keyword and when it should be treated
    ///   as a regular identifier. Because of this, it always treats them as keywords.
    /// </para>
    /// <para>
    ///   You can prefix an identifier that is also a contextual keyword with ` (e.g. <c>`From</c>) to prevent it from being
    ///   highlighted as a keyword. The ` character will not appear in the formatted output, and the identifier
    ///   will not be highlighted.
    /// </para>
    /// <para>
    ///   You can specify identifiers that should be colored as type names using the <see cref="Types"/> property.
    ///   These identifiers will then always be formatted as type names (even in contexts where they are not).
    ///   Like with contextual keywords, you can prefix an identifier with ` to prevent it from being highlighted
    ///   as a type name.
    /// </para>
    /// <para>
    ///   XML literals are supported, however the XML literals must be marked explicitly with with [xml][/xml].
    ///   For example, this would look like this with a simple XML literal: <c>Dim xmlLiteral = [xml]&lt;Foo /&gt;[/xml]</c>.
    /// </para>
    /// <para>
    ///   The [xml][/xml] tags will not be included in the output, and the contents of those tags will be formatted
    ///   as XML literals. Embedded expressions in XML literals (which are delimited by &lt;%= %&gt; blocks) are also
    ///   supported, and the contents of embedded expressions will be formatted as Visual Basic code. However, due to the
    ///   limitations of regular expressions, having an XML literal inside an embedded expression in another XML
    ///   literal is not supported.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class VisualBasicFormattingInfo : IFormattingInfo, IMultilanguageFormattingInfo, IFormattingInfoWithTypes
    {
        private List<CodeElement>? _patterns;
        private IEnumerable<string>? _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicFormattingInfo"/> class.
        /// </summary>
        public VisualBasicFormattingInfo()
        {
        }

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
                    _patterns = new List<CodeElement> {
                        new CodeElement("comment", @"('|REM).*?$"),
                        new CodeElement("xmlImportAttributeName", @"(?<=Imports <).*?(?==)"),
                        new CodeElement("xmlImportAttributeDelimiter", @"(?<=Imports <.*?)="),
                        new CodeElement("xmlImportAttributeQuotes", @"(?<=Imports <.*?=.*)(""|')"),
                        new CodeElement("xmlImportAttributeValue", @"(?<=Imports <.*?=.*(""|')).*?(?=(""|'))"),
                        new CodeElement("string", @""".*?""c?"),
                        new CodeElement("escapedContextualKeyword", @"`[a-zA-Z0-9_]+") { ElementNameIsCssClass = false, MatchValueProcessor = value => value.Substring(1) },
                        new CodeElement("escapedIdentifier", @"\[[a-zA-Z0-9_]+\]") { ElementNameIsCssClass = false },
                        new CodeElement("keyword", new string[] { "AddHandler", "AddressOf", "Alias", "And", "AndAlso", "As", "Boolean", "ByRef",
                            "Byte", "ByVal", "Call", "Case", "Catch", "CBool", "CByte", "CChar", "CDate", "CDbl", "CDec", "Char", "CInt", "Class",
                            "CLng", "CObj", "Const", "Continue", "CSByte", "CShort", "CSng", "CStr", "CType", "CUInt", "CULng", "CUShort", "Date",
                            "Decimal", "Declare", "Default", "Delegate", "Dim", "DirectCast", "Do", "Double", "Each", "Else", "ElseIf", "End",
                            "EndIf", "Enum", "Erase", "Error", "Event", "Exit", "False", "Finally", "For","Friend", "Function", "Get", "GetType",
                            "GetXMLNamespace", "Global", "GoSub", "GoTo", "Handles", "If", "Implements", "Imports", "In", "Inherits", "Integer",
                            "Interface", "Is", "IsNot", "Let", "Lib", "Like", "Long", "Loop", "Me", "Mod", "Module", "MustInherit", "MustOverride",
                            "MyBase", "MyClass", "Namespace", "Narrowing", "New", "Next", "Not", "Nothing",
                            "NotInheritable", "NotOverridable", "Object", "Of", "On", "Operator", "Option", "Optional", "Or", "OrElse", "Out",
                            "Overloads", "Overridable", "Overrides", "ParamArray", "Partial", "Private", "Property", "Protected", "Public",
                            "RaiseEvent", "ReadOnly", "ReDim", "RemoveHandler", "Resume", "Return", "SByte", "Select", "Set", "Shadows", "Shared",
                            "Short", "Single", "Static", "Step", "Stop", "String", "Structure", "Sub", "SyncLock", "Then", "Throw", "To", "True",
                            "Try", "TryCast", "TypeOf", "UInteger", "ULong", "UShort", "Using", "Variant", "Wend", "When", "While", "Widening",
                            "With", "WithEvents", "WriteOnly", "Xor", 
                            // The rest are contextual keywords
                            "Aggregate", "Ansi", "Assembly", "Auto", "Binary", "Compare", "Custom", "Distinct", "Equals", "Explicit", "From",
                            "Group By", "Group Join", "Into", "IsFalse", "IsTrue", "Join", "Key", "Mid", "Off", "Order By", "Preserve", "Skip",
                            "Strict", "Take", "Text", "Unicode", "Until", "Where"
                        }),
                        new CodeElement("xmlDelimiter", @"((?<=\.)@)|((?<=\.)<)|((?<=\.<\S*?)>)|((?<=Imports )<)|((?<=Imports <.*?)>)"),
                        new CodeElement("preprocessor", new string[] { "#Const", "#Else", "#ElseIf", "#End", "#If", "#Region", "#ExternalSource" })
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
        /// Returns <see langword="false" />.
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return false;
            }
        }

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


        /// <summary>
        /// Splits the source code into language regions.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="index">The index in the code to start at.</param>
        /// <param name="length">The number of characters from <paramref name="index"/> to process.</param>
        /// <returns>A list of language regions.</returns>
        public IEnumerable<LanguageRegion> SplitRegions(string code, int index, int length)
        {
            return MultilanguageHelper.SplitRegions(code, index, length, "[xml]", "[/xml]", true, "xmlLiteral", typeof(XmlLiteralFormattingInfo), Types, false);
        }
    }
}
