using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using HttpWebAdapters;

namespace CsQuery.Web
{
    /// <summary>
    /// A class encapsulating the functionality needed to make requests of remote web servers, and
    /// return the HTML as a CQ object.
    /// </summary>

    public class AsyncWebRequest : ICsqWebResponse
    {
        #region constructor

        /// <summary>
        /// Creates an AsyncWebRequest for a WebRequest
        /// </summary>
        ///
        /// <param name="request">
        /// The WebRequest object.
        /// </param>

        public AsyncWebRequest(IHttpWebRequest request)
        {
            Request = request;
            Timeout = request.Timeout;
            UserAgent = request.UserAgent;
        }

        #endregion

        #region private properties

        const int BUFFER_SIZE = 1024;

        /// <summary>
        /// A ManualResetEvent returned by the async request.
        /// </summary>

        protected ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Stream of the HTML response
        /// </summary>

        protected Stream ResponseStream { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the response stream
        /// </summary>

        public Encoding ResponseEncoding { get; set; }
        
        private WebException _WebException;
        
        #endregion


        #region public properties

        /// <summary>
        /// Delegate to invoke upon successful completion of a request
        /// </summary>
        public Action<ICsqWebResponse> CallbackSuccess { get; set; }

        /// <summary>
        /// Delegate to invoke when a request fails
        /// </summary>
        public Action<ICsqWebResponse> CallbackFail { get; set; }

        /// <summary>
        /// A unique identifier for this request
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// The URL of the request
        /// </summary>
        public string Url
        {
            get
            {
                return Request.RequestUri.AbsoluteUri;
            }
        }
        

        /// <summary>
        /// The time that the async request was initiated.
        /// </summary>
        public DateTime? Started { get; protected set; }

        /// <summary>
        /// The time that the async request was completed
        /// </summary>
        public DateTime? Finished { get; protected set; }
        
        /// <summary>
        /// Indicates that an async request has completed.
        /// </summary>
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

        /// <summary>
        /// True if the WebRequest was completed successfully.
        /// </summary>
        public bool Success
        {
            get;
            protected set;

        }

        /// <summary>
        /// When a request fails, contains the exception raised.
        /// </summary>
        public WebException WebException
        {
            get
            {
                return _WebException;
            }
            protected set
            {
                _WebException = value;
                Response = (IHttpWebResponse)value.Response;
            }
        }

        /// <summary>
        /// The WebRequest object
        /// </summary>

        public IHttpWebRequest Request
        {
            get;
            set;
        }

        /// <summary>
        /// Text of any error that occurred.
        /// </summary>

        public string Error
        {
            get
            {
                if (WebException == null)
                {
                    return "";
                }
                else
                {
                    return WebException.Message;
                }
            }
        }

        /// <summary>
        /// The HTTP status code for the response.
        /// </summary>

        public int HttpStatus
        {
            get
            {
                
                if (Response == null)
                {
                    return 0;
                }
                else
                {
                    return (int)Response.StatusCode;
                }
            }
        }

        /// <summary>
        /// The HTTP status description for the response.
        /// </summary>

        public string HttpStatusDescription
        {
            get
            {
                if (Response == null)
                {
                    return "";
                }
                else
                {
                    return Response.StatusDescription;
                }
            }
        }

        /// <summary>
        /// The async HttpWebResponse
        /// </summary>

        public IHttpWebResponse Response
        {
            get;
            protected set;


        }
       
        /// <summary>
        /// Return a document from the HTML web request result.
        /// </summary>
        
        public CQ Dom
        {
            get
            {

                return GetDocument();
            }
        }

        /// <summary>
        /// The HTML returned by the response.
        /// </summary>

        public string Html
        {
            get
            {
                ResponseStream.Position = 0;
                return new StreamReader(ResponseStream, ResponseEncoding).ReadToEnd();
            }
        }

        /// <summary>
        /// Return a CQ object, treating the HTML as a complete document
        /// </summary>
        public CQ GetDocument()
        {
            ResponseStream.Position = 0;
            return CQ.CreateDocument(ResponseStream, ResponseEncoding);
        }

        /// <summary>
        /// Return a CQ object, treating the HTML as content
        /// </summary>
        /// <returns></returns>
        public CQ GetContent() 
        {
            ResponseStream.Position = 0;
            return CQ.Create(ResponseStream, ResponseEncoding);
        }

        /// <summary>
        /// Begin the async request
        /// </summary>
        /// <returns></returns>
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

        
        #endregion

        #region private methods

        private void RespCallback(IAsyncResult ar)
        {

            // Get the RequestState object from the async result.
            WebRequestState rs = (WebRequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            IHttpWebRequest req = rs.Request;
            req.Timeout = Timeout;
            req.Headers["UserAgent"] = UserAgent;
            
            try
            {
               

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                Response = (IHttpWebResponse)req.EndGetResponse(ar);

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

            ResponseEncoding = CsqWebRequest.GetEncoding(Response);

            //  Start reading data from the response stream.
            Stream responseStream = Response.GetResponseStream();

            // Store the response stream in RequestState to read 
            // the stream asynchronously.
            rs.ResponseStreamAsync = responseStream;
            ResponseStream = new MemoryStream();

            
            //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
            IAsyncResult iarRead = responseStream.BeginRead(rs.BufferRead, 0,
               BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
        }


        private void ReadCallBack(IAsyncResult asyncResult)
        {
            Encoding encoding = Encoding.UTF8;
            
            // Get the RequestState object from AsyncResult.
            WebRequestState rs = (WebRequestState)asyncResult.AsyncState;
            
            // Retrieve the ResponseStream that was set in RespCallback. 
            Stream responseStream = rs.ResponseStreamAsync;


            // Read rs.BufferRead to verify that it contains data. 
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                // Convert byte stream to Char array and then to String.
                // len contains the number of characters converted to Unicode.
                ResponseStream.Write(rs.BufferRead, 0, read);

                // Continue reading data until 
                // responseStream.EndRead returns –1.
                IAsyncResult ar = responseStream.BeginRead(
                   rs.BufferRead, 0, BUFFER_SIZE,
                   new AsyncCallback(ReadCallBack), rs);
            }
            else
            {
                if (ResponseStream.Length > 0)
                {
                    ResponseStream.Position = 0;
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

        #endregion


       

        ServerConfig ICsqWebRequest.Options
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

        /// <summary>
        /// Gets or sets the timeout, in milliseconds, for this AsyncWebRequest
        /// </summary>

        public int Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>

        public string UserAgent
        {
            get;
            set;
        }
    }
}
