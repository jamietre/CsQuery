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
        public AsyncWebRequest(WebRequest request)
        {
            Request = request;
        }
        protected ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        protected StringBuilder HtmlStringbuilder { get; set; }
        protected Stream ResponseStream { get; set; }

        

        #region public properties

        public Action<ICsqWebResponse> CallbackSuccess { get; set; }
        public Action<ICsqWebResponse> CallbackFail { get; set; }

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
            get;
            set;
        }

        public string UserAgent
        {
            get;
            set;
        }
        public DateTime? Started { get; protected set; }
        public DateTime? Finished { get; protected set; }
        
        public bool Complete
        {
            get
            {
                return Finished != null;
            }
            protected set
            {
                if (value == true)
                {
                    Finished = DateTime.Now;
                }
                else
                {
                    throw new InvalidOperationException("You can only set complete to True.");
                }
            }
        }

        public bool Success
        {
            get;
            protected set;

        }
        public WebException WebException
        {
            get;
            protected set;
        }
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

        #endregion


        public ManualResetEvent GetAsync()
        {
            Started = DateTime.Now;

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
            req.Timeout = Timeout;
            req.Headers["UserAgent"] = UserAgent;

            WebResponse resp;
            try
            {
               

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                resp = req.EndGetResponse(ar);

            }
            catch (WebException e)
            {
                Complete = true;
                Success = false;
                WebException = e;

                if (CallbackFail != null)
                {
                    CallbackFail(this);
                }
                allDone.Set();
                return;
            }

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
                    HtmlStringbuilder = rs.RequestData;
                    Complete = true;
                    Success = true;

                    if (CallbackSuccess != null)
                    {
                        CallbackSuccess(this);
                    }
                }
                else
                {
                    Complete = true;
                    Success = false;
                    if (CallbackFail != null)
                    {
                        CallbackFail(this);
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
