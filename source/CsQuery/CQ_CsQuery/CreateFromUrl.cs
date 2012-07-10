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
        /// <summary>
        /// Creates a new DOM from an HTML file.
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <returns></returns>
        public static CQ CreateFromUrl(string url, ServerConfig options=null)
        {
            
            CsqWebRequest request = new CsqWebRequest(url);
            ServerConfig.Apply(options, request);

            request.Get();

            return CQ.CreateDocument(request.Html);
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
        /// Waits until all async events have completed. Use for testing primarily as a web app should not stop normally.
        /// </summary>
        /// <param name="millisecondsTimeout">The maximum number of milliseconds to wait</param>
        /// <returns>true if all events were cleared in the allotted time, false if not</returns>
        public static bool WaitForAsyncEvents(int timeout = -1)
        {
            return AsyncWebRequestManager.WaitForAsyncEvents(timeout);
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

    }
}
