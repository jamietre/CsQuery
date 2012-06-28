using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CsQuery.Web
{
    public class WebRequestState
    {
        const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        // Create Decoder for appropriate enconding type.
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
        public AsyncWebRequest RequestInfo;

        public WebRequestState(AsyncWebRequest requestInfo)
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = requestInfo.Request;
            ResponseStream = null;
            RequestInfo = requestInfo;
        }
    }
}
