using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpWebAdapters;

namespace CsQuery.Tests.Mocks
{
    public class MockWebRequestCreator: IHttpWebRequestFactory
    {

        public IHttpWebRequest Create(Uri url)
        {
            var req= MockHttpWebRequest.Create(url);
            req.CharacterSet = CharacterSet;
            req.ResponseHtml = ResponseHTML;
            return req;
        }
        public string CharacterSet { get; set; }
        public string ResponseHTML { get; set; }
    }
}
