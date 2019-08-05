using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace dm.AOL.Bot
{
    public static class Extensions
    {
        public static string Format(this int source)
        {
            return string.Format("{0:#,##0}", source);
        }

        public static string ToDate(this DateTime source)
        {
            //return source.ToString("r");
            return source.ToString(@"ddd, d MMM yyyy HH:mm \U\T\C");
        }

        public static string TrimEnd(this string source, string suffixToRemove, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {

            if (source != null && suffixToRemove != null && source.EndsWith(suffixToRemove, comparisonType))
            {
                return source.Substring(0, source.Length - suffixToRemove.Length);
            }
            else
            {
                return source;
            }
        }
    }
}
