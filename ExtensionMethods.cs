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

namespace Jtc.CsQuery
{
    public static class ExtensionMethods_Public
    {

        public static bool IsJson(this string text)
        {
            return text.StartsWith("{") && !text.StartsWith("{{");
        }
        public static bool IsTruthy(this object obj)
        {
            if (obj == null) return false;
            if (obj is string)
            {
                return !String.IsNullOrEmpty((string)obj);
            }
            if (obj is bool)
            {
                return (bool)obj;
            }
            if (obj is double || obj is float || obj is long || obj is int)
            {
                return Convert.ToDouble(obj) != 0;
            }
            return true;
        }
        public static object Clone(this object obj)
        {
            return obj.Clone(false);
        }
        public static object Clone(this object obj, bool deep)
        {
            if (obj.IsImmutable()) {
                return obj;
            }
            else if (obj is IEnumerable)
            {
                // captures expando objects too
                return ((IEnumerable)obj).CloneList(deep);
            }
            else
            {
                // TODO - check for existence of a "clone" method
                // convert regular objects to expando objects
                return (obj.ToExpando(true));
            }
        }
        /// <summary>
        /// Deep clone an enumerable. Deals with expando objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable CloneList(this IEnumerable obj)
        {
            return obj.CloneList(false);
        }
        public static IEnumerable CloneList(this IEnumerable obj,bool deep)
        {
            IEnumerable newList;
            // TODO - check for existence of a "clone" method
            //if (obj.GetType().IsArray)
            //{
            //    return (IEnumerable)((Array)obj).Clone();
            //} 
            if (obj.IsExpando())
            {
                newList = new ExpandoObject();
                var newListDict = (IDictionary<string, object>)newList;
                foreach (var kvp in ((IDictionary<string,object>)obj))
                {
                    newListDict.Add(kvp.Key, deep ? kvp.Value.Clone(true): kvp.Value);
                }
            }
            else
            {
                newList = new List<object>();
                foreach (var item in obj)
                {
                    ((List<object>)newList).Add(deep ? item.Clone(true): item);
                }
            }
            return newList;
        }
        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this object source)
        {
            return source.ToExpando(false);
        }
        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// If "deep" is true, child properties are cloned rather than referenced.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this object source, bool deep)
        {
            if (source.IsExpando() && !deep)
            {
                return (ExpandoObject)source;
            }
            else
            {
                return ToNewExpando(source, deep);
            }
        }
        private static ExpandoObject ToNewExpando(object source, bool deep)
        {
            if (source is string && ((string)source).IsJson())
            {
                source = Utility.JSON.ParseJSON((string)source);
            }
            else if (source is ExpandoObject)
            {
                return (ExpandoObject)source.Clone(deep);
            }
            else if (!source.IsExtendableType())
            {
                throw new Exception("Conversion to ExpandObject must be from a JSON string, an object, or an ExpandoObject");
            }

            ExpandoObject target = new ExpandoObject();
            IDictionary<string, object> targetDict = target;

            IEnumerable<MemberInfo> members = source.GetType().GetMembers();
            foreach (var member in members)
            {
                string name = member.Name;
                object value = null;
                if (member is PropertyInfo)
                {
                    value = ((PropertyInfo)member).GetGetMethod().Invoke(source, null);

                }
                else if (member is FieldInfo)
                {
                    value = ((FieldInfo)member).GetValue(source);
                }
                else
                {
                    continue;
                }
                targetDict[name] = deep ? value.Clone(true) : value;
            }
            return target;
        }
        /// <summary>
        /// Serailize the object to a JSON string
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(this object objectToSerialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (objectToSerialize is ExpandoObject)
            {
                return Flatten((ExpandoObject)objectToSerialize);
            }
            else
            {
                return (serializer.Serialize(objectToSerialize));
            }
        }
        /// <summary>
        /// Deserialize the JSON string to a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T FromJSON<T>(this string objectToDeserialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return (T)serializer.Deserialize(objectToDeserialize, typeof(T));
        }
        /// <summary>
        /// Deserialize the JSON string to an ExpandoObject or value type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object FromJSON(this string text)
        {
            return Utility.JSON.ParseJSON(text);
        }
        public static string Flatten(this ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();
            List<string> contents = new List<string>();
            var d = expando as IDictionary<string, object>;
            sb.Append("{");

            foreach (KeyValuePair<string, object> kvp in d)
            {
                contents.Add(String.Format("\"{0}\": {1}", kvp.Key,
                   serializer.Serialize(kvp.Value)));
            }
            sb.Append(String.Join(",", contents.ToArray()));

            sb.Append("}");

            return sb.ToString();
        }

        public static IEnumerable<KeyValuePair<string, object>> ToKvpList(this ExpandoObject obj)
        {
            return ((IDictionary<string, object>)obj).ToList();
        }
        /// <summary>
        /// Returns false if this is a value type, string, or enumerable type other than an Expando object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExtendableType(this object obj)
        {
            return obj.IsExpando() ||
                !(obj.IsImmutable() || obj is IEnumerable);
        }
        /// <summary>
        /// Only value types, strings, and null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsImmutable(this object obj)
        {
            return obj == null ||
                obj is string ||
                (obj is ValueType && !(obj.IsKeyValuePair()));
        }
        /// <summary>
        /// Test if is an expando object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExpando(this object obj)
        {
            return (obj is IDictionary<string, object>);
        }
        /// <summary>
        /// Returns true for expando objects with no properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEmptyExpando(this object obj)
        {
            return obj.IsExpando() && ((IDictionary<string,object>)obj).Count==0;
        }
        public static bool IsKeyValuePair(this object obj)
        {
            Type valueType = obj.GetType();
            if (valueType.IsGenericType)
            {
                Type baseType = valueType.GetGenericTypeDefinition();
                if (baseType == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
