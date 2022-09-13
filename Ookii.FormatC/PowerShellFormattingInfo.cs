// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting information for Microsoft PowerShell scripts.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This formatter will use System.Management.Automation to tokenize PowerShell code for more
    ///   accurate formatting. To use this, you must either reference System.Management.Automation
    ///   in your project, or manually load the assembly and pass it to the constructor.
    /// </para>
    /// <para>
    ///   If the System.Management.Automation.PSParser type could not used, regular expression
    ///   based formatting will be used and the <see cref="CodeFormatter.UsedFallbackFormatting"/>
    ///   will be set to <see langword="true"/> after the formatting operation.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class PowerShellFormattingInfo : IFormattingInfo, ICustomFormattingInfo
    {
        private enum PSTokenType
        {
            Unknown = 0,
            Command = 1,
            CommandParameter = 2,
            CommandArgument = 3,
            Number = 4,
            String = 5,
            Variable = 6,
            Member = 7,
            LoopLabel = 8,
            Attribute = 9,
            Type = 10,
            Operator = 11,
            GroupStart = 12,
            GroupEnd = 13,
            Keyword = 14,
            Comment = 15,
            StatementSeparator = 16,
            NewLine = 17,
            LineContinuation = 18,
            Position = 19,
        }

        private const string PSParserTypeName = "System.Management.Automation.PSParser, System.Management.Automation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        private static readonly CodeElement[] _patterns = new CodeElement[] {
                    new CodeElement("psComment", @"#.*?$"),
                    new CodeElement("psString", @""".*?""|'.*?'"),
                    new CodeElement("psKeyword", new string[] { "function", "filter", "global", "script", "local", "private", "if", 
                        "else", "elseif", "for", "foreach", "in", "while", "switch", "continue", "break", "return", "default", 
                        "param", "begin", "process", "end", "throw", "trap"}),
                    new CodeElement("psOperator", new string[] { "-band", "-bor", "-match", "-notmatch", "-like", "-notlike", "-eq", 
                        "-ne", "-gt", "-ge", "-lt", "-le", "-is", "-imatch", "-inotmatch", "-ilike", "-inotlike", "-ieq", "-ine", 
                        "-igt", "-ige", "-ilt", "-ile" }),
                    new CodeElement("psVariable", @"\$[a-zA-Z0-9]+")
        };

        private readonly Type _parserType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellFormattingInfo"/> class.
        /// </summary>
        /// <param name="systemManagementAutomation">The System.Management.Automation assembly used to load the PSParser from.
        /// If <see langword="null" />, <see cref="PowerShellFormattingInfo"/> attempts to load the type directly
        /// which requires the consuming project to reference System.Management.Automation directly.</param>
        public PowerShellFormattingInfo(Assembly systemManagementAutomation = null)
        {
            if (systemManagementAutomation == null)
            {
                // Attempt to load the PowerShell type. That way, if the project that's using Ookii.FomatC
                // references System.Management.Automation, it'll just work without any code changes.
                _parserType = Type.GetType(PSParserTypeName, false);
            }
            else
            {
                _parserType = systemManagementAutomation.GetType(PSParserTypeName, false);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether regular expression based formatting should
        /// be used even if System.Management.Automation.PSParser is available.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to force the use of regular expression based formatting;
        /// <see langword="false" /> to attempt PSParser based formatting first. The default value is
        /// <see langword="false" />.
        /// </value>
        public bool ForceFallbackFormatting { get; set; }

        /// <summary>
        /// Gets a list of regular expression patterns used to identify elements of the code.
        /// </summary>
        /// <value>
        /// A list of <see cref="CodeElement"/> classes that provide regular expressions for identifying elements of the code.
        /// </value>
        public IEnumerable<CodeElement> Patterns
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
            get { return false; }
        }

        /// <summary>
        /// Formats the specified source code.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the formatted code too.</param>
        /// <returns><see langword="true"/> if formatting succeeded, or <see langword="false"/> if custom formatting 
        /// failed and the <see cref="CodeFormatter"/> should fall back to regular formatting.</returns>
        public bool FormatCode(string code, TextWriter writer)
        {
            if( code == null )
                throw new ArgumentNullException(nameof(code));

            if( _parserType != null && !ForceFallbackFormatting )
            {
                object[] args = new object[] { code, null };
                IList tokens = (IList)_parserType.InvokeMember("Tokenize", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args, CultureInfo.CurrentCulture);
                IList errors = (IList)args[1];
                if( errors.Count > 0 )
                {
                    // Use fallback formatting if there were parsing errors.
                    return false;
                }

                int currentColumn = 1;
                int currentLine = 1;
                foreach( dynamic token in tokens )
                {
                    string tokenValue = UpdateOutputPosition(code, ref currentColumn, ref currentLine, writer, token);

                    switch( (PSTokenType)token.Type )
                    {
                    case PSTokenType.NewLine:
                        writer.Write(tokenValue);
                        currentLine++;
                        currentColumn = 1;
                        break;
                    default:
                        writer.WriteStartElement("ps" + (string)token.Type.ToString());
                        WebUtility.HtmlEncode(tokenValue, writer);
                        writer.WriteEndElement();
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        private static string UpdateOutputPosition(string code, ref int currentColumn, ref int currentLine, TextWriter result, dynamic token)
        {
            string tokenValue = code.Substring(token.Start, token.Length);
            if (token.StartLine > currentLine)
            {
                for (; currentLine < token.StartLine; ++currentLine)
                {
                    result.Write('\n');
                }

                currentColumn = 1;
            }

            for (; currentColumn < token.StartColumn; ++currentColumn)
            {
                result.Write(' ');
            }

            currentLine = token.EndLine;
            currentColumn = token.EndColumn;
            return tokenValue;
        }
    }
}
