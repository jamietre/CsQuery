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
    public static class PublicExtensionMethods
    {
        #region IEnumerable<T> extension methods

        public static void AddRange<T>(this ICollection<T> baseList, IEnumerable<T> list)
        {
            foreach (T obj in list)
            {
                baseList.Add(obj);
            }
        }

        #endregion

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
                    newListDict.Add(kvp.Key, deep ? Objects.CloneObject(kvp.Value,true) : kvp.Value);
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
        public static bool HasProperty(this ExpandoObject obj, string propertyName)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(propertyName);
        }
        /// <summary>
        /// Return a yped value from an ExpandoObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(this ExpandoObject obj, string name)
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
    }
    
}
