// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Net;

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
        private IFormattingInfo? _formattingInfo;
        private int _tabSpaces = 4;

        /// <summary>
        /// The default CSS class for the &lt;pre&gt; element wrapping the formatted
        /// output. The value is "code".
        /// </summary>
        public const string DefaultCssClass = "code";

        /// <summary>
        /// The default CSS class for the &lt;span&gt; elements wrapping line numbers.
        /// The value is "lineNumber".
        /// </summary>
        public const string DefaultLineNumberCssClass = "lineNumber";

        /// <summary>
        /// The default format string used for line numbers. The value is "{0,3}. ".
        /// </summary>
        public const string DefaultLineNumberFormat = "{0,3}. ";

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
            get { return _formattingInfo ??= new CSharpFormattingInfo(); }
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
        public string? CssClass { get; set; } = DefaultCssClass;

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
        /// The <see href="http://msdn.microsoft.com/en-us/library/txafckwd.aspx">composite format string</see> used for format the line numbes.
        /// The default value is the value of <see cref="DefaultLineNumberFormat"/>.
        /// </value>
        /// <remarks>
        /// The format string should contain the "{0}" placeholder in the position where the number itself should be.
        /// </remarks>
        public string LineNumberFormat { get; set; } = DefaultLineNumberFormat;

        /// <summary>
        /// Gets or sets the CSS class used for &lt;span&gt; elements wrapping line numbers.
        /// </summary>
        /// <value>The CSS class, or <see langword="null" /> to not use a CSS class. The default
        /// value is <see cref="DefaultLineNumberCssClass"/>.</value>
        /// <remarks>
        /// <para>
        ///   If you use <see cref="LineNumberMode.Table"/> for the <see cref="LineNumberMode"/>
        ///   property, this CSS class is applied to the &lt;td&gt; element containing the line numbers.
        /// </para>
        /// </remarks>
        public string? LineNumberCssClass { get; set; } = DefaultLineNumberCssClass;

        /// <summary>
        /// Gets a value indicating whether fallback formatting was used by the last call to <see cref="FormatCode(string)"/>.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if, on the last call to <see cref="FormatCode(string)"/>, the <see cref="FormattingInfo"/> 
        /// 	supported <see cref="ICustomFormattingInfo"/> and custom formatting failed; otherwise, <see langword="false"/>.
        /// </value>
        public bool UsedFallbackFormatting { get; private set; }
	
        /// <summary>
        /// Formats the specified source code as HTML.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <returns>The formatted HTML.</returns>
        /// <example>For an example see <see cref="CodeFormatter"/>.</example>
        public string FormatCode(string code)
        {
            var result = new StringWriter(new StringBuilder(code.Length * 2));
            FormatCode(code, result);
            return result.ToString();
        }

        /// <summary>
        /// Formats the specified source code as HTML, writing the result to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to write the formatted code to.</param>
        /// <example>For an example see <see cref="CodeFormatter"/>.</example>
        public void FormatCode(string code, TextWriter writer)
        {
            if( code == null )
                throw new ArgumentNullException(nameof(code));
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // Normalize newlines (needed for regular expressions)
            code = code.Replace("\r\n", "\n").Replace("\r", "\n");
            // Convert tabs
            code = code.Replace("\t", new string(' ', _tabSpaces));

            if (LineNumberMode == LineNumberMode.Table)
                FormatCodeLineNumberTable(code, writer);
            else if (IncludePreElement)
                writer.WriteStartElement("pre", CssClass);

            using (var codeWriter = new LineNumberTextWriter(writer))
            {
                if (LineNumberMode == LineNumberMode.Inline)
                {
                    codeWriter.LineNumberFormat = LineNumberFormat;
                    codeWriter.LineNumberClassName = LineNumberCssClass;
                }

                UsedFallbackFormatting = FormatCodeCore(FormattingInfo, code, codeWriter, true, 0, code.Length, false);
            }

            if (LineNumberMode == LineNumberMode.Table)
                writer.Write("</pre></td></tr></table></div>");
            else if (IncludePreElement)
                writer.WriteEndElement("pre");
        }

        private void FormatCodeLineNumberTable(string code, TextWriter writer)
        {
            writer.WriteStartElement("div", CssClass);
            writer.Write("<table><tr>");
            writer.WriteStartElement("td", LineNumberCssClass);
            int lineNumber = 0;

            // We assume the formatted result will have the same number of lines.
            using (var reader = new StringReader(code))
            {
                while (reader.ReadLine() != null)
                {
                    if (lineNumber > 0)
                        writer.Write("<br />");
                    ++lineNumber;
                    writer.Write(LineNumberFormat, lineNumber);
                }
            }

            writer.Write("</td><td><pre>");
        }

        private static bool FormatCodeCore(IFormattingInfo info, string code, TextWriter result, bool splitLanguageRegions, int start, int length, bool needsFullContext)
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
                if (customInfo.FormatCode(code.Substring(start, length), result))
                    return false;
                else
                    usedFallbackFormatting = true;
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
                    if (previousMatchEnd < match.Index)
                        WebUtility.HtmlEncode(code.Substring(previousMatchEnd, match.Index - previousMatchEnd), result);

                    foreach( CodeElement p in info.Patterns )
                    {
                        ProcessGroup(result, match, p);
                    }
                    previousMatchEnd = match.Index + match.Length;
                }
                match = match.NextMatch();
            }

            if( previousMatchEnd < start + length )
                WebUtility.HtmlEncode(code.Substring(previousMatchEnd, start + length - previousMatchEnd), result);

            return usedFallbackFormatting;
        }

        private static bool FormatCodeMultilanguage(IFormattingInfo info, string code, TextWriter result, int start, int length, IMultilanguageFormattingInfo multilanguageInfo)
        {
            bool usedFallbackFormatting = false;
            string? fullContextCode = null;
            LanguageRegion[] regions = multilanguageInfo.SplitRegions(code, start, length).ToArray();
            foreach( LanguageRegion region in regions )
            {
                if( region.CssClass != null )
                {
                    result.WriteStartElement(region.CssClass);
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
                    result.WriteEndElement();
                }
            }

            return usedFallbackFormatting;
        }

        private static void ProcessGroup(TextWriter result, Match match, CodeElement groupElement)
        {
            Group group = match.Groups[groupElement.Name];
            if( group.Success && group.Length != 0 )
            {
                if( groupElement.ElementNameIsCssClass )
                {
                    result.WriteStartElement(groupElement.Name);
                }

                string value = group.Value;
                if( groupElement.MatchValueProcessor != null )
                    value = groupElement.MatchValueProcessor(value);

                WebUtility.HtmlEncode(value, result);
                if( groupElement.ElementNameIsCssClass )
                {
                    result.Write("</span>");
                }
            }
        }

        private static string StripAndAdjustRegionsForContext(string code, LanguageRegion[] regions, int index, int length)
        {
            var result = new StringBuilder(length);
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
            var pattern = new StringBuilder();

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
