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
            StringBuilder sb = new StringBuilder();
            private void valueToJSON(object value)
            {
                if (value.IsImmutable())
                {
                    sb.Append(Serializer.Serialize(value));
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
                        } else {
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
            public string Serialize(object value)
            {
                SerializeImpl(value);
                return sb.ToString();
            }
            public void SerializeImpl(object value) {
                if ((value is IEnumerable && !value.IsExpando()) || value.IsImmutable())
                {
                    valueToJSON(value);
                }
                else
                {
                    sb.Append("{");
                    bool first = true;
                    foreach (KeyValuePair<string,object> kvp in CsQuery.Enumerate(value)) {
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
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return (T)serializer.Deserialize(objectToDeserialize, typeof(T));
        }
        /// <summary>
        /// Parse JSON into an expando object
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

        private static ExpandoObject ParseJSONObject(string objectToDeserialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dict = (Dictionary<string, object>)serializer.Deserialize(objectToDeserialize, typeof(Dictionary<string, object>));
            //ExpandoObject output = new ExpandoObject();
            //foreach (var kvp in dict)
            //{


            //}
            return (ExpandoObject)CsQuery.Extend(true, null, dict);
            //return Dict2Epando(dict);
        }

    }
}
