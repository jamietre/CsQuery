using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the index of a characer starting at the specified position
        /// </summary>
        ///
        /// <param name="charArray">
        /// The character array to act on.
        /// </param>
        /// <param name="target">
        /// The character to seek
        /// </param>
        /// <param name="start">
        /// The starting index
        /// </param>
        ///
        /// <returns>
        /// The index at which the target is found
        /// </returns>

        public static int CharIndexOf(this char[] charArray, char target, int start)
        {
            //return Array.IndexOf<char>(charArray, seek, start);

            // This is faster than Array.IndexOf, appeared cut down load time by about 10% on text heavy dom test

            int pos = start;
            int end = charArray.Length;
            while (pos < end && charArray[pos++] != target)
                ;
            return pos == end && charArray[end - 1] != target ? -1 : pos - 1;

        }

        /// <summary>
        /// Returns the string representation of the characters between startIndex and endIndex
        /// (exclusive of endIndex)
        /// </summary>
        ///
        /// <param name="text">
        /// The character array.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        ///
        /// <returns>
        /// The string between the two indices.
        /// </returns>

        public static string SubstringBetween(this char[] text, int startIndex, int endIndex)
        {
            int len = endIndex-startIndex;

            var sb = new StringBuilder(len);
            sb.Append(text, startIndex, len);
            return sb.ToString();
        }

        /// <summary>
        /// Converts a character array to a string.
        /// </summary>
        ///
        /// <param name="text">
        /// The character array
        /// </param>
        ///
        /// <returns>
        /// A string of the sequence of characters
        /// </returns>

        public static string AsString(this char[] text)
        {
            return new string(text);

        }

     
    }
}
