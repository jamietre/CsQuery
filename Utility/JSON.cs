using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;

namespace Jtc.CsQuery.Utility
{
    public static class JSON
    {
        private static JavaScriptSerializer Serializer
        {
            get
            {
                return _Serializer.Value;
            }
        }
        private static Lazy<JavaScriptSerializer> _Serializer = new Lazy<JavaScriptSerializer>();

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
                if (value.IsImmutable())
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
                        if (obj.IsImmutable())
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
                    throw new Exception("Serializer error: valueToJson called for an object");
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
                if (!value.IsExtendableType())
                {
                    valueToJSON(value);
                }
                else
                {
                    sb.Append("{");
                    bool first = true;
                    foreach (KeyValuePair<string,object> kvp in CsQuery.Enumerate<KeyValuePair<string,object>>(value,new Type[] {typeof(ScriptIgnoreAttribute)})) {
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
        /// <summary>
        /// Convert an object to JSON
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(object objectToSerialize)
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Serialize(objectToSerialize);

        }
        //public static string ToJSONFormatted(object objectToSerialize)
        //{
        //    JsonSerializer serializer = new JsonSerializer();
        //    serializer.FormatOutput = true;
        //    return serializer.Serialize(objectToSerialize);

        //}
        /// <summary>
        ///  Serialize a value type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        
        /// <summary>
        /// Parse JSON into a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T ParseJSON<T>(string objectToDeserialize)
        {
            // TODO: Don't use Javascript Serializer. Even if we are not converting to ExpandoObject, we would like better
            // control over the deserialization process to fix dates. This code only works for date values, not members.
            // JavaScriptSerializer serializer = new JavaScriptSerializer();

            return (T)ParseJSON(objectToDeserialize, typeof(T));
        }
        public static object ParseJSON(string objectToDeserialize, Type type)
        {
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            {
                return ParseJSONObject(objectToDeserialize);
            }
            if (objectToDeserialize == "null" || objectToDeserialize == "undefined")
            {
                return null;
            }
            object output = Serializer.Deserialize(objectToDeserialize, type);
            if (type == typeof(DateTime) || (type == typeof(DateTime?) && output != null))
            {
                DateTime dtVal = (DateTime)(object)output;

                output = DateTime.SpecifyKind(dtVal, DateTimeKind.Utc).ToLocalTime();
            }
            return output;
        }
        /// <summary>
        /// Parse JSON into an JsObject (dynamic) object
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSON(string objectToDeserialize)
        {
            if (String.IsNullOrEmpty(objectToDeserialize))
            {
                return null;
            }
            switch (objectToDeserialize.Trim()[0])
            {
                case '{':
                    return Utility.JSON.ParseJSONObject(objectToDeserialize);
                case '\"':
                    return Utility.JSON.ParseJSON<string>(objectToDeserialize);
                default:
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
                    return objectToDeserialize;


            }
        }
        /// <summary>
        /// Deserialize javscript, then transform to an ExpandObject
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        private static JsObject ParseJSONObject(string objectToDeserialize)
        {
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dict = (Dictionary<string, object>)Serializer.Deserialize(objectToDeserialize, typeof(Dictionary<string, object>));

            return Objects.Dict2Dynamic<JsObject>(dict,true);
        }

    }
}
