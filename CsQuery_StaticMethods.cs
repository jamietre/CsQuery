using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.IO;
using System.Web.Script.Serialization;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{
    public partial class CsQuery
    {
        #region defaults
        /// <summary>
        /// Rendering option flags
        /// </summary>
        public static DomRenderingOptions DefaultDomRenderingOptions = DomRenderingOptions.QuoteAllAttributes;

        /// <summary>
        /// The default rendering type. This mostly controls the header and how tags are closed. UNIMPLEMENTED right now.
        /// </summary>
        public static DocType DefaultDocType = DocType.HTML5;

        #endregion 
        #region Create methods - returns a new DOM
        public static CsQuery Create()
        {
            return new CsQuery();
        }
        /// <summary>
        /// Creeate a new DOM from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CsQuery Create(string html)
        {
            CsQuery csq = new CsQuery();
            csq.Load(html);
            return csq;
        }

        public static CsQuery Create(char[] html)
        {
            CsQuery csq = new CsQuery();
            csq.Load(html);
            return csq;
        }

        public static CsQuery Create(string selector, string jsonCss)
        {
            CsQuery csq = CsQuery.Create(selector);
            return csq.AttrSet(jsonCss);
        }
        public static CsQuery Create(string selector, IDictionary<string,object> css)
        {
            CsQuery csq = CsQuery.Create(selector);
            return csq.Attr(css);
        }

        public static CsQuery CreateFromFile(string path)
        {

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            int bufSize = 32768;
            long len = fileStream.Length;
            char[] data = new char[len];
            long index = 0;
            int charsRead = -1;
            using (StreamReader streamReader = new StreamReader(fileStream))
            {

                while (charsRead > 0)
                {
                    char[] fileContents = new char[bufSize];
                    charsRead = streamReader.Read(data, 0, bufSize);

                    if (index + bufSize >= len)
                    {
                        int pos = 0;
                        for (long i = index; i < len; i++)
                        {
                            data[i] = fileContents[pos++];
                            charsRead = 0;
                        }
                    }
                    else
                    {

                        fileContents.CopyTo(data, index);
                        index += bufSize;
                    }
                }
            }
            return CsQuery.Create(data);
        }

        /// <summary>
        /// Creates a new DOM from a file
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CsQuery LoadFile(string htmlFile)
        {
            return CsQuery.Create(Support.GetFile(htmlFile));
        }
        /// <summary>
        /// Creeate a new DOM from elements
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CsQuery Create(IEnumerable<IDomObject> elements)
        {
            CsQuery csq = new CsQuery();
            csq.Load(elements);
            return csq;
        }

        public static CsQuery Create(IDomObject element)
        {
            CsQuery csq = new CsQuery();
            csq.Load(Objects.Enumerate(element));
            return csq;
        }
        #endregion

        public static void Each<T>(IEnumerable<T> list, Action<T> func)
        {
            foreach (var obj in list)
            {
                func(obj);
            }
        }

        public static IEnumerable<T> Map<T>(IEnumerable<IDomObject> elements, Func<IDomObject, T> function)
        {
            foreach (var element in elements)
            {
                yield return function(element);
            }
            yield break;
        }
        public IEnumerable<T> Map<T>(Func<IDomObject, T> function)
        {
            return CsQuery.Map(this, function);
        }
        public static object Extend(object target, params object[] sources)
        {
            return CsQuery.Extend(false, target, sources);
        }
        public static object Extend(bool deep, object target, params object[] sources)
        {
            return Objects.Extend(null, deep, target, sources);
        }

        /// <summary>
        /// Convert an object to JSON
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string ToJSON(object objectToSerialize)
        {
            return Utility.JSON.ToJSON(objectToSerialize);

        }
        /// <summary>
        /// Parse JSON into a typed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static T ParseJSON<T>(string objectToDeserialize)
        {

            return Utility.JSON.ParseJSON<T>(objectToDeserialize);
        }
        /// <summary>
        /// Parse JSON into an expando object, or a primitive type if possible, or just return
        /// itself if no parsing can be done.
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSON(string objectToDeserialize)
        {
            return Utility.JSON.ParseJSON(objectToDeserialize);
        }
        public static object ParseJSON(string objectToDeserialize, Type type)
        {
            return Utility.JSON.ParseJSON(objectToDeserialize, type);
        }

        /// <summary>
        /// Convert a dictionary to an expando object. Use to get another expando object from a sub-object of an expando object,
        /// e.g. as returned from JSON data
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static JsObject ToExpando(object obj)
        {
            JsObject result;


            if (obj is IDictionary<string, object>)
            {
                result = Objects.Dict2Dynamic<JsObject>((IDictionary<string, object>)obj);
            }
            else
            {
                //throw new Exception("This is not tested at all.");
                return obj.ToExpando();
            }
            return result;
        }
        public static T ToDynamic<T>(object obj) where T : IDynamicMetaObjectProvider, new()
        {
            if (obj is IDictionary<string, object>)
            {
                return Objects.Dict2Dynamic<T>((IDictionary<string, object>)obj);
            }
            else
            {
                return obj.ToExpando<T>();
                //throw new Exception("Not implemented.");
            }
        }
        public static IEnumerable<T> Enumerate<T>(object obj)
        {
            return Enumerate<T>(obj, new Type[] { typeof(ScriptIgnoreAttribute) });
        }
        /// <summary>
        /// Enumerate the properties of an object, casting to type T
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(object obj, IEnumerable<Type> ignoreAttributes)
        {
            HashSet<Type> IgnoreList = new HashSet<Type>();
            if (ignoreAttributes != null)
            {
                IgnoreList.AddRange(ignoreAttributes);
            }

            IDictionary<string, object> source;

            if (obj is IDictionary<string, object>)
            {
                source = (IDictionary<string, object>)obj;
            }
            else
            {
                source = obj.ToExpando<JsObject>(false, ignoreAttributes);
            }
            foreach (KeyValuePair<string, object> kvp in source)
            {
                if (typeof(T) == typeof(KeyValuePair<string, object>))
                {
                    yield return (T)(object)(new KeyValuePair<string, object>(kvp.Key,
                         kvp.Value is IDictionary<string, object> ?
                            ToExpando((IDictionary<string, object>)kvp.Value) :
                            kvp.Value));

                }
                else
                {
                    yield return Objects.Convert<T>(kvp.Value);
                }
            }

        }
    }
}
