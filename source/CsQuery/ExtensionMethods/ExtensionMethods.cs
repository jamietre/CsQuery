using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text;
using System.Reflection;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.ExtensionMethods
{
    /// <summary>
    /// Some extension methods that come in handy when working with CsQuery
    /// </summary>
    public static class ExtensionMethods
    {
       

        #region string extension methods
        
        public static String RegexReplace(this String input, string pattern, string replacements)
        {
            return input.RegexReplace(Objects.Enumerate(pattern), Objects.Enumerate(replacements));
        }

        public static String RegexReplace(this String input, IEnumerable<string> patterns, IEnumerable<string> replacements)
        {
            List<string> patternList = new List<string>(patterns);
            List<string> replacementList = new List<string>(replacements);
            if (replacementList.Count != patternList.Count)
            {
                throw new ArgumentException("Mismatched pattern and replacement lists.");
            }

            for (var i = 0; i < patternList.Count; i++)
            {
                input = Regex.Replace(input, patternList[i], replacementList[i]);
            }

            return input;
        }

        public static string RegexReplace(this String input, string pattern, MatchEvaluator evaluator)
        {

            return Regex.Replace(input, pattern, evaluator);
        }
        /// <summary>
        /// Test whether the regular expression pattern matches the string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool RegexTest(this String input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }
        /// <summary>
        /// Returns true when a value is "truthy" using similar logic as Javascript
        ///   null = false
        ///   empty string = false BUT zero string = true
        ///   zero numeric = false
        ///   false boolean values = false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        #endregion

        /// <summary>
        /// Clone a sequence of elements to a new sequence
        /// </summary>
        ///
        /// <param name="source">
        /// The source sequence
        /// </param>
        ///
        /// <returns>
        /// A sequence containing a clone of each element in the source.
        /// </returns>

        public static IEnumerable<IDomObject> Clone(this IEnumerable<IDomObject> source)
        {
            foreach (IDomObject item in source)
            {
                yield return item.Clone();
            }

        }
        /// <summary>
        /// Serailize the object to a JSON string
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(this object objectToSerialize)
        {
            return JSON.ToJSON(objectToSerialize);
        }
        /// <summary>
        /// Deserialize the JSON string to a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T ParseJSON<T>(this string objectToDeserialize)
        {
            return JSON.ParseJSON<T>(objectToDeserialize);
        }
        /// <summary>
        /// Deserialize the JSON string to an ExpandoObject or value type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSON(this string text)
        {
            return JSON.ParseJSON(text);
        }

        /// <summary>
        /// Indicates whether a property exists on an ExpandoObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this DynamicObject obj, string propertyName)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(propertyName);
        }

        /// <summary>
        /// Return a typed value from a dynamic object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(this DynamicObject obj, string name)
        {
            if (obj == null)
            {
                return default(T);
            }
            var dict = (IDictionary<string, object>)obj;
            object val;
            if (dict.TryGetValue(name, out val))
            {
                return Objects.Convert<T>(val);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Reduce the set of matched elements to a subset beginning with the 0-based index provided.
        /// </summary>
        ///
        /// <param name="array">
        /// The array to act on.
        /// </param>
        /// <param name="start">
        /// The 0-based index at which to begin selecting.
        /// </param>
        /// <param name="end">
        /// The 0-based index of the element at which to stop selecting. The actual element at this
        /// position is not included in the result.
        /// </param>
        ///
        /// <returns>
        /// A new array of the same type as the original.
        /// </returns>

        public static Array Slice(this Array array, int start, int end)
        {
            // handle negative values

            if (start < 0)
            {
                start = array.Length + start;
                if (start < 0) { start = 0; }
            }
            if (end < 0)
            {
                end = array.Length + end;
                if (end < 0) { end = 0; }
            }
            if (end >= array.Length)
            {
                end = array.Length;
            }


            int length = end - start;

            Type arrayType = array.GetType().GetElementType();
            Array output =  Array.CreateInstance(arrayType,length);

            int newIndex = 0;
            for (int i=start;i<end;i++) {
                output.SetValue(array.GetValue(i), newIndex++);
            }

            return output;
        
        }

        /// <summary>
        /// Reduce the set of matched elements to a subset beginning with the 0-based index provided.
        /// </summary>
        ///
        /// <param name="array">
        /// The array to act on.
        /// </param>
        /// <param name="start">
        /// The 0-based index at which to begin selecting.
        /// </param>
        ///
        /// <returns>
        /// A new array of the same type as the original.
        /// </returns>

        public static Array Slice(this Array array, int start)
        {
            return Slice(array, start, array.Length);
        }
    }
    
}
