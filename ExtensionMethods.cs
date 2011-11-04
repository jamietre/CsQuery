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

using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Utility;

namespace Jtc.CsQuery
{


    public static class ExtensionMethods_Public
    {
        /// <summary>
        /// Returns true if the string appears to be JSON.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsJson(this string text)
        {
            return text.StartsWith("{") && !text.StartsWith("{{");
        }
        /// <summary>
        /// Returns true if the object is a primitive numeric type, e.g. exluding string & char
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumericType(this object obj)
        {

            Type t = Objects.GetUnderlyingType(obj.GetType());
            return t.IsPrimitive && !(t == typeof(string) || t == typeof(char));
        

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
        public static bool IsTruthy(this object obj)
        {
            return Objects.IsTruthy(obj);
        }
        public static object CloneObject(this object obj)
        {
            return obj.CloneObject(false);
        }
        public static object CloneObject(this object obj, bool deep)
        {
            if (obj.IsImmutable())
            {
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
        public static IEnumerable CloneList(this IEnumerable obj, bool deep)
        {
            IEnumerable newList;
            // TODO - check for existence of a "clone" method
            //if (obj.GetType().IsArray)
            //{
            //    return (IEnumerable)((Array)obj).Clone();
            //} 
            if (obj.IsExpando())
            {
                newList = new JsObject();
                var newListDict = (IDictionary<string, object>)newList;
                foreach (var kvp in ((IDictionary<string, object>)obj))
                {
                    newListDict.Add(kvp.Key, deep ? kvp.Value.CloneObject(true) : kvp.Value);
                }
            }
            else
            {
                newList = new List<object>();
                foreach (var item in obj)
                {
                    ((List<object>)newList).Add(deep ? item.CloneObject(true) : item);
                }
            }
            return newList;
        }
        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JsObject ToExpando(this object source)
        {
            return source.ToExpando(false);
        }
        public static T ToExpando<T>(this object source) where T : IDynamicMetaObjectProvider, new()
        {
            return source.ToExpando<T>(false);
        }
        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// If "deep" is true, child properties are cloned rather than referenced.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JsObject ToExpando(this object source, bool deep)
        {
            return ToExpando<JsObject>(source, deep);
        }
        public static T ToExpando<T>(this object source, bool deep) where T : IDynamicMetaObjectProvider, new()
        {
            return ToExpando<T>(source, deep, new Type[] { });
        }
        public static T ToExpando<T>(this object source, bool deep, IEnumerable<Type> ignoreAttributes) where T : IDynamicMetaObjectProvider, new()
        {
            if (source.IsExpando() && !deep)
            {
                return Objects.Dict2Dynamic<T>((IDictionary<string, object>)source);
            }
            else
            {
                return ToNewExpando<T>(source, deep, ignoreAttributes);
            }
        }
        private static T ToNewExpando<T>(object source, bool deep,IEnumerable<Type> ignoreAttributes) where T: IDynamicMetaObjectProvider, new()
        {
            if (source == null)
            {
                return default(T);
            }
            HashSet<Type> IgnoreList = new HashSet<Type>(ignoreAttributes);

            if (source is string && ((string)source).IsJson())
            {
                source = Utility.JSON.ParseJSON((string)source);
            }
            else if (source.IsExpando())
            {
                return (T)source.CloneObject(deep);
            }
            else if (!source.IsExtendableType())
            {
                throw new Exception("Conversion to ExpandObject must be from a JSON string, an object, or an ExpandoObject");
            }

            T target = new T();
            IDictionary<string, object> targetDict = ( IDictionary<string, object>)target;

            IEnumerable<MemberInfo> members = source.GetType().GetMembers();
            foreach (var member in members)
            {

                foreach (object attrObj in member.GetCustomAttributes(false))
                {
                    Attribute attr = (Attribute)attrObj;
                    if (IgnoreList.Contains(attr.GetType())) {
                        goto NextAttribute;
                    }
                }
                string name = member.Name;


                object value = null;
                bool skip = false;
                if (member is PropertyInfo && ((PropertyInfo)member).GetIndexParameters().Length == 0)
                {
                    // wrap this because we are testing every single property - if it doesn't work we don't want to use it
                    try
                    {
                        value = ((PropertyInfo)member).GetGetMethod().Invoke(source, null);
                    }
                    catch
                    {
                        skip = true;
                    }

                }
                else if (member is FieldInfo)
                {
                    value = ((FieldInfo)member).GetValue(source);
                }
                else
                {
                    continue;
                }
                if (!skip)
                {
                    targetDict[name] = deep ? value.CloneObject(true) : value;
                }
            NextAttribute: { }
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
        /// Convert an expandoobject to a list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKvpList(this ExpandoObject obj)
        {
            var dict = ((IDictionary<string, object>)obj);
            return dict == null ? Objects.EmptyEnumerable<KeyValuePair<string, object>>() : dict.ToList();
        }
        public static bool HasProperty(this ExpandoObject obj, string propertyName)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(propertyName);
        }
        /// <summary>
        /// Return typed value from an expandoobject
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
        public static JsObject Get(this ExpandoObject obj, string name)
        {
            IDictionary<string, object> dict = obj;
            object subProp;
            if (dict.TryGetValue(name, out subProp))
            {
                return CsQuery.ToExpando(subProp);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Returns false if this is a value type, null string, or enumerable (but not Extendable)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExtendableType(this object obj)
        {
            //return obj.IsExpando() || (!obj.IsImmutable() && !(obj is IEnumerable));
            // Want to allow enumerable types since we can treat them as objects. Exclude arrays.
            // This is tricky. How do we know if something should be treated as an object or enumerated? Do both somehow?
            return obj.IsExpando() || (!obj.IsImmutable() && !(obj is IEnumerable));
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
            return (obj is IDictionary<string, object>) ;
        }
        /// <summary>
        /// Returns true for expando objects with no properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEmptyExpando(this object obj)
        {
            return obj.IsExpando() && ((IDictionary<string, object>)obj).Count == 0;
        }
       
    }
    
}
