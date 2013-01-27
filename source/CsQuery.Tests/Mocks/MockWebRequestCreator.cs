using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HttpWebAdapters;

namespace CsQuery.Tests.Mocks
{
    public class MockWebRequestCreator: IHttpWebRequestFactory
    {

        public IHttpWebRequest Create(Uri url)
        {
            var req= MockHttpWebRequest.Create(url);
            req.CharacterSet = CharacterSet;
            req.ResponseStream = ResponseStream;
            req.ResponseTime = ResponseTime;
            return req;
        }
        public string CharacterSet { get; set; }
        private Stream _ResponseStream;
        public Stream ResponseStream
        {
            get
            {
                var stream = new MemoryStream();
                
                _ResponseStream.Position = 0;
                _ResponseStream.CopyTo(stream);
                stream.Position = 0;
                _ResponseStream.Position = 0;
                return stream;
            }
            set
            {
                _ResponseStream = value;
            }
        }
        public int ResponseTime { get; set; }
    }
}
