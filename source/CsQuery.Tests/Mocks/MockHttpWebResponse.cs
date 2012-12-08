using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
namespace CsQuery.Tests.Mocks
{
    public class MockHttpWebResponse: HttpWebResponse
    {
        [Obsolete]
        public MockHttpWebResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        /// <summary>
        /// Gets or sets the HTML that will be sent as a response.
        /// </summary>

        public string ResponseHtml { get; set; }

        /// <summary>
        /// Gets the stream that is used to read the body of the response from the server.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="T:System.IO.Stream" /> containing the body of the response.
        /// </returns>

        public void AddHeader(HttpResponseHeader header, string value)
        {
            _Headers.Add(header,value);
        }

        public override Stream GetResponseStream()
        {
            var output = new MemoryStream();
            
            Encoding encoding = String.IsNullOrEmpty(CharacterSet) ?
                Encoding.UTF8 :
                Encoding.GetEncoding(CharacterSet);
            
            var writer = new StreamWriter(output,encoding);

            writer.Write(ResponseHtml);
            return output;
        }

        protected WebHeaderCollection _Headers = new WebHeaderCollection();

        public override WebHeaderCollection Headers { 
            get
            {
                return _Headers;
            }
        }
        
    }
}
