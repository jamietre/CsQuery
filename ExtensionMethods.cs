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
using System.ComponentModel;

using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery.Utility;

namespace Jtc.CsQuery
{
    //TODO: need to write a better json parsing method that converts [{Key: 'x', Value: 'y'}] to ExpandoObject
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
                return System.Convert.ToDouble(obj) != 0;
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
                if (member is PropertyInfo && ((PropertyInfo)member).GetIndexParameters().Length==0)
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
        /// <summary>
        /// Convert an expandoobject to a list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKvpList(this ExpandoObject obj)
        {
            var dict = ((IDictionary<string, object>)obj);
            return dict==null ? Objects.EmptyEnumerable<KeyValuePair<string,object>>() : dict.ToList();
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
                return Convert<T>(val);
            }
            else
            {
                return default(T);
            }
        }
        public static ExpandoObject Get(this ExpandoObject obj, string name)
        {
            IDictionary<string,object> dict= obj;
            object subProp;
            if (dict.TryGetValue(name,out subProp)) {
                return CsQuery.ToExpando(subProp);
            } else {
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

        public static T Convert<T>(object value)
        {
            T output;
            if (!TryConvert<T>(value, out output))
            {
                throw new Exception("Unable to convert to type " + typeof(T).ToString());
            }
            return output;
        }
        public static object Convert(object value, Type type)
        {
            object output;
            if (!TryConvert(value, out output, type, DefaultValue(type)))
            {
                throw new Exception("Unable to convert to type " + type.ToString());
            }
            return output;
        }
        public static T Convert<T>(object value, T defaultValue)
        {
            T output;
            if (!TryConvert<T>(value, out output))
            {
                output = defaultValue;
            }
            return output;
        }
        public static bool TryConvert<T>(object value, out T typedValue)
        {
            object interimValue;
            bool result = TryConvert(value, out interimValue, typeof(T), default(T));
            if (result)
            {
                typedValue = (T)interimValue;
            }
            else
            {
                typedValue = default(T);
            }
            return result;
        }

        public static bool TryConvert(object value, out object typedValue, Type type, object defaultValue)
        {

            object output = defaultValue;
            bool success = false;
            Type realType;
            string stringVal = value == null ? String.Empty : value.ToString().ToLower().Trim();
            if (type == typeof(string))
            {
                typedValue = value == null ? null : value.ToString();
                return true;
            }
            else if (IsNullableType(type))
            {
                if (stringVal == String.Empty)
                {
                    typedValue = null;
                    return true;
                }
                realType = GetUnderlyingType(type);
            }
            else
            {
                if (stringVal == String.Empty)
                {
                    typedValue = DefaultValue(type);
                    return false;
                }
                realType = type;
            }


            if (realType == value.GetType())
            {
                output = value;
                success = true;
            }
            else if (realType == typeof(bool))
            {
                switch (stringVal)
                {
                    case "on":
                    case "yes":
                    case "true":
                    case "enabled":
                    case "active":
                    case "1":
                        output = true;
                        success = true;
                        break;
                    case "off":
                    case "no":
                    case "false":
                    case "disabled":
                    case "0":
                        output = false;
                        success = true;
                        break;
                }
            }
            else if (realType.IsEnum)
            {
                output = Enum.Parse(realType, stringVal);
                success = true;
            }
            else if (realType == typeof(int)
                || realType == typeof(long)
                || realType == typeof(float)
                || realType == typeof(double)
                || realType == typeof(decimal))
            {
                object val;

                if (TryParseNumber(stringVal, out val, realType))
                {
                    output = val;
                    success = true;
                }
            }
            else if (realType == typeof(DateTime))
            {
                DateTime val;
                if (DateTime.TryParse(stringVal, out val))
                {
                    output = val;
                    success = true;
                }
            }
            else if (realType == typeof(string))
            {
                output = value;
            }
            else
            {
                throw new Exception("Don't know how to convert type " + type.UnderlyingSystemType.ToString());
            }
            if (output != null && output.GetType() != realType)
            {
                typedValue = System.Convert.ChangeType(output, realType);
            }
            else
            {
                typedValue = output;
            }
            return success;
        }

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>An object whose Type is conversionType (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to value. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.</returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// </remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if (value == null)
                {
                    return null;
                } // end if

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            } // end if

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return System.Convert.ChangeType(value, conversionType);
        }
        public static bool TryParseNumber(string value, out object number, Type T)
        {
            double val;
            number = 0;
            if (double.TryParse(value, out val))
            {
                if (T == typeof(int))
                {
                    number = System.Convert.ToInt32(Math.Round(val));
                }
                else if (T == typeof(long))
                {
                    number = System.Convert.ToInt64(Math.Round(val));
                }
                else if (T == typeof(double))
                {
                    number = System.Convert.ToDouble(val);
                }
                else if (T == typeof(decimal))
                {
                    number = System.Convert.ToDecimal(val);
                }
                else if (T == typeof(float))
                {
                    number = System.Convert.ToSingle(val);
                }
                else
                {
                    throw new Exception("Unhandled type for TryParseNumber: " + T.GetType().ToString());
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsNullableType(Type type)
        {
            return type == typeof(string) ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        public static object DefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        public static Type GetUnderlyingType(Type type)
        {

            if (type != typeof(string) && IsNullableType(type))
            {
                return Nullable.GetUnderlyingType(type);
            }
            else
            {
                return type;
            }

        }

    }
}
