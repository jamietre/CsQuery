using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    public static class ExtensionMethods
    {
        public static int CharIndexOf(this char[] charArray, char seek, int start)
        {
            //return Array.IndexOf<char>(charArray, seek, start);


            //int pos = start - 1;
            //int end = charArray.Length;
            //while (++pos < end && charArray[pos] != seek)
            //    ;
            //return pos == end ? -1 : pos;

            // This is substantially faster than Array.IndexOf, cut down load time by about 10% on text heavy dom test

            int pos = start;
            int end = charArray.Length;
            while (pos < end && charArray[pos++] != seek)
                ;
            return pos == end && charArray[end - 1] != seek ? -1 : pos - 1;

        }

        public static string SubstringBetween(this char[] text, int startIndex, int endIndex)
        {
            int len = endIndex - startIndex + 1;
            string result = "";
            for (int i = startIndex; i < endIndex; i++)
            {
                result += text[i];
            }
            return result;
        }
        public static string AsString(this char[] text)
        {
            return String.Join("", text);

        }

     
    }
}
