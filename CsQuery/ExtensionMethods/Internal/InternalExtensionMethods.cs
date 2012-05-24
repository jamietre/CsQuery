using System;
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

namespace CsQuery.ExtensionMethods.Internal
{
    public static class InternalExtensionMethods
    {
        #region object extension methods
        /// <summary>
        /// Only value types, strings, and null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsImmutable(this object obj)
        {
            return obj == null ||
                obj == System.DBNull.Value || 
                obj is string ||
                (obj is ValueType && !(Objects.IsKeyValuePair(obj)));
        }
        /// <summary>
        /// Returns false if this is a value type, null string, or enumerable (but not Extendable)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExtendableType(this object obj)
        {
            // Want to allow enumerable types since we can treat them as objects. Exclude arrays.
            // This is tricky. How do we know if something should be treated as an object or enumerated? Do both somehow?
            return Objects.IsExpando(obj) || (!obj.IsImmutable() && !(obj is IEnumerable));
        }
        #endregion
        #region Enums
        /// <summary>
        /// Returns true if the enum is any of the parameters in question
        /// </summary>
        /// <param name="theEnum"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <returns></returns>
        public static bool IsOneOf(this Enum theEnum, params Enum[] values)
        {
    
            int i = values.Length;
            while (--i>=0)
            {
                if (theEnum.Equals(values[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOneOf(this string match, params string[] values)
        {
            return IsOneOf(match,true, values);
        }
        public static bool IsOneOf(this string match, bool matchCase=true, params string[] values )
        {
           
            int i = values.Length;
            StringComparison comp = matchCase ? StringComparison.CurrentCulture: StringComparison.CurrentCultureIgnoreCase;
            while (--i>=0)
            {
                if (match.Equals(values[i],comp))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


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
        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static string GetValueAsString(this Enum value)
        {
            return GetValue(value).ToString();
        }
       

        public static bool IsNullOrEmpty<T>(this ICollection<T> baseList)
        {
            return (baseList == null ||
                baseList.Count == 0);

        }
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
        public static string SubstringBetween(this char[] text, int startIndex, int endIndex)
        {
            int len = endIndex - startIndex + 1;
            string result="";
            for (int i = startIndex; i < endIndex; i++)
            {
                result += text[i];
            }
            return result;
        }
        public static string Substring(this char[] text, int startIndex, int length)
        {
            
            string result = "";
            for (int i = startIndex; i <startIndex+ length; i++)
            {
                result += text[i];
            }
            return result;
        }
        public static int Seek(this char[] text, string seek)
        {
            return Seek(text, seek, 0);
        }
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

        public static string RemoveWhitespace(this string text)
        {
            return text == null ? null : Regex.Replace(text, @"\s+", " ");
        }
        /// <summary>
        /// Returns the part of the string before the last occurence of text
        /// </summary>
        /// <param name="?"></param>
        /// <param name="text"></param>
        /// <returns></returns>
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
        /// Given a relative path, locates a file in the parent heirarchy by matching parts of the path
        /// </summary>
        /// <param name="text"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string FindPathTo(this string currentRootedPath, string find)
        {
            List<string> rootedPath = new List<string>(currentRootedPath.ToLower().Split('\\'));
            List<string> findPath =  new List<string>(find.ToLower().Split('\\'));

            int start = rootedPath.IndexOf(findPath[0]);
            if (start<0) {
                return "";
            } else {
                int i=0;
                while (rootedPath[++start] == findPath[++i])
                    ;

                return string.Join("\\",rootedPath.GetRange(0,start)) + "\\" 
                    + string.Join("\\",findPath.GetRange(i,findPath.Count-i));

            }
        }
        /// <summary>
        /// Returns the string after the end of the first occurrence of "find"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="text"></param>
        /// <returns></returns>
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
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return true;
            
            foreach (T t in baseList)
            {
                return false;
            }
            return true;
        }

        public static bool TryGetFirst<T>(this IEnumerable<T> baseList, out T firstElement)
        {
            if (baseList == null)
            {
                firstElement = default(T);
                return false;
            }
            bool result = false;
            // I think this is the most efficient way to verify an empty IEnumerable
            firstElement=default(T);
            foreach (T obj in baseList)
            {
                result = true;
                firstElement = obj;
                break;
            }
            return result;
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> func)
        {
            foreach (T obj in list)
            {
                func(obj);
            }
        }
        /// <summary>
        /// Converts null to String.Empty and trims
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanUp(this string value)
        {
            return (value ?? String.Empty).Trim();
        }
        private static char[] stringSep = new char[] {' ','\n','\t', '\r'};
        public static IEnumerable<string> SplitClean(this string text)
        {
            return SplitClean(text, stringSep);
        }
        public static IEnumerable<string> SplitClean(this string text, char separator)
        {
            char[] sep = new char[1];
            sep[0]=separator;
            return SplitClean(text, sep);
        }
        /// <summary>
        /// Splits a list, removes empty entries; trims; removes dups
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitClean(this string text, char[] separator)
        {
            
            string[] list = (text ?? "").Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (list.Length>0) {
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

        public static string IfNullOrEmpty(this string value, string alternate)
        {
            return value.IsNullOrEmpty() ? alternate : value;
        }
        public static string IfNull(this string value, string alternate)
        {
            return value==null ? alternate : value;
        }
        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action) where T: class
        {
            foreach (T obj in list) {
                action(obj);
            }
            return list;
        }
        public static IEnumerable<T> DoWhen<T>(this IEnumerable<T> list, Action<T> action, Func<T,bool> condition) where T : class
        {
            foreach (T obj in list)
            {
                if (condition(obj)) action(obj);
            }
            return list;
        }
        public static string DoBuildString<T>(this IEnumerable<T> list, Func<T, string> action)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T obj in list)
            {
                sb.Append(action(obj));
            }
            return sb.ToString();
        }
    }
}
