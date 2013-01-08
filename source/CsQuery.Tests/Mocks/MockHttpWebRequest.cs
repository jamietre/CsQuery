using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;
using CsQuery.Web;

using HttpWebAdapters;

namespace CsQuery.Tests.Mocks
{
    /// <summary>
    /// Mock HTTP web request -- to generate certian content to test the CsQuery.Web stuff
    /// </summary>

    public class MockHttpWebRequest: IHttpWebRequest
    {
        public MockHttpWebRequest()
        {
            ResponseTime = 100;
            Timeout = 1200;
            Headers = new WebHeaderCollection();
        }
        /// <summary>
        /// Creates a new mock MockHttpWebRequest.
        /// </summary>
        ///
        /// <param name="uri">
        /// URI of the document.
        /// </param>
        ///
        /// <returns>
        /// The new MockHttpWebRequest.
        /// </returns>

        public static MockHttpWebRequest Create(Uri uri)
        {
            MockHttpWebRequest request = new MockHttpWebRequest();
            request._RequestUri = uri;
            return request;
        }

        /// <summary>
        /// Gets or sets the time that a response should take when using the mock.
        /// </summary>

        public int ResponseTime { get; set; }

        private Uri _RequestUri;

        /// <summary>
        /// Gets or sets the value of the Content-type HTTP header that will be passed with the response.
        /// </summary>
        ///
        /// <value>
        /// The value of the Content-type HTTP header. The default value is null.
        /// </value>

        public string CharacterSet { get; set; }

        /// <summary>
        /// Gets or sets the HTML that will be sent as a response.
        /// </summary>

        public Stream ResponseStream { get; set; }

        public Uri RequestUri
        {
            get { return _RequestUri; }
        }

        public HttpWebRequestMethod Method
        {
            get;
            set;
        }

        private IHttpWebResponse GetResponse()
        {
            var response = new MockHttpWebResponse();
            response.CharacterSet = CharacterSet;
            response.ResponseStream = ResponseStream;

            if (!String.IsNullOrEmpty(CharacterSet))
            {
                response.AddHeader(HttpResponseHeader.ContentType, "text/html;charset=" + CharacterSet);
            }

            if (ResponseTime > Timeout)
            {
                 throw new WebException("The response was not receved within " + Timeout + " milliseconds.",WebExceptionStatus.Timeout);
            }

            return response;
        }

        IHttpWebResponse IHttpWebRequest.GetResponse()
        {
            return GetResponse();
        }

        public System.IO.Stream GetRequestStream()
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void AddRange(int from, int to)
        {
            throw new NotImplementedException();
        }

        public void AddRange(int range)
        {
            throw new NotImplementedException();
        }

        public void AddRange(string rangeSpecifier, int from, int to)
        {
            throw new NotImplementedException();
        }

        public void AddRange(string rangeSpecifier, int range)
        {
            throw new NotImplementedException();
        }

        public bool AllowAutoRedirect
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

        public bool AllowWriteStreamBuffering
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

        public bool HaveResponse
        {
            get { throw new NotImplementedException(); }
        }

        public bool KeepAlive
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

        public bool Pipelined
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

        public bool PreAuthenticate
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

        public bool UnsafeAuthenticatedConnectionSharing
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

        public bool SendChunked
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

        public DecompressionMethods AutomaticDecompression
        {
            get;
            set;
        }

        public int MaximumResponseHeadersLength
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

        public System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
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

        public CookieContainer CookieContainer
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

        public long ContentLength
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

        public int Timeout
        {
            get;
            set;
        }

        public int ReadWriteTimeout
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

        public Uri Address
        {
            get { throw new NotImplementedException(); }
        }

        public ServicePoint ServicePoint
        {
            get { throw new NotImplementedException(); }
        }

        public int MaximumAutomaticRedirections
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

        public ICredentials Credentials
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

        public bool UseDefaultCredentials
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

        public string ConnectionGroupName
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

        public WebHeaderCollection Headers
        {
            get;
            set;
        }

        public IWebProxy Proxy
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

        public Version ProtocolVersion
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

        public string ContentType
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

        public string MediaType
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

        public string TransferEncoding
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

        public string Connection
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

        public string Accept
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

        public string Referer
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
                return Headers["UserAgent"];
            }
            set
            {
                Headers["UserAgent"] = value;
            }
        }

        public string Expect
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

        public DateTime IfModifiedSince
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

        MockAsyncResult AsyncResult;
        Timer AsyncTimer;
        private AsyncCallback Callback;

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            AsyncResult = new MockAsyncResult
            {
                 IsCompleted = false,
                 AsyncState=state,
                 CompletedSynchronously = false,
                 AsyncWaitHandle = GetWaitHandle()
            };
            Callback = callback;
            return AsyncResult;

            
        }

        private WaitHandle GetWaitHandle()
        {
            var cb = new TimerCallback(new Action<object>((stateInfo)=> {
                AsyncResult.IsCompleted = true;
                Callback.DynamicInvoke(AsyncResult);
            }));
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            AsyncTimer = new System.Threading.Timer(cb, autoEvent, ResponseTime, System.Threading.Timeout.Infinite);
            return autoEvent;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        public IHttpWebResponse EndGetResponse(IAsyncResult result)
        {
            return GetResponse();
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream EndGetRequestStream(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        
    }
}
