using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace CsQuery.Web
{
    internal static class AsyncWebRequestManager
    {
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

        #region public methods

        /// <summary>
        /// Start an async request, and return a unique ID that identifies it.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int StartAsyncWebRequest(string url, Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail, ServerConfig options = null)
        {
            int id = GetAsyncRequestID();
            StartAsyncWebRequest(url, success, fail, id, options);
            return id;
        }

        public static void StartAsyncWebRequest(string url, Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail, int id, ServerConfig options = null)
        {
            var request = new CsqWebRequest(url);
            request.Id = id;
            request.Async = true;

            var mrEvent = request.GetAsync(success, fail);
            AsyncEvents.Add(mrEvent);
        }

        /// <summary>
        /// Waits until all async events have completed. Use for testing primarily as a web app should not stop normally.
        /// </summary>
        public static void WaitForAsyncEvents(int millisecondsTimeout = -1)
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
            DateTime endTime = DateTime.Now;
        }

        public static int GetAsyncRequestID()
        {
            lock (Locker)
            {
                return ++LastAsyncRequestID;
            }
        }

        #endregion
    }

}
