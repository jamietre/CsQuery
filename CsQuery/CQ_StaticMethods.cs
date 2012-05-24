using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using CsQuery.ExtensionMethods;
using CsQuery.Utility;

namespace CsQuery
{
    public partial class CQ
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
        public static CQ Create()
        {
            return new CQ();
        }
        /// <summary>
        /// Creeate a new DOM from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ Create(string html)
        {
            CQ csq = new CQ();
            csq.Load(html);
            return csq;
        }

        public static CQ Create(char[] html)
        {
            CQ csq = new CQ();
            csq.Load(html);
            return csq;
        }


        public static CQ Create(string selector, object css)
        {
            CQ csq = CQ.Create(selector);
            
            return csq.AttrSet(css,true);
        }

        /// <summary>
        /// Creates a new DOM from a file
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateFromFile(string htmlFile)
        {
            return CQ.Create(Support.GetFile(htmlFile));
        }
        /// <summary>
        /// Creeate a new DOM from elements
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ Create(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ();
            csq.Load(elements);
            return csq;
        }

        public static CQ Create(IDomObject element)
        {
            CQ csq = new CQ();
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
        /// <summary>
        /// Map each element of the result set to a new form. If a value is returned from the function, the element
        /// will be excluded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(IEnumerable<IDomObject> elements, Func<IDomObject, T> function)
        {
            foreach (var element in elements)
            {
                T result = function(element);
                if (result != null)
                {
                    yield return result;
                }
            }
        }

        public IEnumerable<T> Map<T>(Func<IDomObject, T> function)
        {
            return CQ.Map(this, function);
        }
        public static object Extend(object target, params object[] sources)
        {
            return CQ.Extend(false, target, sources);
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
                return Objects.ToExpando(obj);
            }
            return result;
        }
        public static T ToDynamic<T>(object obj) where T : IDynamicMetaObjectProvider, IDictionary<string,object>,new()
        {
            if (obj is IDictionary<string, object>)
            {
                return Objects.Dict2Dynamic<T>((IDictionary<string, object>)obj);
            }
            else
            {
                return Objects.ToExpando<T>(obj);
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
                source = Objects.ToExpando<JsObject>(obj,false, ignoreAttributes);
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

        private static Browser _Browser;
        public static Browser Browser
        {
            get
            {
                if (_Browser == null)
                {
                    _Browser = new Browser(HttpContext.Current);
                }

                return _Browser;
            }
        }
    }
}
