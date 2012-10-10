using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.IO;
using CsQuery.HtmlParser;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Extension methods that support MVC integration
    /// </summary>

    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns the value of the "src" or "href" attribute, depending on the element type. If non
        /// applicable, returns empty string.
        /// </summary>
        ///
        /// <param name="obj">
        /// The obj to act on.
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>

        public static string UrlSource(this IDomObject obj)
        {
            switch (obj.NodeNameID)
            {
                case HtmlData.tagLINK:
                case HtmlData.tagA:
                    return obj.GetAttribute("href");
                case HtmlData.tagIMG:
                case HtmlData.tagSCRIPT:
                    return obj.GetAttribute("src");
                default:
                    return "";
            }   
        }

        /// <summary>
        /// Determine if any element in the list matches a predicate.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="list">
        /// The list to act on.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        ///
        /// <returns>
        /// true if the element is found, false if not.
        /// </returns>

        public static bool Any<T>(this IEnumerable list, Func<T,bool> predicate) 
        {
            foreach (var item in list)
            {
                if (item is T && predicate((T)item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine if any element in the list equals the item passed by parameter
        /// </summary>
        ///
        /// <param name="list">
        /// The list to act on.
        /// </param>
        /// <param name="match">
        /// The object to match
        /// </param>
        ///
        /// <returns>
        /// true if match is found in the list
        /// </returns>

        public static bool Any(this IEnumerable list, object match)
        {
            foreach (var item in list)
            {
                if (item.Equals(match))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Convert the sequence to a strongly-typed list. If an item cannot be cast as T, the list will
        /// contain a null entry.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="list">
        /// The list to act on.
        /// </param>
        ///
        /// <returns>
        /// The sequence as a new List&lt;T&gt;
        /// </returns>

        public static List<T> ToList<T>(this IEnumerable list) where T: class
        {
            List<T> output = new List<T>();
            foreach (var item in list)
            {
                T typedItem = item as T;
                output.Add(typedItem);
            }
            return output;
        }

        /// <summary>
        /// Return the part of the string after the last occurrence of find
        /// </summary>
        ///
        /// <param name="source">
        /// The source to act on.
        /// </param>
        /// <param name="find">
        /// The find to act on.
        /// </param>
        ///
        /// <returns>
        /// The part of the string after the last occurrence
        /// </returns>

        public static string AfterLast(this string source, string find)
        {
            int index = source.LastIndexOf(find);
            if (index > 0)
            {
                return source.Substring(index + find.Length);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Return the portion of a string before the first occurrence of "find", or the entire string if
        /// it's not found.
        /// </summary>
        ///
        /// <param name="source">
        /// The source to act on.
        /// </param>
        /// <param name="find">
        /// The find to act on.
        /// </param>
        ///
        /// <returns>
        /// The substring
        /// </returns>

        public static string Before(this string source, string find)
        {
            int index = source.IndexOf(find);
            if (index > 0)
            {
                return source.Substring(0,index);
            }
            else
            {
                return source;
            }
        }
    }
}
