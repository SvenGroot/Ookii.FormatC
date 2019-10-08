// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Provides formatting info for XML documents.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Because the XML formatter is regular expression based, it has some limitations.
    /// </para>
    /// <para>
    ///   The most important is that use of the > character as text (not part of an XML tag) is not handled correctly. If the > character
    ///   occurs within an attribute value, any further attributes of that element will not be colored correctly.
    ///   If the > character occurs in the text content of the document, it will be colored as an element delimiter.
    /// </para>
    /// <para>
    ///   To avoid these issues, we recommend that you encode the > character as &amp;gt; in XML documents that you
    ///   wish to process with FormatC.
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public class XmlFormattingInfo : IFormattingInfo
    {
        // NOTE: If you modify this, don't forget to copy the change to XmlLiteralFormattingInfo.
        private static readonly CodeElement[] _patterns = new CodeElement[] { new CodeElement("comment", "<!--([^-]|-[^-])*?-->"),
                    new CodeElement("tagDelimiter", @"<!\[CDATA\[|(?<=<!\[CDATA\[(.|\n)*?)\]\]>|<(\?|/|!)?|(?<=</?(.|\n)*?)/?>|(?<=<\?(.|\n)*?)\?>"),
                    new CodeElement("cdata", @"(?<=<!\[CDATA\[)(.|\n)*?(?=\]\]>)"),
                    new CodeElement("tagName", @"(?<=<(\?|/|!)?)[\w:.-]*?(?=( |>))"),
                    new CodeElement("attributeValue", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?)("".*?""|'.*?')"),
                    new CodeElement("attributeName", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?\s)[\w:.-]*"),
                    new CodeElement("attributeDelimiter", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?)="),
                    new CodeElement("entity", "&.*?;")
                };

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFormattingInfo"/> class.
        /// </summary>
        public XmlFormattingInfo()
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
            get
            {
                return true;
            }
        }

        #endregion
    }
}
