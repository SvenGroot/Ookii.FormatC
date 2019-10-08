// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting information for Microsoft PowerShell scripts.
    /// </summary>
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

        private static readonly Type _parserType = Type.GetType("System.Management.Automation.PSParser, System.Management.Automation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", false);

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellFormattingInfo"/> class.
        /// </summary>
        public PowerShellFormattingInfo()
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
        /// <returns>
        /// An HTML fragment containing the formatted code, or <see langword="null"/> if custom formatting
        /// failed and the <see cref="CodeFormatter"/> should fall back to regular formatting.
        /// </returns>
        public string FormatCode(string code)
        {
            if( code == null )
                throw new ArgumentNullException(nameof(code));

            if( _parserType != null )
            {
                object[] args = new object[] { code, null };
                IList tokens = (IList)_parserType.InvokeMember("Tokenize", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args, CultureInfo.CurrentCulture);
                IList errors = (IList)args[1];
                if( errors.Count > 0 )
                {
                    // Use fallback formatting if there were parsing errors.
                    return null;
                }

                int currentColumn = 1;
                int currentLine = 1;
                StringBuilder result = new StringBuilder(code.Length * 2);
                foreach( dynamic token in tokens )
                {
                    string tokenValue = UpdateOutputPosition(code, ref currentColumn, ref currentLine, result, token);

                    switch( (PSTokenType)token.Type )
                    {
                    case PSTokenType.NewLine:
                        result.Append(tokenValue);
                        currentLine++;
                        currentColumn = 1;
                        break;
                    default:
                        result.AppendToken("ps" + (string)token.Type.ToString(), tokenValue);
                        break;
                    }
                }
                return result.ToString();
            }

            return null;
        }

        private static string UpdateOutputPosition(string code, ref int currentColumn, ref int currentLine, StringBuilder result, dynamic token)
        {
            string tokenValue = code.Substring(token.Start, token.Length);
            if( token.StartLine > currentLine )
            {
                result.Append('\n', token.StartLine - currentLine);
                currentColumn = 1;
            }
            if( token.StartColumn != currentColumn )
            {
                result.Append(' ', token.StartColumn - currentColumn);
            }
            currentLine = token.EndLine;
            currentColumn = token.EndColumn;
            return tokenValue;
        }
    }
}