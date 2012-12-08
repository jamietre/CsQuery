using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;

namespace CsQuery.Tests.Mocks
{
    /// <summary>
    /// Mock HTTP web request -- to generate certian content to test the CsQuery.Web stuff
    /// </summary>

    public class MockHttpWebRequest: HttpWebRequest
    {
        public new static MockHttpWebRequest Create(string uri)
        {
            MockHttpWebRequest request = (MockHttpWebRequest)FormatterServices.GetUninitializedObject(typeof(MockHttpWebRequest));
            request._RequestUri = new Uri(uri);
            return request;
        }
        [Obsolete]
        public MockHttpWebRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }


        private Uri _RequestUri;

        public override WebResponse GetResponse()
        {
            var response = (MockHttpWebResponse)FormatterServices.GetUninitializedObject(typeof(MockHttpWebResponse));
            response.ResponseHtml = ResponseHtml;
            if (!String.IsNullOrEmpty(CharacterSet))
            {
                response.AddHeader(HttpResponseHeader.ContentType, "text/html;charset=" + CharacterSet);
            }

            return response;
        }

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

        public override Uri RequestUri
        {
            get { return _RequestUri; }
        }
    }
}
