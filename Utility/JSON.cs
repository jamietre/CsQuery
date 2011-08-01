using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Dynamic;

namespace Jtc.CsQuery.Utility
{
    public static class JSON
    {
        /// <summary>
        /// Convert an object to JSON
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(object objectToSerialize)
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
        public static ExpandoObject ParseJSON(string objectToDeserialize)
        {
            if (String.IsNullOrEmpty(objectToDeserialize))
            {
                return null;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dict = (Dictionary<string, object>)serializer.Deserialize(objectToDeserialize, typeof(Dictionary<string, object>));
            return (ExpandoObject)CsQuery.Extend(true, null, dict);
        }

        private static string Flatten(ExpandoObject expando)
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
    }
}
