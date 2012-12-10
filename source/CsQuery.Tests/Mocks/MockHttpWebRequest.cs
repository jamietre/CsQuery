using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;
using HttpWebAdapters;

namespace CsQuery.Tests.Mocks
{
    /// <summary>
    /// Mock HTTP web request -- to generate certian content to test the CsQuery.Web stuff
    /// </summary>

    public class MockHttpWebRequest: IHttpWebRequest
    {
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

        public string ResponseHtml { get; set; }

        public Uri RequestUri
        {
            get { return _RequestUri; }
        }

        public HttpWebRequestMethod Method
        {
            get;
            set;
        }

        IHttpWebResponse IHttpWebRequest.GetResponse()
        {
            var response = new MockHttpWebResponse();
            response.CharacterSet = CharacterSet;
            response.ResponseHtml = ResponseHtml;
            if (!String.IsNullOrEmpty(CharacterSet))
            {
                response.AddHeader(HttpResponseHeader.ContentType, "text/html;charset=" + CharacterSet);
            }

            return response;
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IHttpWebResponse EndGetResponse(IAsyncResult result)
        {
            throw new NotImplementedException();
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
