// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides source code syntax highlighting functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The <see cref="CodeFormatter"/> class will format source code based on the information provided
    ///   by an implementation of the <see cref="IFormattingInfo"/> interface.
    /// </para>
    /// <para>
    ///   The result will be HTML source code that will display the formatted source code when combined
    ///   with the appropriate style sheet.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// The following code sample shows how to use the <see cref="CodeFormatter"/> class to format
    /// C# source code.
    /// </para>
    /// <code>
    /// CodeFormatter formatter = new CodeFormatter();
    /// formatter.FormattingInfo = new CSharpFormattingInfo();
    /// string formattedHtml = formatter.FormatCode(System.IO.File.ReadAllText("MySourceFile.cs"));</code>
    /// </example>
    /// <threadsafety static="true" instance="false" />
    public class CodeFormatter
    {
        private IFormattingInfo _formattingInfo;
        private int _tabSpaces = 4;
        private string _lineNumberFormat = "{0,3}. ";

        /// <summary>
        /// The default CSS class for the &lt;pre&gt; element wrapping the formatted
        /// output. The value is "code".
        /// </summary>
        public const string DefaultCssClass = "code";

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFormatter"/> class.
        /// </summary>
        public CodeFormatter()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="IFormattingInfo" /> that provides information hot to format the source code.
        /// </summary>
        /// <value>
        /// The <see cref="IFormattingInfo" /> that provides information hot to format the source code.
        /// </value>
        public IFormattingInfo FormattingInfo
        {
            get { return _formattingInfo ?? (_formattingInfo = new CSharpFormattingInfo()); }
            set { _formattingInfo = value; }
        }

        /// <summary>
        /// Gets or sets the number of spaces that a tab character should be replaced with.
        /// </summary>
        /// <value>
        /// The number of spaces that a tab character should be replaced with. The default value is 4.
        /// </value>
        public int TabSpaces
        {
            get { return _tabSpaces; }
            set { _tabSpaces = value; }
        }

        /// <summary>
        /// Gets or sets the CSS class name to use on the &lt;pre&gt; element in the output HTML.
        /// </summary>
        /// <value>
        /// The CSS class name to use on the &lt;pre&gt; element. The default value is the value
        /// of <see cref="DefaultCssClass"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   If you change this value, you must also modify your CSS stylesheet accordingly.
        /// </para>
        /// <para>
        ///   If <see cref="LineNumberMode"/> is <see cref="Ookii.FormatC.LineNumberMode.Table"/>,
        ///   this class is applied to the encapsulating &lt;div&gt; element instead.
        /// </para>
        /// <para>
        ///   If you set this value to <see langword="null"/>, the element will not have a CSS class.
        /// </para>
        /// <para>
        ///   This property is ignored if <see cref="IncludePreElement"/> is <see langword="false" />.
        /// </para>
        /// </remarks>
        public string CssClass { get; set; } = DefaultCssClass;

        /// <summary>
        /// Gets or sets a value that indicates whether to emit the &lt;pre&gt; element in the
        /// output HTML.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to omit the element; <see langword="false" /> to emit the
        /// formatted code without a wrapping element. The default value is <see langword="true" />.
        /// </value>
        /// <remarks>
        /// This property is ignored when using <see cref="LineNumberMode.Table"/>, which always
        /// emits the &lt;pre&gt; element along with the table holding the line numbers.
        /// </remarks>
        public bool IncludePreElement { get; set; } = true;

        /// <summary>
        /// Gets or sets the line number mode.
        /// </summary>
        /// <value>One of the <see cref="Ookii.FormatC.LineNumberMode"/> values that indicates how line numbers are added to the output..</value>
        public LineNumberMode LineNumberMode { get; set; }

        /// <summary>
        /// Gets or sets the format string used to format the line numbers.
        /// </summary>
        /// <value>
        /// The <see href="http://msdn.microsoft.com/en-us/library/txafckwd.aspx">composite format string</see> used for format the line numbes. The default value is "{0,3}. ".
        /// </value>
        /// <remarks>
        /// The format string should contain the "{0}" placeholder in the position where the number itself should be.
        /// </remarks>
        public string LineNumberFormat
        {
            get { return _lineNumberFormat; }
            set { _lineNumberFormat = value; }
        }

        /// <summary>
        /// Gets a value indicating whether fallback formatting was used by the last call to <see cref="FormatCode"/>.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if, on the last call to <see cref="FormatCode"/>, the <see cref="FormattingInfo"/> 
        /// 	supported <see cref="ICustomFormattingInfo"/> and custom formatting failed; otherwise, <see langword="false"/>.
        /// </value>
        public bool UsedFallbackFormatting { get; private set; }
	
        /// <summary>
        /// Formats the specifies source code as HTML.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <returns>The formatted HTML.</returns>
        /// <example>For an example see <see cref="CodeFormatter"/>.</example>
        /// <exception cref="InvalidOperationException"><see cref="FormattingInfo"/> is <see langword="null" />.</exception>
        public string FormatCode(string code)
        {
            if( code == null )
                throw new ArgumentNullException(nameof(code));
            if( _formattingInfo == null )
                throw new InvalidOperationException(Properties.Resources.Error_NoFormattingInfo);

            StringBuilder result = new StringBuilder(code.Length * 2);

            // Normalize newlines
            code = code.Replace("\r\n", "\n").Replace("\r", "\n");
            // Convert tabs
            code = code.Replace("\t", new String(' ', _tabSpaces));

            UsedFallbackFormatting = FormatCodeCore(FormattingInfo, code, result, true, 0, code.Length, false);

            result.Replace("\n", "\r\n");

            switch( LineNumberMode )
            {
            case FormatC.LineNumberMode.Inline:
                AddInlineLineNumbers(result);
                break;
            case FormatC.LineNumberMode.Table:
                AddTableLineNumbers(result);
                return result.ToString(); // The <pre> is already added by this method.
            }

            if (IncludePreElement)
            {
                if (CssClass != null)
                {
                    return $"<pre class=\"{CssClass}\">{result}</pre>";
                }

                return $"<pre>{result}</pre>";
            }

            return result.ToString();
        }

        private void AddInlineLineNumbers(StringBuilder result)
        {
            string temp = result.ToString();
            result.Length = 0;
            using( StringReader reader = new StringReader(temp) )
            {
                int lineNumber = 0;
                string line;
                while( (line = reader.ReadLine()) != null )
                {
                    ++lineNumber;
                    result.Append("<span class=\"lineNumber\">");
                    result.AppendFormat(System.Globalization.CultureInfo.CurrentCulture, _lineNumberFormat, lineNumber);
                    result.Append("</span>");

                    result.AppendLine(line);
                }
            }
        }

        private void AddTableLineNumbers(StringBuilder result)
        {
            string temp = result.ToString();
            result.Length = 0;
            int lineNumber = 0;

            result.Append("<div");
            if (CssClass != null)
            {
                result.AppendFormat(" class=\"{0}\"", CssClass);
            }

            result.Append("><table><tr><td class=\"lineNumbers\">");

            using( StringReader reader = new StringReader(temp) )
            {
                while( reader.ReadLine() != null )
                {
                    if( lineNumber > 0 )
                        result.Append("<br />");
                    ++lineNumber;
                    result.AppendFormat(System.Globalization.CultureInfo.CurrentCulture, _lineNumberFormat, lineNumber);
                }
            }

            result.Append("</td><td><pre>");
            result.Append(temp);
            result.Append("</pre></td></tr></table></div>");
        }

        internal static string HtmlEncode(string input)
        {
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private static bool FormatCodeCore(IFormattingInfo info, string code, StringBuilder result, bool splitLanguageRegions, int start, int length, bool needsFullContext)
        {
            bool usedFallbackFormatting = false;
            if( splitLanguageRegions )
            {
                if (info is IMultilanguageFormattingInfo multilanguageInfo)
                {
                    return FormatCodeMultilanguage(info, code, result, start, length, multilanguageInfo);
                }
            }

            if (info is ICustomFormattingInfo customInfo)
            {
                string formattedCode = customInfo.FormatCode(code.Substring(start, length));
                if (formattedCode != null)
                {
                    result.Append(formattedCode);
                    return false;
                }
                else
                {
                    usedFallbackFormatting = true;
                }
            }

            Regex regex = CreateRegex(info);
            Match match;
            if( needsFullContext )
                match = regex.Match(code, 0, code.Length);
            else
                match = regex.Match(code, start, length);

            int previousMatchEnd = start;
            while( match.Success )
            {
                if( match.Index + match.Length > start + length )
                    break;
                if( match.Index >= start )
                {
                    if( previousMatchEnd < match.Index )
                        result.Append(HtmlEncode(code.Substring(previousMatchEnd, match.Index - previousMatchEnd)));

                    foreach( CodeElement p in info.Patterns )
                    {
                        ProcessGroup(result, match, p);
                    }
                    previousMatchEnd = match.Index + match.Length;
                }
                match = match.NextMatch();
            }

            if( previousMatchEnd < start + length )
                result.Append(HtmlEncode(code.Substring(previousMatchEnd, start + length - previousMatchEnd)));

            return usedFallbackFormatting;
        }

        private static bool FormatCodeMultilanguage(IFormattingInfo info, string code, StringBuilder result, int start, int length, IMultilanguageFormattingInfo multilanguageInfo)
        {
            bool usedFallbackFormatting = false;
            string fullContextCode = null;
            LanguageRegion[] regions = multilanguageInfo.SplitRegions(code, start, length).ToArray();
            foreach( LanguageRegion region in regions )
            {
                if( region.CssClass != null )
                {
                    result.Append("<span class=\"");
                    result.Append(region.CssClass);
                    result.Append("\">");
                }
                IFormattingInfo regionInfo = region.FormattingInfo ?? info;

                string regionCode = code;
                if( region.NeedsFullContext )
                {
                    if( fullContextCode == null )
                        fullContextCode = StripAndAdjustRegionsForContext(code, regions, start, length);
                    regionCode = fullContextCode;
                }
                usedFallbackFormatting |= FormatCodeCore(regionInfo, regionCode, result, region.FormattingInfo != null, region.Start, region.Length, region.NeedsFullContext);

                if( region.CssClass != null )
                {
                    result.Append("</span>");
                }
            }

            return usedFallbackFormatting;
        }

        private static void ProcessGroup(StringBuilder result, Match match, CodeElement groupElement)
        {
            Group group = match.Groups[groupElement.Name];
            if( group.Success && group.Length != 0 )
            {
                if( groupElement.ElementNameIsCssClass )
                {
                    result.Append("<span class=\"");
                    result.Append(groupElement.Name);
                    result.Append("\">");
                }

                string value = group.Value;
                if( groupElement.MatchValueProcessor != null )
                    value = groupElement.MatchValueProcessor(value);
                result.Append(HtmlEncode(value));

                if( groupElement.ElementNameIsCssClass )
                {
                    result.Append("</span>");
                }
            }
        }

        private static string StripAndAdjustRegionsForContext(string code, LanguageRegion[] regions, int index, int length)
        {
            StringBuilder result = new StringBuilder(length);
            int offset = index;
            foreach( LanguageRegion region in regions )
            {
                if( region.FormattingInfo == null )
                {
                    // Needs to be adjusted, and included.
                    result.Append(code, region.Start, region.Length);
                    if( region.NeedsFullContext )
                        region.Start -= offset;
                }
                else
                    offset += region.Length;
            }

            return result.ToString();
        }

        private static Regex CreateRegex(IFormattingInfo info)
        {
            StringBuilder pattern = new StringBuilder();

            bool first = true;
            foreach( CodeElement p in info.Patterns )
            {
                if( first )
                    first = false;
                else
                    pattern.Append("|");

                pattern.Append(p.Regex);
            }

            RegexOptions options = RegexOptions.Multiline;
            if( !info.CaseSensitive )
                options |= RegexOptions.IgnoreCase;
            return new Regex(pattern.ToString(), options);
        }
    }
}
