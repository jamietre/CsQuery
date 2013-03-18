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
        ///
        /// <param name="url">
        /// The URL of the remote server.
        /// </param>
        /// <param name="options">
        /// The options to use when creating the reqest.
        /// </param>
        ///
        /// <returns>
        /// A CQ object composed from the HTML response from the server.
        /// </returns>

        public static CQ CreateFromUrl(string url, ServerConfig options=null)
        {
            
            CsqWebRequest request = new CsqWebRequest(url);
            request.Options = options;

            var httpRequest = request.GetWebRequest();
            var response = httpRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var encoding = CsqWebRequest.GetEncoding(response);

            return CQ.CreateDocument(responseStream,encoding);
        }

        /// <summary>
        /// Start an asynchronous request to an HTTP server, returning a promise that will resolve when
        /// the request is completed or rejected.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the remote server
        /// </param>
        /// <param name="options">
        /// The options to use when creating the reqest
        /// </param>
        ///
        /// <returns>
        /// A promise that resolves when the request completes
        /// </returns>

        public static IPromise<ICsqWebResponse> CreateFromUrlAsync(string url, ServerConfig options = null)
        {
            var deferred = When.Deferred<ICsqWebResponse>();
            int uniqueID = AsyncWebRequestManager.StartAsyncWebRequest(url, deferred.Resolve, deferred.Reject, options);
            return deferred;
        }

        /// <summary>
        /// Start an asynchronous request to an HTTP server.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the remote server.
        /// </param>
        /// <param name="callbackSuccess">
        /// A delegate to invoke upon successful completion of the request.
        /// </param>
        /// <param name="callbackFail">
        /// A delegate to invoke upon failure.
        /// </param>
        /// <param name="options">
        /// Options to use when creating the request.
        /// </param>
        ///
        /// <returns>
        /// A unique identifier which will be passed through to the response and can be used to assocate
        /// a response with this request.
        /// </returns>

        public static int CreateFromUrlAsync(string url, Action<ICsqWebResponse> callbackSuccess, Action<ICsqWebResponse> callbackFail=null, ServerConfig options = null)
        {
            return AsyncWebRequestManager.StartAsyncWebRequest(url,callbackSuccess,callbackFail,options); 
            
        }


        /// <summary>
        /// Start an asynchronous request to an HTTP server.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the remote server.
        /// </param>
        /// <param name="id">
        /// An identifier that will be passed through to the response.
        /// </param>
        /// <param name="callbackSuccess">
        /// A delegate to invoke upon successful completion of the request.
        /// </param>
        /// <param name="callbackFail">
        /// A delegate to invoke upon failure.
        /// </param>
        /// <param name="options">
        /// Options to use when creating the request.
        /// </param>

        public static void CreateFromUrlAsync(string url, int id, Action<ICsqWebResponse> callbackSuccess, Action<ICsqWebResponse> callbackFail = null, ServerConfig options = null)
        {
            AsyncWebRequestManager.StartAsyncWebRequest(url, callbackSuccess, callbackFail,id, options);

        }

        /// <summary>
        /// Waits until all async events have completed. Use for testing primarily as a web app should
        /// not stop normally.
        /// </summary>
        ///
        /// <param name="timeout">
        /// The maximum number of milliseconds to wait.
        /// </param>
        ///
        /// <returns>
        /// true if all events were cleared in the allotted time, false if not.
        /// </returns>

        public static bool WaitForAsyncEvents(int timeout = -1)
        {
            return AsyncWebRequestManager.WaitForAsyncEvents(timeout);
        }

        /// <summary>
        /// Return a new promise that resolves when all the promises passed in are resolved.
        /// </summary>
        ///
        /// <param name="promises">
        /// One or more promises
        /// </param>
        ///
        /// <returns>
        /// A new promise
        /// </returns>

        public static IPromise WhenAll(params IPromise[] promises)
        {
            return When.All(promises);
        }

    }
}
