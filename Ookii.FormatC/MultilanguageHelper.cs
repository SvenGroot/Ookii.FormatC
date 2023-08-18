// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;

namespace Ookii.FormatC
{
    static class MultilanguageHelper
    {
        public static IEnumerable<LanguageRegion> SplitRegions(string code, int index, int length, string startTag, string endTag, bool excludeTag, string? cssClass, Type formattingInfoType, IEnumerable<string>? types, bool needsFullContext)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (startTag == null)
            {
                throw new ArgumentNullException(nameof(startTag));
            }

            if (endTag == null)
            {
                throw new ArgumentNullException(nameof(endTag));
            }

            if (formattingInfoType == null)
            {
                throw new ArgumentNullException(nameof(formattingInfoType));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), Properties.Resources.Error_LengthLessThanZero);
            }

            if (index < 0 || index + length > code.Length)
            {
                throw new ArgumentException(Properties.Resources.Error_IndexOrLengthOutOfRange);
            }

            return SplitRegionsCore(code, index, length, startTag, endTag, excludeTag, cssClass, formattingInfoType, types, needsFullContext);
        }

        private static IEnumerable<LanguageRegion> SplitRegionsCore(string code, int index, int length, string startTag, string endTag, bool excludeTag, string? cssClass, Type formattingInfoType, IEnumerable<string>? types, bool needsFullContext)
        {
            int level = 0;
            int previousRegionStart = index;
            IFormattingInfo? nestedInfo = null;
            int end = index + length;
            for (; index < end; ++index)
            {
                if (code.MatchSubstring(index, startTag))
                {
                    if (level == 0)
                    {
                        int regionLength = index - previousRegionStart;
                        if (!excludeTag)
                        {
                            regionLength += startTag.Length;
                        }

                        yield return new LanguageRegion() { Start = previousRegionStart, Length = regionLength, NeedsFullContext = needsFullContext };

                        previousRegionStart = index + startTag.Length; // don't include the tag
                        index += startTag.Length - 1; // skip the tag
                    }

                    ++level;
                }
                else if (code.MatchSubstring(index, endTag))
                {
                    --level;
                    if (level == 0)
                    {
                        if (nestedInfo == null)
                        {
                            nestedInfo = (IFormattingInfo)Activator.CreateInstance(formattingInfoType);
                            if (nestedInfo is IFormattingInfoWithTypes infoWithTypes)
                            {
                                infoWithTypes.Types = types;
                            }
                        }

                        yield return new LanguageRegion() { Start = previousRegionStart, Length = index - previousRegionStart, FormattingInfo = nestedInfo, CssClass = cssClass };
                        previousRegionStart = index;
                        if (excludeTag)
                        {
                            previousRegionStart += endTag.Length; // don't include the tag
                        }

                        index += endTag.Length - 1; // skip the tag.
                    }
                }
            }

            if (previousRegionStart != end)
            {
                yield return new LanguageRegion() { Start = previousRegionStart, Length = end - previousRegionStart, NeedsFullContext = needsFullContext };
            }
        }
    }
}
