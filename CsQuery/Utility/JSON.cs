using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text.RegularExpressions;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Utility
{
    /// <summary>
    /// Methods for working with JSON. TODO: This class needs some help. While not thrilled about the idea of writing another JSON serializer,
    /// CsQuery does some unique handling for deserialization, e.g. mapping sub-objects to expando objects. We can do a post-op parsing from 
    /// any other JSON serializer (such as we are doing now) but this doubles the overhead required. Look at a customized implementation from 
    /// Newtonsoft, though any customization makes it difficult to use a simple strategy for drop-in replacement of the serializer. Perhaps 
    /// implement an interface for the serializer with a simple wrapper class for a generic serializer that performs needed post-op
    /// substitutions as part of the base library, with an optimized native implementation?
    /// </summary>
    public static class JSON
    {
        #region constructor and internal data

        static JSON()
        {
            escapeLookup = new char[127];
            escapeLookup['b'] = (char)8;
            escapeLookup['f'] = (char)12;
            escapeLookup['n'] = (char)10;
            escapeLookup['e'] = (char)13;
            escapeLookup['t'] = (char)9;
            escapeLookup['v'] = (char)11;
            escapeLookup['"'] = '"';
            escapeLookup['\\'] = '\\';

        }
        private static char[] escapeLookup;
        /// <summary>
        /// Internal class to optimize StringBuilder creation
        /// </summary>
        private class JsonSerializer
        {
            public bool FormatOutput
            { get; set; }
            protected int Indent = 0;

            StringBuilder sb = new StringBuilder();
            private void valueToJSON(object value)
            {
                if (Objects.IsImmutable(value))
                {
                    sb.Append(Serializer.Serialize(value));
                }
                else if (IsKeyValueDictionary(value))
                {
                    sb.Append("{");
                    bool first = true;
                    foreach (dynamic item in (IEnumerable)value)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        sb.Append(item.Key + ":" + JSON.ToJSON(item.Value));
                    }
                    sb.Append("}");
                }
                else if (value is IEnumerable)
                {
                    sb.Append("[");
                    bool first = true;
                    foreach (object obj in (IEnumerable)value)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        if (Objects.IsImmutable(obj))
                        {
                            valueToJSON(obj);
                        }
                        else
                        {
                            sb.Append(ToJSON(obj));
                        }
                    }
                    sb.Append("]");
                }
                else
                {
                    throw new InvalidOperationException("Serializer error: valueToJson called for an object");
                }
            }
            protected bool IsKeyValueDictionary(object value)
            {
                Type type = value.GetType();
                return type.IsGenericType &&
                    typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition());
            }
            public string Serialize(object value)
            {
                SerializeImpl(value);
                return sb.ToString();
            }
            public void SerializeImpl(object value) {
                //if ((value is IEnumerable && !value.IsExpando()) || value.IsImmutable())
                if (!Objects.IsExtendableType(value))
                {
                    valueToJSON(value);
                }
                else
                {
                    sb.Append("{");
                    bool first = true;
                    foreach (KeyValuePair<string,object> kvp in CQ.Enumerate<KeyValuePair<string,object>>(value,new Type[] {typeof(ScriptIgnoreAttribute)})) {
                        if (first)
                        {
                            first = false; 
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        sb.Append("\"" + kvp.Key + "\":");
                        SerializeImpl(kvp.Value);

                    }
                    sb.Append("}");
                }
            }
        }

        private static Lazy<JavaScriptSerializer> _Serializer = new Lazy<JavaScriptSerializer>();
        private static JavaScriptSerializer Serializer
        {
            get
            {
                return _Serializer.Value;
            }
        }
       
        #endregion

        #region public methods

        /// <summary>
        /// Convert an object to JSON using the default handling of the serializer
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(object objectToSerialize)
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Serialize(objectToSerialize);

        }
        /// <summary>
        /// Parse JSON into a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T ParseJSON<T>(string objectToDeserialize)
        {
            return (T)ParseJSON(objectToDeserialize, typeof(T));
        }

        /// <summary>
        /// Parse JSON into a typed object
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ParseJSON(string objectToDeserialize, Type type)
        {
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            {
                return ParseJSONObject(objectToDeserialize);
            }
            else if (Objects.IsNativeType(type))
            {
                return ParseJSONValue(objectToDeserialize, type);
            } else {
                object output = Serializer.Deserialize(objectToDeserialize, type);
                return output;
            }
        }
        /// <summary>
        /// Parse JSON into a dynamic object, or single typed value
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSON(string objectToDeserialize)
        {
            if (String.IsNullOrEmpty(objectToDeserialize))
            {
                return null;
            } else {
                    return ParseJSONValue(objectToDeserialize);
            }
        }

        /// <summary>
        /// Parse a JSON value to a C# value (string,bool, int, double, datetime) or, if the value is another object, an object or array.
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSONValue(string objectToDeserialize)
        {
            object value;
            if (TryParseJsonValueImpl(objectToDeserialize, out value)) {
                return value;
            } else {
                // It's not a string, see what we can get out of it
                int integer;
                if (int.TryParse(objectToDeserialize, out integer))
                {
                    return integer;
                }
                double dbl;
                if (double.TryParse(objectToDeserialize, out dbl))
                {
                    return dbl;
                }
                bool boolean;
                if (bool.TryParse(objectToDeserialize, out boolean))
                {
                    return boolean;
                }

                throw new ArgumentException("The value '" + objectToDeserialize + "' could not be parsed, it doesn't seem to be something that should be a JSON value");

           }
        }

        /// <summary>
        /// Parse a JSON value to a C# value of the type requested
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSONValue(string objectToDeserialize, Type type)
        {
            
            Type baseType = Objects.GetUnderlyingType(type);
            object value;
            if (TryParseJsonValueImpl(objectToDeserialize, out value))
            {
                return Convert.ChangeType(value, type);
            }
            else
            {
                string obj = objectToDeserialize.Trim();
                if (type.IsEnum)
                {
                    int integer;
                    if (int.TryParse(obj, out integer))
                    {
                        return Enum.Parse(type, integer.ToString());
                    }
                }
                if (Objects.IsNumericType(type))
                {
                    int integer;
                    if (int.TryParse(obj, out integer))
                    {
                        return Convert.ChangeType(integer, type);
                    }
                    double dbl;
                    if (double.TryParse(obj, out dbl))
                    {
                        return Convert.ChangeType(dbl, type);
                    }
                }
                else if (baseType == typeof(bool))
                {
                    bool boolean;
                    if (bool.TryParse(obj, out boolean))
                    {
                        return boolean;
                    }
                }
            }
            throw new ArgumentException("The value '" + objectToDeserialize + "' could not be parsed to type '" + type.ToString() + "'");

        }

       
        /// <summary>
        /// The value represents a JSON date (MS format)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsJsonDate(string input)
        {
            return input.Length >= 7 && input.Substring(0, 7) == "\"\\/Date";
        }

        /// <summary>
        /// The value represents a JSON object, e.g. is bounded by curly braces
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsJsonObject(string input)
        {
            return input != null && input.StartsWith("{") && input.EndsWith("}");
        }

        /// <summary>
        /// The value represents a JSON string, e.g. is bounded by double-quotes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsJsonString(string input)
        {
            return input.StartsWith("\"") && input.EndsWith("\"");
        }

        /// <summary>
        /// The value represents a JSON array, e.g. is bounded by square brackets
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsJsonArray(string input)
        {
            return input.StartsWith("[") && input.EndsWith("]");
        }
       
        #endregion

        #region private methods
        /// <summary>
        /// Try to parse a JSON value into a value type or, if the value represents an object or array, an object. This method does not
        /// address numeric types, leaving that up to a caller, so that they can map to specific numeric casts if desired.
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryParseJsonValueImpl(string objectToDeserialize, out object value)
        {
            var obj = objectToDeserialize.Trim();
            if (String.IsNullOrEmpty(obj))
            {
                throw new ArgumentException("No value passed, not a valid json value.");
            }
            else if (obj == "null" || obj == "undefined")
            {
                value = null;
            }
            else if (obj == "{}")
            {
                value = new JsObject();
            }
            else if (IsJsonObject(obj))
            {
                value = ParseJSONObject(obj);
            }
            else if (IsJsonDate(obj))
            {
                value = FromJSDateTime(obj);
            }
            else if (IsJsonString(obj))
            {
                value = ParseJsonString(obj);
            }
            else if (IsJsonArray(obj))
            {
                value = ParseJsonArray(obj);
            }
            else
            {
                value = null;
                return false;
            }
            return true;
        }

        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime FromJSDateTime(string jsDateTime)
        {
            Regex regex = new Regex(@"^""\\/Date\((?<ticks>-?[0-9]+)\)\\/""");

            string ticks = regex.Match(jsDateTime).Groups["ticks"].Value;

            DateTime dt = unixEpoch.AddMilliseconds(Convert.ToDouble(ticks));
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToLocalTime();

        }
        /// <summary>
        /// Deserialize javscript, then transform to an ExpandObject
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        private static JsObject ParseJSONObject(string objectToDeserialize)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)Serializer.Deserialize(objectToDeserialize, typeof(Dictionary<string, object>));

            return Objects.Dict2Dynamic<JsObject>(dict, true);
        }

        private  static string ParseJsonString(string input)
        {

            string obj = input.Substring(1, input.Length - 2);
            StringBuilder output = new StringBuilder();
            int pos = 0;
            while (pos < obj.Length)
            {
                char cur = obj[pos];
                if (cur == '\\')
                {
                    cur = obj[++pos];
                    char unescaped = escapeLookup[(byte)obj[pos]];
                    if (unescaped > 0)
                    {
                        output.Append(unescaped);
                    }
                    else
                    {
                        output.Append(cur);
                    }
                }
                else
                {
                    output.Append(cur);
                }
                pos++;
            }
            return output.ToString();
        }

         private static object ParseJsonArray(string input)
        {
            string obj = input.Substring(1, input.Length - 2);
            List<object> list = new List<object>();
            Type oneType=null;
            bool typed=true;
            string[] elements = obj.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<elements.Length;i++) {
                string el = elements[i];
                object json = ParseJSONValue(el);
                
                if (i == 0)
                {
                    oneType = json.GetType();
                }
                else if (typed)
                {
                    if (json.GetType() != oneType)
                    {
                        oneType = typeof(object);
                        typed = false;
                    }
                }
                list.Add(json);
            }
            if (typed)
            {
                Type listType = typeof(List<>).MakeGenericType(new Type[] { oneType });
                IList typedList = (IList)Activator.CreateInstance(listType);
                foreach (var item in list)
                {
                    typedList.Add(item);
                }
                return typedList;
            }
            else
            {
                return list;
            }

        }
        #endregion
    }
}
