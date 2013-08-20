using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using HttpWebAdapters;

namespace CsQuery.Tests.Mocks
{
    public class MockHttpWebResponse: IHttpWebResponse
    {
        public MockHttpWebResponse()
        {
            ContentType = "text/html";
        }
       
        /// <summary>
        /// Gets or sets the HTML that will be sent as a response.
        /// </summary>

        public Stream ResponseStream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the stream that is used to read the body of the response from the server.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="T:System.IO.Stream" /> containing the body of the response.
        /// </returns>

        public void AddHeader(HttpResponseHeader header, string value)
        {
            Headers.Add(header,value);
        }

        public Stream GetResponseStream()
        {
            return ResponseStream;

        }

        protected WebHeaderCollection _Headers;

        public WebHeaderCollection Headers { 
            get
            {
                if (_Headers==null) {
                     _Headers= new WebHeaderCollection();
                }
                return _Headers;
            }
        }


        public string GetResponseHeader(string headerName)
        {
            throw new NotImplementedException();
        }

        public CookieCollection Cookies
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

        public string ContentEncoding
        {
            get;set; 
        }

        public string CharacterSet
        {
            get;set;
        }

        public string Server
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastModified
        {
            get { throw new NotImplementedException(); }
        }

        public HttpStatusCode StatusCode
        {
            get { throw new NotImplementedException(); }
        }

        public string StatusDescription
        {
            get { throw new NotImplementedException(); }
        }

        public Version ProtocolVersion
        {
            get { throw new NotImplementedException(); }
        }

        public string Method
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool IsFromCache
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMutuallyAuthenticated
        {
            get { throw new NotImplementedException(); }
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

        private string _ContentType;
        public string ContentType
        {
            get
            {
                return _ContentType +
                    (!String.IsNullOrEmpty(CharacterSet) ? 
                    "; charset=" + CharacterSet :
                    "");
            }
            set
            {
                 _ContentType = value;
            }
           
        }

        public Uri ResponseUri
        {
            get; set; 
        }

        public void Dispose()
        {
           
        }
    }
}
