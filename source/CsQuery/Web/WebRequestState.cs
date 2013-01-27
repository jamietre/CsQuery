using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HttpWebAdapters;

namespace CsQuery.Web
{
    /// <summary>
    /// Web request state: a class encapsulating information about an async web request
    /// </summary>

    public class WebRequestState
    {
        const int BufferSize = 1024;

        /// <summary>
        /// A buffer
        /// </summary>

        public byte[] BufferRead;

        /// <summary>
        /// The WebRequest
        /// </summary>

        public IHttpWebRequest Request;

        /// <summary>
        /// The response stream.
        /// </summary>

        public Stream ResponseStreamAsync;


        /// <summary>
        /// Information describing the request.
        /// </summary>

        public AsyncWebRequest RequestInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="requestInfo">
        /// Information describing the request.
        /// </param>

        public WebRequestState(AsyncWebRequest requestInfo)
        {
            BufferRead = new byte[BufferSize];
            Request = requestInfo.Request;
            ResponseStreamAsync = null;
            RequestInfo = requestInfo;
        }
    }
}
