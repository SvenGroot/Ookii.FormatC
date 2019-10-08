// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ookii.FormatC
{
    static class StringExtensions
    {
        public static bool MatchSubstring(this string value, int index, string substring)
        {
            for( int x = 0; x < substring.Length; ++x, ++index )
            {
                if( !(index < value.Length && value[index] == substring[x]) )
                    return false;
            }
            return true;
        }
    }
}
