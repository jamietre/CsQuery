﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CsQuery.Utility
{
    /// <summary>
    /// TODO: This class needs some help. While not thrilled about the idea of writing another JSON serializer,
    /// CsQuery does some unique handling for serialization &amp;  deserialization, e.g. mapping sub-objects to expando objects. 
    /// 
    /// We can do a post-op parsing from 
    /// any other JSON serializer (such as we are doing now) but this doubles the overhead required. Look at a customized implementation from 
    /// Newtonsoft, though any customization makes it difficult to use a simple strategy for drop-in replacement of the serializer. Perhaps 
    /// implement an interface for a serializer wrapper class that lets us pass any generic serializer that performs needed post-op
    /// substitutions as part of the base library, with an optimized native implementation?
    /// </summary>
    public class JsonSerializer: IJsonSerializer
    {
        /// <summary>
        ///  The real serializer implementation. We need to set up a DI contiainer to manage this (see todo above)
        /// </summary>
        private static Lazy<JavaScriptSerializer> _Serializer = new Lazy<JavaScriptSerializer>();
        private static JavaScriptSerializer Serializer
        {
            get
            {
                return _Serializer.Value;
            }
        }
        
        protected StringBuilder sb = new StringBuilder();

        public string Serialize(object value)
        {
            sb.Clear();
            SerializeImpl(value);
            return sb.ToString();
        }
        public object Deserialize(string value, Type type)
        {
            return Serializer.Deserialize(value, type);
        }
        public T Deserialize<T>(string value)
        {
            return Serializer.Deserialize<T>(value);
        }
        #region private methods

        protected void SerializeImpl(object value) {
            //if ((value is IEnumerable && !value.IsExpando()) || value.IsImmutable())
            if (!Objects.IsExtendableType(value))
            {
                valueToJSON(value);
            }
            else
            {
                sb.Append("{");
                bool first = true;
                foreach (KeyValuePair<string,object> kvp in 
                    Objects.EnumerateProperties<KeyValuePair<string,object>>(
                        value,
                        new Type[] {typeof(ScriptIgnoreAttribute)})
                ) {
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

        protected void valueToJSON(object value)
        {
            if (Objects.IsImmutable(value))
            {
                sb.Append(Serializer.Serialize(value));
            }
            else if (IsDictionary(value))
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
                    sb.Append("\"" + item.Key.ToString() + "\":" + JSON.ToJSON(item.Value));
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
                        SerializeImpl(obj);
                    }
                }
                sb.Append("]");
            }
            else
            {
                throw new InvalidOperationException("Serializer error: valueToJson called for an object");
            }
        }


        protected bool IsDictionary(object value)
        {
            Type type = value.GetType();

            return type.GetInterfaces()
              .Where(t => t.IsGenericType)
              .Select(t => t.GetGenericTypeDefinition())
              .Any(t => t.Equals(typeof(IDictionary<,>)));

        }


        #endregion

    }

   
}
