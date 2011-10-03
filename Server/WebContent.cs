using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Jtc.CsQuery;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Server
{
    public class WebData
    {
        public string Url {get;set;}
        public string UserAgent {get;set;}

        public string Get(string url) {
            Url=url;
            return Get();
        }
        public string Html
        {
            get;set;
        }
        public string PostDataText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kvp in PostData) {
                    sb.Append((sb.Length==0?"":"&") + HttpUtility.UrlEncode(kvp.Key+ "=" + kvp.Value));
                }
                return sb.ToString();
            }
            set
            {
                PostData.Clear();
                string[] pairs = value.Split('&');
                string key=String.Empty;
                string val=String.Empty;
                foreach (string item in pairs) {
                    int pos = item.IndexOf("=");
 
                    if (pos>0) {
                        key = item.Substring(0,pos);
                        val = item.Substring(pos+1);
                    } else {
                        key=item;
                    }
                    PostData.Add(new KeyValuePair<string,string>(key,val));
                }
            }
        }
        Lazy<List<KeyValuePair<string,string>>> _PostData = new Lazy<List<KeyValuePair<string,string>>>();
        public List<KeyValuePair<string,string>> PostData 
        {
            get {
                return _PostData.Value;
            }
        }
        Lazy<Dictionary<int,RequestInfo>> _AsyncRequests = new Lazy<Dictionary<int,RequestInfo>>();
        public Dictionary<int,RequestInfo> AsyncRequests
        {
            get
            {
                return _AsyncRequests.Value;
            }
        }

        public void GetAsync(Action<CsQuery, bool> callback)
        {
            var request = WebRequest.Create(Url);

        }
        public string Get()
        {
           // HttpWebRequest webRequest = default(HttpWebRequest);
           // var async = WebRequest.
            var request = WebRequest.Create(Url);
            
            Html=null;
            using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream())) {
                Html = responseReader.ReadToEnd();
            }
            return Html;
        }
        public string Post() {
            return Post(Url,PostData);
        }
        public string Post(string url) {
            return Post(url,PostData);
        }
        public string Post(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(PostDataText);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            using (var response = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                Html = response.ReadToEnd();
            }
            return Html;
        }
    }
    public class RequestInfo
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public RequestInfo(HttpWebRequest request)
        {
            Request = request;
        }
        public void GetAsync()
        {

            // Create the state object.
            //RequestState rs = new RequestState(this);

            // Put the request into the state object so it can be passed around.
            //rs.Request = Request;

            // Issue the async request.
            IAsyncResult r = (IAsyncResult)Request.BeginGetResponse(
               new AsyncCallback(RespCallback),this);
        }
        private static void RespCallback(IAsyncResult ar)
        {
            // Get the RequestState object from the async result.
            RequestState rs = (RequestState)ar.AsyncState;

            // Get the WebRequest from RequestState.
            WebRequest req = rs.Request;

            // Call EndGetResponse, which produces the WebResponse object
            //  that came from the request issued above.
            WebResponse resp = req.EndGetResponse(ar);

            //  Start reading data from the response stream.
            Stream ResponseStream = resp.GetResponseStream();

            // Store the response stream in RequestState to read 
            // the stream asynchronously.
            rs.ResponseStream = ResponseStream;

            //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
            IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0,
               BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
        }
        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            // Get the RequestState object from AsyncResult.
            RequestState rs = (RequestState)asyncResult.AsyncState;

            // Retrieve the ResponseStream that was set in RespCallback. 
            Stream responseStream = rs.ResponseStream;

            // Read rs.BufferRead to verify that it contains data. 
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                // Prepare a Char array buffer for converting to Unicode.
                Char[] charBuffer = new Char[BUFFER_SIZE];

                // Convert byte stream to Char array and then to String.
                // len contains the number of characters converted to Unicode.
                int len =
                   rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);

                String str = new String(charBuffer, 0, len);

                // Append the recently read data to the RequestData stringbuilder
                // object contained in RequestState.
                rs.RequestData.Append(
                   Encoding.ASCII.GetString(rs.BufferRead, 0, read));

                // Continue reading data until 
                // responseStream.EndRead returns –1.
                IAsyncResult ar = responseStream.BeginRead(
                   rs.BufferRead, 0, BUFFER_SIZE,
                   new AsyncCallback(ReadCallBack), rs);
            }
            else
            {
                if (rs.RequestData.Length > 0)
                {
                    //  Display data to the console.
                   //rs.RequestInfo=
                }
                // Close down the response stream.
                responseStream.Close();
                // Set the ManualResetEvent so the main thread can exit.
                allDone.Set();
            }
            return;
        }    

        public string Url { 
            get {
                return Request.RequestUri.AbsoluteUri;
            }}
        public DateTime Started {get;set;}
        public DateTime Finished {get;set;}
        public int ID;
        public bool Complete { get; set; }
        public HttpWebRequest Request
        {
           get;set;
        }
        public string Html
        { get; set; }

    }
    public class RequestState
    {
        const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        // Create Decoder for appropriate enconding type.
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
        public RequestInfo RequestInfo;

        public RequestState(RequestInfo requestInfo)
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = null;
            ResponseStream = null;
            RequestInfo = requestInfo;
        }
    }
}
