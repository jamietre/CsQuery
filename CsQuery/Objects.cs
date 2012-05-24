using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Web.Script.Serialization;
using System.ComponentModel;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery
{
    public static class Objects
    {
        static Objects()
        {
            IgnorePropertyNames = new HashSet<string>();
            var info = typeof(object).GetMembers();
            foreach (var member in info)
            {
                IgnorePropertyNames.Add(member.Name);

            }

        }
        static HashSet<string> IgnorePropertyNames;

        /// <summary>
        /// Returns true if the string appears to be JSON.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsJson(object obj)
        {
            string text = obj as string;
            return text != null && text.StartsWith("{") && !text.StartsWith("{{");
        }


         // <summary>
        /// Perform only required HTML encoding
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(string html)
        {
            return System.Web.HttpUtility.HtmlEncode(html);
    
        }
        public static string HtmlDecode(string html)
        {
            return System.Web.HttpUtility.HtmlDecode(html);
        }

        /// <summary>
        /// Encode text as part of an attribute
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AttributeEncode(string text)
        {
            string quoteChar;
            string attribute = AttributeEncode(text, 
                CQ.DefaultDomRenderingOptions.HasFlag(DomRenderingOptions.QuoteAllAttributes),
                out quoteChar);
            return quoteChar + attribute + quoteChar;
        }
        /// <summary>
        /// Htmlencode a string, except for double-quotes, so it can be enclosed in single-quotes
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AttributeEncode(string text, bool alwaysQuote, out string quoteChar)
        {
            if (text == "")
            {
                quoteChar = "\"";
                return "";
            }

            bool hasQuotes = text.IndexOf("\"") >= 0;
            bool hasSingleQuotes = text.IndexOf("'") >= 0;
            string result = text;
            if (hasQuotes || hasSingleQuotes)
            {

                //un-encode quotes or single-quotes when possible. When writing the attribute it will use the right one
                if (hasQuotes && hasSingleQuotes)
                {
                    result = result.Replace("'", "&#39;");
                    quoteChar = "\'";
                }
                else if (hasQuotes)
                {
                    quoteChar = "'";
                }
                else
                {
                    quoteChar = "\"";
                }
            }
            else
            {
                if (alwaysQuote)
                {
                    quoteChar = "\"";
                }
                else
                {
                    quoteChar = result.IndexOfAny(Utility.DomData.MustBeQuotedAll) >= 0 ? "\"" : "";
                }
            }

            return result;
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
        public static bool IsTruthy(object obj)
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
            if (Objects.IsNumericType(obj.GetType()))
            {
                // obj is IConvertible if IsNumericType already
                return System.Convert.ToDouble(obj) != 0.0;
            }

            return true;
        }
        /// <summary>
        /// Returns true if the object is a primitive numeric type, e.g. exluding string &amp; char
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNumericType(Type type)
        {
            Type t = GetUnderlyingType(type);
            return t.IsPrimitive && !(t == typeof(string) || t == typeof(char) || t==typeof(bool));
        }
        /// <summary>
        /// Returns true if the value is a JS native type (string, number, bool, datetime)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNativeType(Type type)
        {
            Type t = GetUnderlyingType(type);
            return t.IsEnum || t.IsValueType || t.IsPrimitive || t == typeof(string);
        }
        public static string Join(Array list)
        {
            return Join(toStringList(list), ",");
        }
        public static string Join(IEnumerable list)
        {
            return Join(toStringList(list), ",");
        }
        /// <summary>
        /// Test if an object is "Expando-like", e.g. a an IDictionary-string,object-
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExpando(object obj)
        {
            return (obj is IDictionary<string, object>);
        }

        /// <summary>
        /// Test if an object is a an IDictionary-string,object- that is empty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEmptyExpando(object obj)
        {
            return IsExpando(obj) && ((IDictionary<string, object>)obj).Count == 0;
        }

        /// <summary>
        /// Test if an object is a KeyValuePair<,> (e.g. of any types)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKeyValuePair(object obj)
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
        /// <summary>
        /// Combine elements of a list into a single string, separated by separator
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Join(IEnumerable<string> list, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in list)
            {
                sb.Append(sb.Length == 0 ? item : separator + item);
            }
            return sb.ToString();
        }
        private static IEnumerable<string> toStringList(IEnumerable source)
        {
            foreach (var item in source)
            {
                yield return item.ToString();
            }
        }
     
        public static object DefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        /// <summary>
        /// Returns an enumerable of one element from an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(T obj) 
        {
            //List<T> list = new List<T>();
            if (obj != null)
            {
                yield return obj;
                //list.Add(obj);
            }
            //return list;
        }

        public static IEnumerable<T> Enumerate<T>(params T[] obj)
        {
            return obj;
        }


        public static IEnumerable<T> EmptyEnumerable<T>()
        {
            yield break;
        }

        /// <summary>
        /// Convert (recursively) an IDictionary<string,object> to expando objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Dict2Dynamic<T>(IDictionary<string, object> obj) where T : IDynamicMetaObjectProvider, new()
        {
            return Dict2Dynamic<T>(obj, false);
        }
        /// <summary>
        /// Deal with datetime values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ParseValue(object value)
        {
            object result;
            if (value != null && value.GetType().IsAssignableFrom(typeof(DateTime)))
            {
                result = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc).ToLocalTime();
            }
            else
            {
                result = value;
            }

            return result;
        }
        /// <summary>
        /// Takes a default deserialized value from JavaScriptSerializer and parses it into expando objectes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="convertDates"></param>
        /// <returns></returns>
        private static object ConvertDeserializedValue<T>(object value, bool convertDates) where T : IDynamicMetaObjectProvider, new()
        {
            if (value is IDictionary<string, object>)
            {
                return Dict2Dynamic<T>((IDictionary<string, object>)value);
            }
            else if (value is IEnumerable && !(value is string))
            {
                // JSON arrays are returned as ArrayLists of values or IDictionary<string,object> by JavaScriptSerializer 
                // We need to convert the values to expando objects
                IList<object> objectList = new List<object>();
                    
                Type onlyType=null;
                bool same = true;
                foreach (var val in (IEnumerable)value)
                {
                    if (same)
                    {
                        if (onlyType == null)
                        {
                            onlyType = val.GetType();
                        }
                        else
                        {
                            same = onlyType == val.GetType();
                        }
                    }
                    objectList.Add(val);

                }
                if (onlyType != null)
                {
                    IList list;
                    // If it's a list of obejcts, map again to the default dynamic type
                    if (typeof(IDictionary<string, object>).IsAssignableFrom(onlyType))
                    {
                        list = new List<T>();
                    }
                    else
                    {
                        Type listType = typeof(List<>).MakeGenericType(new Type[1] { onlyType });
                        list = (IList)Activator.CreateInstance(listType);
                    }

                    foreach (var item in objectList)
                    {
                        //list.Add(item);
                        list.Add(ConvertDeserializedValue<T>(item, true));
                    }
                    return list;
                }
                else
                {
                    return objectList;
                }
            }
            else if (convertDates)
            {
                return ParseValue(value);
            }
            else
            {
                return value;
            }
            

        }
        /// <summary>
        /// Convert any IDictionary<string,object> into an expandoobject recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="convertDates"></param>
        /// <returns></returns>
        public static T Dict2Dynamic<T>(IDictionary<string, object> obj, bool convertDates) where T : IDynamicMetaObjectProvider, new()
        {
            T returnObj = new T();
            if (obj != null)
            {
                IDictionary<string, object> dict = (IDictionary<string, object>)returnObj;
                foreach (KeyValuePair<string, object> kvp in obj)
                {
                    dict[kvp.Key] = ConvertDeserializedValue<T>(kvp.Value, convertDates);
                }
            }
            return returnObj;
        }

        public static object Extend(HashSet<object> parents, bool deep, object target, params object[] inputObjects )
        {
            if (deep && parents == null)
            {
                parents = new HashSet<object>();
                parents.Add(target);
            }
            // Add all non-null parameters to a processing queue
            Queue<object> inputs = new Queue<object>(inputObjects);
            Queue<object> sources= new Queue<object>();
            HashSet<object> unique = new HashSet<object>();

            while (inputs.Count>0)
            {
                object src = inputs.Dequeue();
                if (src is string && Objects.IsJson(src))
                {
                    src= CQ.ParseJSON((string)src);
                }
                if (!Objects.IsExpando(src) && src.IsExtendableType() &&  src is IEnumerable)
                {
                    foreach (var innerSrc in (IEnumerable)src)
                    {
                        inputs.Enqueue(innerSrc);
                    }
                }
                if (!src.IsImmutable() && unique.Add(src))
                {
                    sources.Enqueue(src);
                }
            }
            // Create a new empty object if there's no existing target -- same as using {} as the jQuery parameter
            if (target == null)
            {
                target = new ExpandoObject();
            }

            else if (!target.IsExtendableType())
            {
                throw new InvalidCastException("Target type '" + target.GetType().ToString() + "' is not valid for CsQuery.Extend.");
            }

            //sources = sources.Dequeue();
            object source;
            while (sources.Count > 0)
            {
                source = sources.Dequeue();

                if (Objects.IsExpando(source))
                {
                    // Expando object -- copy/clone it
                    foreach (var kvp in (IDictionary<string, object>)source)
                    {

                        AddExtendKVP(deep, parents, target, kvp.Key, kvp.Value);
                    }
                }
                else if (!source.IsExtendableType() && source is IEnumerable)
                {
                    // For enumerables, treat each value as another object. Append to the operation list 
                    // This check is after the Expand check since Expandos are elso enumerable
                    foreach (object obj in ((IEnumerable)source))
                    {
                        sources.Enqueue(obj);
                        continue;
                    }
                }
                else
                {
                    // treat it as a regular object - try to copy fields/properties
                    IEnumerable<MemberInfo> members = source.GetType().GetMembers();

                    object value;
                    foreach (var member in members)
                    {
                        if (!IgnorePropertyNames.Contains(member.Name))
                        {
                            // 2nd condition skips index properties
                            if (member is PropertyInfo)
                            {
                                PropertyInfo propInfo = (PropertyInfo)member;
                                if (!propInfo.CanRead || propInfo.GetIndexParameters().Length > 0)
                                {
                                    continue;
                                }
                                value = ((PropertyInfo)member).GetGetMethod().Invoke(source, null);
                            }
                            else if (member is FieldInfo)
                            {
                                FieldInfo fieldInfo = (FieldInfo)member;
                                if (!fieldInfo.IsPublic || fieldInfo.IsStatic)
                                {
                                    continue;
                                }
                                value = fieldInfo.GetValue(source);
                            }
                            //else if (member is MethodInfo)
                            //{
                            //    // Attempt to identify anonymous types which are implemented as methods with no parameters and 
                            //    // names starting with "get_". This is not really ideal, but I don't know a better way to identify
                            //    // them, and I think it's also reasonably safe to invoke any methods named with "get_" anyway.
                            //    MethodInfo methodInfo = (MethodInfo)member;
                            //    if (methodInfo.IsStatic || !methodInfo.IsPublic || methodInfo.IsAbstract || methodInfo.IsConstructor ||
                            //        !(methodInfo.Name.StartsWith("get_")) || methodInfo.GetParameters().Length>0) {
                            //        continue;
                            //    }
                            //    value = methodInfo.Invoke(source,null);
                            //} 
                            else
                            {
                                //It's a method or something we don't know how to handle. Skip it.
                                continue;
                            }
                            AddExtendKVP(deep, parents, target, member.Name, value);

                        }
                    }
                }

            }
            return target;
        }

        /// <summary>
        /// Coerce a javascript object into a Javascript type (null, bool, int, double, datetime, or string). If you know what the 
        /// type should be, then use Convert instead.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IConvertible Coerce(object value)
        {
            if (value==null) {
                return null;
            }
            Type realType = GetUnderlyingType(value.GetType());

            if (realType == typeof(bool) || realType == typeof(DateTime) || realType == typeof(double))
            {
                return (IConvertible)value;
            } 
            else if (IsNumericType(value.GetType()))
            {
                return NumberToDoubleOrInt((IConvertible)value);
            }

            string stringVal = value.ToString();
            
            double doubleVal;
            DateTime dateTimeVal;

            if (stringVal=="false")
            {
                return false;
            }
            else if (stringVal == "true")
            {
                return true;
            }
            else if (stringVal == "undefined" || stringVal == "null")
            {
                return null;
            }
            else if (Double.TryParse(stringVal, out doubleVal))
            {
                return NumberToDoubleOrInt(doubleVal);
            }
            else if (DateTime.TryParse(stringVal, out dateTimeVal))
            {
                return dateTimeVal;
            }
            else
            {
                return stringVal;
            }


        }
        /// <summary>
        /// Convert an object of any value type to the specified type using any known means
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Convert<T>(object value)
        {
            T output;

            if (!TryConvert<T>(value, out output))
            {
                throw new InvalidCastException("Unable to convert to type " + typeof(T).ToString());
            }
            return output;
        }
        /// <summary>
        /// Convert an object of any value type to the specified type using any known means
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Convert(object value, Type type)
        {
            object output;
            if (!TryConvert(value, out output, type, Objects.DefaultValue(type)))
            {
                throw new InvalidCastException("Unable to convert to type " + type.ToString());
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
            object outVal;
            if (TryConvert(value,out outVal,typeof(T))) {
                typedValue = (T)outVal;
                return true;
            } else {
                
                typedValue = (T)DefaultValue(typeof(T));
                return false;
            }
        }

        public static bool TryConvert(object value, out object typedValue, Type type, object defaultValue=null)
        {
            typedValue = null;
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
                    typedValue = Objects.DefaultValue(type);
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
                bool result;
                success = TryStringToBool(stringVal, out result);
                if (success) {
                    output = result;
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
            else
            {
                output = value;
            }

            // cast the ou

            if (output != null 
                && output.GetType() != realType )
            {

                if (realType is IConvertible)
                {
                    try
                    {
                        typedValue = System.Convert.ChangeType(output, realType);
                        success = true;
                    }
                    catch
                    {
                        typedValue = output ?? DefaultValue(realType);
                    }
                }
                if (!success)
                {
                    typedValue = output ?? DefaultValue(realType);
                }
                
            }
            else
            {
                typedValue = output ?? DefaultValue(realType);
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
                    throw new InvalidCastException("Unhandled type for TryParseNumber: " + T.GetType().ToString());
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

        /// <summary>
        /// Return the proper type for an object (ignoring nullability)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        
        #region Object/Expando Manipulation

        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JsObject ToExpando(object source)
        {
            return ToExpando(source,false);
        }
        public static T ToExpando<T>(object source) where T : IDynamicMetaObjectProvider, IDictionary<string,object>, new()
        {
            return ToExpando<T>(source,false);
        }
        /// <summary>
        /// Converts a regular object to an expando object, or returns the source object if it is already an expando object.
        /// If "deep" is true, child properties are cloned rather than referenced.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JsObject ToExpando(object source, bool deep)
        {
            return ToExpando<JsObject>(source, deep);
        }
        public static T ToExpando<T>(object source, bool deep) where T : IDictionary<string,object>,IDynamicMetaObjectProvider, new()
        {
            return ToExpando<T>(source, deep, new Type[] { });
        }
        public static T ToExpando<T>(object source, bool deep, IEnumerable<Type> ignoreAttributes) where T : IDictionary<string,object>, IDynamicMetaObjectProvider, new()
        {
            if (Objects.IsExpando(source) && !deep)
            {
                return Objects.Dict2Dynamic<T>((IDictionary<string, object>)source);
            }
            else
            {
                return ToNewExpando<T>(source, deep, ignoreAttributes);
            }
        }

        public static object CloneObject(object obj)
        {
            return CloneObject(obj,false);
        }
        public static object CloneObject(object obj, bool deep)
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
                return (ToExpando(obj,true));
            }
        }
        

        /// <summary>
        /// Remove a property from an object, returning a new object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static object DeleteProperty(object obj, string property)
        {
            if (!Objects.IsExpando(obj))
            {
                throw new InvalidOperationException("This method only works on objects that implement IDictionary<string,object>");
            }
            ExpandoObject target = (ExpandoObject)CloneObject(obj);
            DeleteProperty(target,property);
            return target;

        }
        public static void DeleteProperty(ExpandoObject obj, string property)
        {
            IDictionary<string, object> objDict = (IDictionary<string, object>)obj;
            objDict.Remove(property);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Implementation of "Extend" functionality
        /// </summary>
        /// <param name="deep"></param>
        /// <param name="parents"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static void AddExtendKVP(bool deep, HashSet<object> parents, object target, string name, object value)
        {
            IDictionary<string, object> targetDict = null;
            if (Objects.IsExpando(target))
            {
                targetDict = (IDictionary<string, object>)target;
            }
            if (deep)
            {
                // Prevent recursion by seeing if this value has been added to the object already.
                // Though jQuery skips such elements, we could get away with this because we clone everything
                // during deep copies. The recursing property wouldn't exist yet when we cloned it.

                // for non-expando objects, we still want to add it & skip - but we can't remove a property
                if (value.IsExtendableType()
                    && !parents.Add(value))
                {
                    if (targetDict != null)
                    {
                        targetDict.Remove(name);
                    }
                    return;
                }

                object curValue;
                if (value.IsExtendableType()
                    && targetDict != null
                    && targetDict.TryGetValue(name, out curValue))
                {
                    //targetDic[name]=Extend(parents,true, null, curValue.IsExtendableType() ? curValue : null, value);
                    value = Extend(parents, true, null, curValue.IsExtendableType() ? curValue : null, value);

                }
                else
                {
                    // targetDic[name] = deep ? value.Clone(true) : value;
                    value = CloneObject(value,true);
                }
            }

            if (targetDict != null)
            {
                targetDict[name] = value;
            }
            else
            {
                // It's a regular object. It cannot be extended, but set any same-named properties.
                IEnumerable<MemberInfo> members = target.GetType().GetMembers();

                foreach (var member in members)
                {
                    if (member.Name.Equals(name, StringComparison.CurrentCulture))
                    {
                        if (member is PropertyInfo)
                        {
                            PropertyInfo propInfo = (PropertyInfo)member;
                            if (!propInfo.CanWrite)
                            {
                                continue;
                            }
                            propInfo.GetSetMethod().Invoke(value, null);

                        }
                        else if (member is FieldInfo)
                        {
                            FieldInfo fieldInfo = (FieldInfo)member;
                            if (fieldInfo.IsStatic || !fieldInfo.IsPublic || fieldInfo.IsLiteral || fieldInfo.IsInitOnly)
                            {
                                continue;
                            }
                            fieldInfo.SetValue(target, value);
                        }
                        else
                        {
                            //It's a method or something we don't know how to handle. Skip it.
                            continue;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Implementation of object>expando
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="deep"></param>
        /// <param name="ignoreAttributes"></param>
        /// <returns></returns>
        private static T ToNewExpando<T>(object source, bool deep, IEnumerable<Type> ignoreAttributes) where T : IDynamicMetaObjectProvider, IDictionary<string,object>, new()
        {
            if (source == null)
            {
                return default(T);
            }
            HashSet<Type> IgnoreList = new HashSet<Type>(ignoreAttributes);

            if (source is string && Objects.IsJson(source))
            {
                source = Utility.JSON.ParseJSON((string)source);
            }

            if (Objects.IsExpando(source))
            {
                return (T)Objects.CloneObject(source, deep);
            }
            else if (source is IDictionary)
            {
                T dict = new T();
                IDictionary sourceDict = (IDictionary)source;
                IDictionary itemDict = (IDictionary)source;
                foreach (var key in itemDict.Keys)
                {
                    string stringKey = key.ToString();
                    if (dict.ContainsKey(stringKey))
                    {
                        throw new InvalidCastException("The key '" + key + "' could not be added because the same key already exists. Conversion of the source object's keys to strings did not result in unique keys.");
                    }
                    dict.Add(stringKey, itemDict[key]);
                }
                return (T)dict;
            }
            else if (!source.IsExtendableType())
            {
                throw new InvalidCastException("Conversion to ExpandObject must be from a JSON string, an object, or an ExpandoObject");
            }

            T target = new T();
            IDictionary<string, object> targetDict = (IDictionary<string, object>)target;

            IEnumerable<MemberInfo> members = source.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            foreach (var member in members)
            {
                if (!IgnorePropertyNames.Contains(member.Name)) {
                    foreach (object attrObj in member.GetCustomAttributes(false))
                    {
                        Attribute attr = (Attribute)attrObj;
                        if (IgnoreList.Contains(attr.GetType()))
                        {
                            goto NextAttribute;
                        }
                    }
                    string name = member.Name;


                    object value = null;
                    bool skip = false;
                
                    if (member is PropertyInfo) 
                    {
                        PropertyInfo propInfo = (PropertyInfo)member;
                        if (propInfo.GetIndexParameters().Length == 0 &&
                            propInfo.CanRead) {

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
                        targetDict[name] = deep ? Objects.CloneObject(value,true) : value;
                    }
                }
            NextAttribute: { }
            }
        
            return target;
        }
        private static bool TryStringToBool(string value, out bool result)
        {
            switch (value)
            {
                case "on":
                case "yes":
                case "true":
                case "enabled":
                case "active":
                case "1":
                    result = true;
                    return true;
                case "off":
                case "no":
                case "false":
                case "disabled":
                case "0":
                    result = false;
                    return true;
            }
            result = false;
            return false;
        }
        /// <summary>
        /// Return an int or double from any number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IConvertible NumberToDoubleOrInt(IConvertible value)
        {
            double val = (double)System.Convert.ChangeType(value, typeof(double));
            if (val == Math.Floor(val))
            {
                return (int)val;
            }
            else
            {
                return val;
            }
        }
        #endregion
    }
}
