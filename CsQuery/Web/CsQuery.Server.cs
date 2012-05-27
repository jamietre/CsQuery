using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Threading;
using CsQuery.Web;
using CsQuery.Utility;

/*
 * Plugin for parsing server posted data. Just add this namespace.
 */
namespace CsQuery
{
    public static class Server
    {
        /*
         * TODO: A bunch of cleanup. Make the Response method inherit the Request method. Pull the async code
         * out. Let the timeouts work. Add an error callback.
         */

        
        public static string UserAgent { get; set; }
        public static CQ CreateFromUrl(string url)
        {
            CsqWebRequest con = new CsqWebRequest(url);
            con.Get();
            con.UserAgent = UserAgent;
            return CQ.Create(con.Html);
        }
        public static SimpleDictionary<string> PostData()
        {
            return PostData(HttpContext.Current);
        }
        public static SimpleDictionary<string> PostData(HttpContext context)
        {
             return new SimpleDictionary<string>(context.Request.Form);
        }
        


        public static CsQueryHttpContext CreateFromRender(Page page, Action<HtmlTextWriter> renderMethod, HtmlTextWriter writer)
        {
            return CreateFromRender(page, renderMethod, writer, HttpContext.Current);
        }

        public static void StartAsyncWebRequest(string url, Action<ICsqWebResponse> callback, object id=null)
        {

            var request = new CsqWebRequest(url);
            request.Id = id;
            request.Async = true;

            var mrEvent = request.GetAsync(callback);
            AsyncEvents.Add(mrEvent);

        }

        /// <summary>
        /// Creates a new CSQuery object from a Page.Render method. The base Render method of a page should be overridden,
        /// and this called from inside it to configure the CsQUery
        /// </summary>
        /// <param name="page">The current System.Web.UI.Page</param>
        /// <param name="renderMethod">The delegate to the base render method</param>
        /// <param name="writer">The HtmlTextWriter to output the final stream (the parameter passed to the Render method)</param>
        public static CsQueryHttpContext CreateFromRender(
            Page page,
            Action<HtmlTextWriter> renderMethod,
            HtmlTextWriter writer,
            HttpContext context)
        {
            CsQueryHttpContext csqContext = new CsQueryHttpContext(context);
            csqContext.RealWriter = writer;
            csqContext.Page = page;
            csqContext.ControlRenderMethod = renderMethod;
            csqContext.Create();
            return csqContext;

        }

        /// <summary>
        /// Waits until all async events have completed. Use for testing primarily as a web app should not stop normally.
        /// </summary>
        public static void WaitForAsyncEvents(int millisecondsTimeout=-1)
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
        internal static ConcurrentBag<ManualResetEvent> AsyncEvents
        {
            get
            {
                return _AsyncEvents.Value;
            }
        }
        private static Lazy<ConcurrentBag<ManualResetEvent>> _AsyncEvents = new Lazy<ConcurrentBag<ManualResetEvent>>();
    }

}
