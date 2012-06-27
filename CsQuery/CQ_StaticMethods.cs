﻿using System;
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
using CsQuery.Engine;
using CsQuery.Web;
using CsQuery.Promises;

namespace CsQuery
{
    public partial class CQ
    {
        #region CsQuery global options

        /// <summary>
        /// Rendering option flags
        /// </summary>
        public static DomRenderingOptions DefaultDomRenderingOptions = DomRenderingOptions.QuoteAllAttributes;

        /// <summary>
        /// The default settings used when making remote requests
        /// </summary>
        public static ServerConfig DefaultServerConfig = new ServerConfig();

        /// <summary>
        /// The default rendering type. This mostly controls the header and how tags are closed. UNIMPLEMENTED right now.
        /// </summary>
        public static DocType DefaultDocType = DocType.HTML5;

        #endregion 

        #region private properties
        
        private static Browser _Browser;
        
        #endregion

        #region Create methods: return a new CQ object from HTML or other elements

        /// <summary>
        /// Create an empty CQ object
        /// </summary>
        /// <returns></returns>
        public static CQ Create()
        {
            return new CQ();
        }
        /// <summary>
        /// Creeate a new CQ object from an HTML string
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ Create(string html)
        {
            CQ csq = new CQ();
            csq.LoadContent(ConvertHtmlString(html));
            return csq;
        }

        /// <summary>
        /// Create a new CQ object from an HTML character array 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ Create(char[] html)
        {
            CQ csq = new CQ();
            csq.LoadDocument(html);
            return csq;
        }

        /// <summary>
        /// Create a new CQ from an HTML fragment, and use quickSet to create attributes (and/or css)
        /// </summary>
        /// <param name="html">A string of HTML</param>
        /// <param name="quickSet">an object containing CSS properties and attributes to be applied to the resulting fragment</param>
        /// <returns></returns>
        public static CQ Create(string html, object quickSet)
        {
            CQ csq = CQ.CreateFragment(html);
            return csq.AttrSet(quickSet, true);
        }

        /// <summary>
        /// Create a new CQ object from an existing context, bound to the same domain.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
        protected void Create(string selector, CQ context)
        {
            // when creating a new CsQuery from another, leave Dom blank - it will be populated automatically with the
            // contents of the selector.

            CsQueryParent = context;

            if (!String.IsNullOrEmpty(selector))
            {
                Selectors = new Selector(selector);
                AddSelectionRange(Selectors.Select(Document, context));
            }
        }


        /// <summary>
        /// Create a new CQ object from a single element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CQ Create(IDomObject element)
        {
            CQ csq = new CQ();
            csq.LoadFragment(Objects.Enumerate(element));
            return csq;
        }

        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object
        /// </summary>
        /// <param name="elements">A sequence of elements</param>
        /// <returns></returns>
        public static CQ Create(IEnumerable<IDomObject> elements)
        {
            CQ csq = new CQ();
            csq.LoadFragment(elements);
            return csq;
        }

        /// <summary>
        /// Create a new CQ object from a stream of HTML, treating the HTML as a content document
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static CQ Create(Stream stream)
        {
            return Create(StreamToCharArray(stream));
        }

        /// <summary>
        /// Creeate a new fragment from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(string html)
        {
            CQ csq = new CQ();
            csq.LoadFragment(ConvertHtmlString(html));
            return csq;
        }


        /// <summary>
        /// Create a new fragment from HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(char[] html)
        {
            CQ csq = new CQ();
            csq.LoadFragment(html);
            return csq;
        }

        /// <summary>
        /// Create a new fragment from a stream of HTML text
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateFragment(Stream html)
        {
            return CreateFragment(StreamToCharArray(html));
        }

        /// <summary>
        /// Creeate a new CQ object from a squence of elements, or another CQ object
        /// </summary>
        /// <param name="elements">A sequence of elements</param>
        /// <returns></returns>
        public static CQ CreateFragment(IEnumerable<IDomObject> elements)
        {
            // this is synonymous with the Create method of the same sig because we definitely
            // would never autogenerate elements from a sequence of elements

            return Create(elements);
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(string html)
        {
            CQ csq = new CQ();
            csq.LoadDocument(ConvertHtmlString(html));
            return csq;
        }

        /// <summary>
        /// Creeate a new DOM from HTML text using full HTML5 tag generation
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(char[] html)
        {
            CQ csq = new CQ();
            csq.LoadDocument(html);
            return csq;
        }


        /// <summary>
        /// UNTESTED!
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static CQ CreateDocument(Stream html)
        {
            return CreateDocument(StreamToCharArray(html));
        }
        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateDocumentFromFile(string htmlFile)
        {
            return CQ.CreateDocument(Support.GetFileStream(htmlFile));
        }

        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateFromFile(string htmlFile)
        {
            return CQ.Create(Support.GetFileStream(htmlFile));
        }

        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateFromUrl(string url, ServerConfig options=null)
        {
            ServerConfig config = ServerConfig.Merge(options);
             
            CsqWebRequest con = new CsqWebRequest(url);
            
            if (config.UserAgent!=null) {
                con.UserAgent = config.UserAgent;
            }
            if (config.Timeout != null)
            {
                con.Timeout = (int)config.Timeout;
            }
            con.Get();

            return CQ.CreateDocument(con.Html);
        }
        
        /// <summary>
        /// Start an asynchronous request to an HTTP server, returning a promise that will resolve when the request is completed or rejected
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSuccess"></param>
        /// <param name="callbackFail"></param>
        /// <param name="options"></param>
        /// <returns></returns>

        public static IPromise<ICsqWebResponse> CreateFromUrlAsync(string url, ServerConfig options = null)
        {
            var deferred = When.Deferred<ICsqWebResponse>();
            int uniqueID = AsyncWebRequestManager.StartAsyncWebRequest(url, deferred.Resolve, deferred.Reject, options);
            return deferred;
        }


        /// <summary>
        /// Start an asynchronous request to an HTTP server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSuccess"></param>
        /// <param name="callbackFail"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int CreateFromUrlAsync(string url, Action<ICsqWebResponse> callbackSuccess, Action<ICsqWebResponse> callbackFail=null, ServerConfig options = null)
        {
            return AsyncWebRequestManager.StartAsyncWebRequest(url,callbackSuccess,callbackFail,options); 
            
        }
        /// <summary>
        /// Start an asynchronous request to an HTTP server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callbackSuccess"></param>
        /// <param name="callbackFail"></param>
        /// <param name="options"></param>
        /// <returns></returns>

        public static void CreateFromUrlAsync(string url, int id, Action<ICsqWebResponse> callbackSuccess, Action<ICsqWebResponse> callbackFail = null, ServerConfig options = null)
        {
            AsyncWebRequestManager.StartAsyncWebRequest(url, callbackSuccess, callbackFail,id, options);

        }

        /// <summary>
        /// Block this thread until all pending asynchronous web requests have completed.
        /// </summary>
        /// <param name="timeout"></param>
        public static void WaitForAsyncEvents(int timeout = -1)
        {
            AsyncWebRequestManager.WaitForAsyncEvents(timeout);
        }

        /// <summary>
        /// Return a new promise that resolves when all the promises passed in are resolved
        /// </summary>
        /// <param name="promises"></param>
        /// <returns></returns>
        public static IPromise WhenAll(params IPromise[] promises)
        {
            return When.All(promises);
        }


        #endregion

        #region static utility methods

        /// <summary>
        /// Iterate over each element in a sequence, and call a delegate for each element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="func"></param>
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

        /// <summary>
        /// Map each property of the objects in sources to the target object.  Returns an expando object (either 
        /// the target object, if it's an expando object, or a new expando object)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static object Extend(object target, params object[] sources)
        {
            return Objects.Extend(false, target, sources);
        }

        /// <summary>
        /// Map each property of the objects in sources to the target object.  Returns an expando object (either 
        /// the target object, if it's an expando object, or a new expando object)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static object Extend(bool deep, object target, params object[] sources)
        {
            return Objects.Extend(deep, target, sources);
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
        /// Parse a JSON string into an expando object, or a json value into a primitive type.
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
        public static object ParseJSON(string objectToDeserialize)
        {
            return Utility.JSON.ParseJSON(objectToDeserialize);
        }

        /// <summary>
        /// Parse a JSON string into an expando object, or a json value into a primitive type.
        /// </summary>
        /// <param name="objectToDeserialize"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Enumerate the values of the properties of an object to a sequence of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateProperties<T>(object obj)
        {
            return EnumerateProperties<T>(obj, new Type[] { typeof(ScriptIgnoreAttribute) });
        }

        /// <summary>
        /// Enumerate the values of the properties of an object to a sequence of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="ignoreAttributes">All properties with an attribute of these types will be ignored</param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateProperties<T>(object obj, IEnumerable<Type> ignoreAttributes)
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


        /// <summary>
        /// Provide simple user agent information
        /// </summary>
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
        
        #endregion

        #region private methods

        
        private static char[] StreamToCharArray(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
           
            long len = stream.Length;

            if (len>0 && len < int.MaxValue) {
                char[] arr = new char[stream.Length];
                reader.Read(arr,0,Convert.ToInt32(len));
                return arr;
            } else {
                return reader.ReadToEnd().ToCharArray();
            }
        }

        private static char[] ConvertHtmlString(string html)
        {
            return String.IsNullOrEmpty(html) ?
                null :
                html.ToCharArray();
        }
        #endregion
    }
}
