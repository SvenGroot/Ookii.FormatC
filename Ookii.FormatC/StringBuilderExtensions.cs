// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ookii.FormatC
{
    static class StringBuilderExtensions
    {
        public static void AppendToken(this StringBuilder sb, string className, string contents)
        {
            if( sb == null )
                throw new ArgumentNullException(nameof(sb));
            if( className == null )
                throw new ArgumentNullException(nameof(className));
            if( contents == null )
                throw new ArgumentNullException(nameof(contents));

            sb.Append("<span class=\"");
            sb.Append(className);
            sb.Append("\">");
            sb.Append(CodeFormatter.HtmlEncode(contents));
            sb.Append("</span>");
        }
    }
}
