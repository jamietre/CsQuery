﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using CsQuery;
using CsQuery.StringScanner;

namespace CsQuery.ExtensionMethods.Internal
{
    /// <summary>
    /// Extension methods used by CsQuery but not specialized enough to be considered useful for clients; therefore
    /// in a separate namespace.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Enums

        /// <summary>
        /// Returns true if the enum is any of the parameters in question.
        /// </summary>
        ///
        /// <param name="theEnum">
        /// The enum object
        /// </param>
        /// <param name="values">
        /// The values to test for
        /// </param>
        ///
        /// <returns>
        /// true if one of, false if not.
        /// </returns>
  

        public static bool IsOneOf(this Enum theEnum, params Enum[] values)
        {
            return values.Any(item => item.Equals(theEnum));

            //int i = values.Length;
            //while (--i>=0)
            //{
            //    if (theEnum.Equals(values[i]))
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
        public static bool IsOneOf(this string match, params string[] values)
        {
            return IsOneOf(match,true, values);
        }
        public static bool IsOneOf(this string match, bool matchCase=true, params string[] values )
        {
            return values.Any(item => match.Equals(item,
                matchCase ? StringComparison.CurrentCulture :
                StringComparison.CurrentCultureIgnoreCase));

            //int i = values.Length;
            //StringComparison comp = matchCase ? StringComparison.CurrentCulture: StringComparison.CurrentCultureIgnoreCase;
            //while (--i>=0)
            //{
            //    if (match.Equals(values[i],comp))
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }


        public static bool TryParse<T>(this Enum theEnum, string strType, out T result)
        {
            string strTypeFixed = strType.Replace(' ', '_');
            if (Enum.IsDefined(typeof(T), strTypeFixed))
            {
                result = (T)Enum.Parse(typeof(T), strTypeFixed, true);
                return true;
            }
            else
            {
                foreach (string value in Enum.GetNames(typeof(T)))
                {
                    if (value.Equals(strTypeFixed,
                                    StringComparison.OrdinalIgnoreCase))
                    {
                        result = (T)Enum.Parse(typeof(T), value);
                        return true;
                    }
                }
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Return the integer value for an enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Return the integer value cast as a string for an enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValueAsString(this Enum value)
        {
            return GetValue(value).ToString();
        }
       
        #endregion

        #region IEnumerable<T> extension methods

        

        /// <summary>
        /// Add all the items in a sequence to a collection.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type of the collections.
        /// </typeparam>
        /// <param name="target">
        /// The target collection
        /// </param>
        /// <param name="elements">
        /// The elements to add
        /// </param>

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> elements)
        {
            IList<T> elementsList = new List<T>(elements);
            foreach (T obj in elementsList)
            {
                target.Add(obj);
            }
        }

      

        /// <summary>
        /// Return true of a given collection is null or has no values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseList"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> baseList)
        {
            return baseList == null ||
                (baseList is ICollection<T> && ((ICollection<T>)baseList).Count==0) ||
                !baseList.Any();
        }

        /// <summary>
        /// Try to get the first element of a sequence. If the sequence is null or has no elements, return false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseList"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static bool TryGetFirst<T>(this IEnumerable<T> baseList, out T firstElement)
        {
            if (baseList == null)
            {
                firstElement = default(T);
                return false;
            }
            bool result = false;
            // I think this is the most efficient way to verify an empty IEnumerable
            firstElement = default(T);
            foreach (T obj in baseList)
            {
                result = true;
                firstElement = obj;
                break;
            }
            return result;
        }

        /// <summary>
        /// Return a single element or the default value; does not fail on >1 value but also returns the default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T SingleOrDefaultAlways<T>(this IEnumerable<T> list)
        {
            T single = default(T);
            bool found = false;
            foreach (var item in list)
            {
                if (!found)
                {
                    single = item;
                    found = true;
                }
                else
                {
                    return default(T);
                }
            }
            return single;
        }

     

       

        #endregion

        #region string methods

        public static int OccurrencesOf(this string text, char find) {
            int pos=0;
            int count = 0;
            while ((pos = text.IndexOf(find,pos))>=0)
            {
                count++;
                pos++;
            }
            return count;
        }

        /// <summary>
        /// Given a string that repesents a list demarcated by separator, add a new value to it
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ListAdd(this string list, string value, string separator)
        {
            if (String.IsNullOrEmpty(value))
            {
                return list.Trim();
            }
            if (list == null)
            {
                list = String.Empty;
            }
            else
            {
                list = list.Trim();
            }

            int pos = (list + separator).IndexOf(value + separator);
            if (pos < 0)
            {
                if (list.LastIndexOf(separator) == list.Length - separator.Length)
                {
                    // do not add separator - it already exists
                    return list + value;
                }
                else
                {
                    return (list + (list == "" ? "" : separator) + value);
                }
            }
            else
            {
                // already has value
                return (list);
            }
        }

        /// <summary>
        /// Given a string that is a list demarcated by separator, remove a value from it
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ListRemove(this string list, string value, string separator)
        {
            string result = (separator + list).Replace(separator + value, "");
            if (result.Substring(0, 1) == separator)
            {
                result = result.Remove(0, 1);
            }
            return (result);
        }

        /// <summary>
        /// Returns the text between startIndex and endIndex (exclusive of endIndex)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string SubstringBetween(this string text, int startIndex, int endIndex)
        {
            if (endIndex > text.Length || endIndex <0) {
                return "";
            }
            return (text.Substring(startIndex, endIndex - startIndex));
        }


        /// <summary>
        /// Remove all whitespace from a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string text)
        {
            return text == null ? null : Regex.Replace(text, @"\s+", " ");
        }

        /// <summary>
        /// Returns the part of the string before the last occurence of the operand.
        /// </summary>
        ///
        /// <param name="text">
        /// The text on which to perate
        /// </param>
        /// <param name="find">
        /// The text to find.
        /// </param>
        ///
        /// <returns>
        /// The portion of the text before the beginning of the last occurence of "find"
        /// </returns>

        public static string BeforeLast(this string text, string find)
        {
            int index = text.LastIndexOf(find);
            if (index >= 0)
            {
                return (text.Substring(0, index));
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the string after the end of the first occurrence of "find".
        /// </summary>
        ///
        /// <param name="text">
        /// The text to search within
        /// </param>
        /// <param name="find">
        /// The text to find.
        /// </param>
        ///
        /// <returns>
        /// The portion of the text after the first occurence of "find"
        /// </returns>

        public static string After(this string text, string find)
        {
            int index = text.IndexOf(find);
            if (index < 0 || index + find.Length >= text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(index + find.Length));
            }
        }

        /// <summary>
        /// Return the part of the string that is after the last occurrence of the operand
        /// </summary>
        /// <param name="text">The source string</param>
        /// <param name="find">The text to find</param>
        /// <returns></returns>
        public static string AfterLast(this string text, string find)
        {
            int index = text.LastIndexOf(find);
            if (index < 0 || index + find.Length >= text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(index + find.Length));

            }
        }

        /// <summary>
        /// Return the part of a string that is before the first occurrence of the operand
        /// </summary>
        /// <param name="text">The source string</param>
        /// <param name="find">The text to find</param>
        /// <returns></returns>
        public static string Before(this string text, string find)
        {
            int index = text.IndexOf(find);
            if (index < 0 || index == text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(0, index));
            }
        }

        /// <summary>
        /// Clean a string by converts null to an empty string and trimming any whitespace from the beginning and end
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanUp(this string value)
        {
            return (value ?? String.Empty).Trim();
        }



        /// <summary>
        /// Perform a string split using whitespace demarcators (' ', tab, newline, return) and trimming each result
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitClean(this string text)
        {
            return SplitClean(text, CharacterData.charsHtmlSpaceArray);
        }

        /// <summary>
        /// Perform a string split that also trims whitespace from each result and removes duplicats
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitClean(this string text, char separator)
        {
            char[] sep = new char[1];
            sep[0] = separator;
            return SplitClean(text, sep);
        }

        /// <summary>
        /// Perform a string split that also trims whitespace from each result and removes duplicats
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitClean(this string text, char[] separator)
        {

            string[] list = (text ?? "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (list.Length > 0)
            {
                HashSet<string> UniqueList = new HashSet<string>();
                for (int i = 0; i < list.Length; i++)
                {
                    if (UniqueList.Add(list[i]))
                    {
                        yield return list[i].Trim();
                    }
                }
            }
            yield break;
        }

        #endregion

        #region char and char array methods 

        /// <summary>
        /// Return a substring from a character array starting at the startIndex provided of the
        /// specified length.
        /// </summary>
        ///
        /// <param name="text">
        /// The source text.
        /// </param>
        /// <param name="startIndex">
        /// The starting index.
        /// </param>
        /// <param name="length">
        /// The length of the substring to extract.
        /// </param>
        ///
        /// <returns>
        /// The substring.
        /// </returns>

        public static string Substring(this char[] text, int startIndex, int length)
        {
            var sb = new StringBuilder(length);
            sb.Append(text, startIndex, length);
            return sb.ToString();

        }

        /// <summary>
        /// Return a substring from a character array from the starting index to the end of the array.
        /// </summary>
        ///
        /// <param name="text">
        /// The source text.
        /// </param>
        /// <param name="startIndex">
        /// The starting index.
        /// </param>
        ///
        /// <returns>
        /// The substring.
        /// </returns>

        public static string Substring(this char[] text, int startIndex)
        {
            int len = text.Length - startIndex;
            var sb = new StringBuilder(len);
            sb.Append(text, startIndex,len);
            return sb.ToString();

        }

        /// <summary>
        /// Return the position of the first occurrence of a string in a character array.
        /// </summary>
        ///
        /// <param name="text">
        /// The text to seek within
        /// </param>
        /// <param name="seek">
        /// The tarket
        /// </param>
        ///
        /// <returns>
        /// The index of the target, or -1 if not found
        /// </returns>

        public static int Seek(this char[] text, string seek)
        {
            return Seek(text, seek, 0);
        }

        /// <summary>
        /// Return the position of the first occurrence of a string in a character array that is on or
        /// after startIndex.
        /// </summary>
        ///
        /// <param name="text">
        /// .
        /// </param>
        /// <param name="seek">
        /// .
        /// </param>
        /// <param name="startIndex">
        /// .
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static int Seek(this char[] text, string seek, int startIndex)
        {
            int nextPos =startIndex;

            char firstChar = seek[0];
            while (nextPos >= 0)
            {
                nextPos = Array.IndexOf<char>(text, firstChar, nextPos);
                if (nextPos > 0)
                {
                    bool match = true;
                    for (int i = 0; i < seek.Length; i++)
                    {
                        if (text[nextPos + i] != seek[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return nextPos;
                    }
                    else
                    {
                        nextPos++;
                    }
                }  
            }
            return -1;
        }

        /// <summary>
        /// Convert a single character to lower case
        /// </summary>
        ///
        /// <param name="character">
        /// The character to act on.
        /// </param>
        ///
        /// <returns>
        /// The lowercased character
        /// </returns>

        public static char ToLower(this char character)
        {
            if (character >= 'A' && character <= 'Z')
            {
                return (char)(character + 32);
            }
            else
            {
                return character;
            }
        }

        /// <summary>
        /// Convert a single character to upper case
        /// </summary>
        ///
        /// <param name="character">
        /// The character to act on.
        /// </param>
        ///
        /// <returns>
        /// The uppercased character
        /// </returns>

        public static char ToUpper(this char character)
        {
            if (character >= 'a' && character <= 'z')
            {
                return (char)(character - 32);
            }
            else
            {
                return character;
            }
        }

        #endregion

        #region Array methods

        /// <summary>
        /// Return the index of item in an array. If count is > 0 then that is considered the length of
        /// the array.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// .
        /// </typeparam>
        /// <param name="arr">
        /// .
        /// </param>
        /// <param name="item">
        /// .
        /// </param>
        /// <param name="count">
        /// .
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static int IndexOf<T>(this T[] arr, T item, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if ((arr[i] == null && item == null) || arr[i].Equals(item))
                {
                    return i;
                }
            }
            return -1;

        }
        #endregion

        #region miscellaneous methods

        /// <summary>
        /// Return the default StringComparer class that implements the behavior for a StringComparison
        /// </summary>
        ///
        /// <param name="comparison">
        /// The comparison to act on.
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static StringComparer ComparerFor(this StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default:
                    throw new NotImplementedException("Unknown StringComparer enum value");
            }
        }

        /// <summary>
        /// (Alpha) Clone a sequence of objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable CloneList(this IEnumerable obj)
        {
            return obj.CloneList(false);
        }
        /// <summary>
        /// (Alpha) Deep clone a sequence of objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public static IEnumerable CloneList(this IEnumerable obj, bool deep)
        {
            IEnumerable newList;
            // TODO - check for existence of a "clone" method
            //if (obj.GetType().IsArray)
            //{
            //    return (IEnumerable)((Array)obj).Clone();
            //} 
            if (Objects.IsExpando(obj))
            {
                newList = new JsObject();
                var newListDict = (IDictionary<string, object>)newList;
                foreach (var kvp in ((IDictionary<string, object>)obj))
                {
                    newListDict.Add(kvp.Key, deep ? Objects.CloneObject(kvp.Value, true) : kvp.Value);
                }
            }
            else
            {
                newList = new List<object>();
                foreach (var item in obj)
                {
                    ((List<object>)newList).Add(deep ? Objects.CloneObject(item, true) : item);
                }
            }
            return newList;
        }

        #endregion
    }
}
