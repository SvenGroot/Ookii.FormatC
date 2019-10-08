// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Used for VB XML literals; mostly the same as XmlFormattingInfo, only it supports embedded expressions.
    /// </summary>
    class XmlLiteralFormattingInfo : IFormattingInfo, IMultilanguageFormattingInfo, IFormattingInfoWithTypes
    {
        // NOTE: If you modify this (except embeddedExpressionDelimiter), don't forget to copy the change to XmlFormattingInfo.
        private static readonly CodeElement[] _patterns = new CodeElement[] { new CodeElement("comment", "<!--([^-]|-[^-])*?-->"),
                    new CodeElement("embeddedExpressionDelimiter", @"(<%=)|((?<=<%=.*?)%>)"),
                    new CodeElement("tagDelimiter", @"<!\[CDATA\[|(?<=<!\[CDATA\[(.|\n)*?)\]\]>|<(\?|/|!)?|(?<=</?(.|\n)*?)/?>|(?<=<\?(.|\n)*?)\?>"),
                    new CodeElement("cdata", @"(?<=<!\[CDATA\[)(.|\n)*?(?=\]\]>)"),
                    new CodeElement("tagName", @"(?<=<(\?|/|!)?)[\w:.-]*?(?=( |>))"),
                    new CodeElement("attributeValue", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?)("".*?""|'.*?')"),
                    new CodeElement("attributeName", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?\s)[\w:.-]*"),
                    new CodeElement("attributeDelimiter", @"(?<=<(\?|!)?((.|\n)(?!(?<!%)>))*?)="),
                    new CodeElement("entity", "&.*?;")
                };

        public IEnumerable<CodeElement> Patterns
        {
            get { return _patterns; }
        }


        public bool CaseSensitive
        {
            get { return true; }
        }

        public IEnumerable<string> Types { get; set; }

        public IEnumerable<LanguageRegion> SplitRegions(string code, int index, int length)
        {
            return MultilanguageHelper.SplitRegions(code, index, length, "<%=", "%>", false, null, typeof(VisualBasicFormattingInfo), Types, true);
        }

    }
}
