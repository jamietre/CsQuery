using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.Web;
using CsQuery.Promises;
using CsQuery.HtmlParser;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        #region CsQuery global options

        /// <summary>
        /// Rendering option flags
        /// </summary>
        public static DomRenderingOptions DefaultDomRenderingOptions 
        {
            get {
                return Config.DomRenderingOptions;
            }
            set {
                Config.DomRenderingOptions = value;
            }
        }

       

        /// <summary>
        /// The default rendering type. 
        /// </summary>
        public static DocType DefaultDocType {
            get
            {
                return Config.DocType;
            }
            set
            {
                Config.DocType = value;
            }
        }

        #endregion 

        #region private properties
        
        private static Browser _Browser;
        
        #endregion

        #region static utility methods

       
      

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

       
        #endregion
    }
}
