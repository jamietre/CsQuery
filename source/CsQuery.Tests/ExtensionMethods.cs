using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;

namespace CsQuery.Tests
{
    public static class TestExtensionMethods
    {
        public static string ElementText(this IDomObject element)
        {
            StringBuilder sb = new StringBuilder();
            if (element.HasChildren)
            {
                foreach (var child in element.ChildNodes.Where(item=>item.NodeType == NodeType.TEXT_NODE))
                {
                    sb.Append(child.NodeValue);
                }
            }
            return sb.ToString();
        }

        public static string AsString(this ushort[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var sh in array)
            {
                sb.Append((char)sh);
            }
            return sb.ToString();
        }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return true;
            return baseList.IsEmpty();
        }
        public static bool IsEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return false;
            bool result = true;
            // I think this is the most efficient way to verify an empty IEnumerable
            foreach (T t in baseList)
            {
                result = false;
                break;
            }
            return (result);
        }
        public static IEnumerable<string> NodeNames(this IEnumerable<IDomObject> cq)
        {
            return cq.Select(item => item.NodeName).ToList();

        }

        /// <summary>
        /// A string extension method that normalize line endings to the local environment
        /// </summary>
        ///
        /// <param name="text">
        /// The text to act on.
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>

        public static string NormalizeLineEndings(this string text)
        {
            return text
                .Replace("\r", "")
                .Replace("\n", System.Environment.NewLine);

        }
    }
}
