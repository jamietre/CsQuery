using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace CsQuery.Web
{
    /// <summary>
    /// A controller for creating and managing asynchronous web requests
    /// </summary>

    public static class AsyncWebRequestManager
    {

        #region private properties

        private static int LastAsyncRequestID = 0;
        private static object Locker = new object();
        private static ConcurrentBag<ManualResetEvent> AsyncEvents
        {
            get
            {
                return _AsyncEvents.Value;
            }
        }
        private static Lazy<ConcurrentBag<ManualResetEvent>> _AsyncEvents = new Lazy<ConcurrentBag<ManualResetEvent>>();

        #endregion

        #region public methods

        /// <summary>
        /// Start an async request, and return a unique ID that identifies it.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the remote server.
        /// </param>
        /// <param name="success">
        /// A delegate to invoke upon successful completion of the request.
        /// </param>
        /// <param name="fail">
        /// A delegate to invoke when a request fails.
        /// </param>
        /// <param name="options">
        /// Options to be used when creating this request. If not provided, the default options will be
        /// used.
        /// </param>
        ///
        /// <returns>
        /// A unique identifier that can be used to track this request when it resolves.
        /// </returns>

        public static int StartAsyncWebRequest(string url, Action<ICsqWebResponse> success, 
            Action<ICsqWebResponse> fail, ServerConfig options = null)
        {
            int id = GetAsyncRequestID();
            StartAsyncWebRequest(url, success, fail, id, options);
            return id;
        }

        /// <summary>
        /// Start an async request from an ICsqWebRequest object
        /// </summary>
        ///
        /// <param name="request">
        /// The URL of the remote server.
        /// </param>
        /// <param name="success">
        /// A delegate to invoke upon successful completion of the request.
        /// </param>
        /// <param name="fail">
        /// A delegate to invoke when a request fails.
        /// </param>

        public static void StartAsyncWebRequest(ICsqWebRequest request, Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail)
        {
            var requestObj = (CsqWebRequest)request;
            requestObj.Async = true;

            var mrEvent = requestObj.GetAsync(success, fail);
            AsyncEvents.Add(mrEvent);
        }

        /// <summary>
        /// Start an async request identified by a user-supplied ID.
        /// </summary>
        ///
        /// <param name="url">
        /// The URL of the remote server.
        /// </param>
        /// <param name="success">
        /// A delegate to invoke upon successful completion of the request.
        /// </param>
        /// <param name="fail">
        /// A delegate to invoke when a request fails.
        /// </param>
        /// <param name="id">
        /// The identifier.
        /// </param>
        /// <param name="options">
        /// Options to be used when creating this request. If not provided, the default options will be
        /// used.
        /// </param>

        public static void StartAsyncWebRequest(string url, Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail, int id, ServerConfig options = null)
        {
            var request = new CsqWebRequest(url);
            ServerConfig.Apply(options, request);

            request.Id = id;
            request.Async = true;
            
            var mrEvent = request.GetAsync(success, fail);
            AsyncEvents.Add(mrEvent);
        }

       

        /// <summary>
        /// Waits until all async events have completed. Use for testing primarily as a web app should
        /// not stop normally.
        /// </summary>
        ///
        /// <param name="millisecondsTimeout">
        /// The maximum number of milliseconds to wait.
        /// </param>
        ///
        /// <returns>
        /// true if all events were cleared in the allotted time, false if not.
        /// </returns>

        public static bool WaitForAsyncEvents(int millisecondsTimeout = -1)
        {
            ManualResetEvent evt;
            int timeLeft = millisecondsTimeout;
            DateTime start = DateTime.Now;

            while (AsyncEvents.TryTake(out evt) && timeLeft != 0)
            {
                if (!evt.SafeWaitHandle.IsClosed)
                {
                    evt.WaitOne(timeLeft);
                    if (timeLeft >= 0)
                    {
                        // subtract elapsed time from the total timeout for waiting on all threads
                        timeLeft = Math.Max(0, millisecondsTimeout - (int)(DateTime.Now - start).TotalMilliseconds);
                    }
                }
            }
            return timeLeft > 0;
        }

        /// <summary>
        /// Cancel all outstanding async events.
        /// </summary>

        public static void CancelAsyncEvents()
        {
            foreach(var evt in AsyncEvents) {
                evt.Close();
            }
            
        }

        /// <summary>
        /// Gets the asynchronous request identifier.
        /// </summary>
        ///
        /// <returns>
        /// The asynchronous request identifier.
        /// </returns>

        private static int GetAsyncRequestID()
        {
            lock (Locker)
            {
                return ++LastAsyncRequestID;
            }
        }

        #endregion
    
    }
}
