using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace CsQuery.Web
{
    public class AsyncWebRequest : ICsqWebResponse
    {
        protected ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public AsyncWebRequest(WebRequest request)
        {
            Request = request;
        }
        public object Id { get; set; }

        public string Url
        {
            get
            {
                return Request.RequestUri.AbsoluteUri;
            }
        }

        public int Timeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string UserAgent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public DateTime Started { get; protected set; }
        public DateTime Finished { get; protected set; }

        public bool Complete { get; set; }
        public WebRequest Request
        {
            get;
            set;
        }
       
        public string Html
        {
            get
            {
                return HtmlStringbuilder.ToString();
            }
        }
        public CQ Dom
        {
            get
            {
                return CQ.Create(Html);
            }
        }
        protected StringBuilder HtmlStringbuilder { get; set; }
        protected Stream ResponseStream { get; set; }

        public Action<ICsqWebResponse> Callback { get; set; }

        public ManualResetEvent GetAsync()
        {

            // Create the state object.
            WebRequestState rs = new WebRequestState(this);
            
            // Put the request into the state object so it can be passed around.
            rs.Request = Request;

            // Issue the async request.

            IAsyncResult r = (IAsyncResult)Request.BeginGetResponse(
               new AsyncCallback(RespCallback), rs);
            return allDone;
        }
        private void RespCallback(IAsyncResult ar)
        {
            // Get the RequestState object from the async result.
            WebRequestState rs = (WebRequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            WebRequest req = rs.Request;

            // Call EndGetResponse, which produces the WebResponse object
            //  that came from the request issued above.
            WebResponse resp = req.EndGetResponse(ar);

            //  Start reading data from the response stream.
            Stream ResponseStream = resp.GetResponseStream();

            // Store the response stream in RequestState to read 
            // the stream asynchronously.
            rs.ResponseStream = ResponseStream;

            //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
            IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0,
               BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
        }
        private void ReadCallBack(IAsyncResult asyncResult)
        {
            // Get the RequestState object from AsyncResult.
            WebRequestState rs = (WebRequestState)asyncResult.AsyncState;

            // Retrieve the ResponseStream that was set in RespCallback. 
            Stream responseStream = rs.ResponseStream;

            // Read rs.BufferRead to verify that it contains data. 
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                // Prepare a Char array buffer for converting to Unicode.
                Char[] charBuffer = new Char[BUFFER_SIZE];

                // Convert byte stream to Char array and then to String.
                // len contains the number of characters converted to Unicode.
                int len =
                   rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                String str = new String(charBuffer, 0, len);

                // Append the recently read data to the RequestData stringbuilder
                // object contained in RequestState.
                rs.RequestData.Append(
                 Encoding.UTF8.GetString(rs.BufferRead, 0, read));

                // Continue reading data until 
                // responseStream.EndRead returns –1.
                IAsyncResult ar = responseStream.BeginRead(
                   rs.BufferRead, 0, BUFFER_SIZE,
                   new AsyncCallback(ReadCallBack), rs);
            }
            else
            {
                if (rs.RequestData.Length > 0)
                {
                    //  Display data to the console.
                    HtmlStringbuilder = rs.RequestData;

                    if (Callback != null)
                    {
                        Callback( this);
                    }
                }
                // Close down the response stream.
                responseStream.Close();
                // Set the ManualResetEvent so the main thread can exit.

                allDone.Set();
            }
            return;
        }




    }
}
