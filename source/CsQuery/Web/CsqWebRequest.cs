using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using CsQuery;
using CsQuery.ExtensionMethods;
using HttpWebAdapters;

namespace CsQuery.Web
{
    /// <summary>
    /// A CsqWebRequest object manages data and state related to a WebRequest
    /// </summary>

    public class CsqWebRequest: ICsqWebRequest
    {
        #region constructor

        /// <summary>
        /// Creates a new CsqWebRequest for a given URL using the default IHttpWebRequestFactory.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>

        public CsqWebRequest(string url)
        {
            Url = url;
            WebRequestFactory = Config.WebRequestFactory;
        }

        /// <summary>
        /// Creates a new CsqWebRequest for a URL using the provided IHttpWebRequestFactory. (Usually,
        /// you should use the default constructor, unless replacing the .NET framework HttpWebRequest
        /// object for testing or some other purpose)
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        /// <param name="webRequestFactory">
        /// The web request factory.
        /// </param>

        public CsqWebRequest(string url, IHttpWebRequestFactory webRequestFactory)
        {
            Url = url;
            WebRequestFactory = webRequestFactory;
        }

        #endregion

        #region private properties

        IHttpWebRequestFactory WebRequestFactory;
        Lazy<List<KeyValuePair<string, string>>> _PostData = new Lazy<List<KeyValuePair<string, string>>>();

        #endregion

        #region public properties
        
        /// <summary>
        /// The url to load.
        /// </summary>

        public string Url { get; protected set; }

        /// <summary>
        /// The UserAgent string to present to the remote server.
        /// </summary>

        public string UserAgent {get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether the asynchronous.
        /// </summary>

        public bool Async { get; set; }

        /// <summary>
        /// Returns true when this request has finished processing.
        /// </summary>

        public bool Complete { get; protected set; }

        /// <summary>
        /// The CQ object representing the contents of the URL.
        /// </summary>

        public CQ Dom { get; protected set; }

        /// <summary>
        /// The time, in milliseconds, after which to abort an incomplete request.
        /// </summary>

        public int Timeout { get; set; }

        /// <summary>
        /// A unique ID for this request. This will be automatically generated if not assigned.
        /// </summary>

        public object Id {get;set;}

        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>

        public string Html
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets the post data string.
        /// </summary>

        public string PostDataString
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

        /// <summary>
        /// Gets the information describing the post data to be sent this request.
        /// </summary>

        public List<KeyValuePair<string,string>> PostData 
        {
            get {
                return _PostData.Value;
            }
        }

        #endregion
        
        #region public methods

        /// <summary>
        /// Initiates an asynchronous GET request.
        /// </summary>
        ///
        /// <param name="success">
        /// A delegate that will be invoked with the response data structure upon successful resolution
        /// of the request.
        /// </param>
        /// <param name="fail">
        /// A delegate that will be invoked with the response data structure upon failure.
        /// </param>
        ///
        /// <returns>
        /// A ManualResetEvent object for this asynchronous operation.
        /// </returns>

        public ManualResetEvent GetAsync(Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail)
        {
            IHttpWebRequest request = GetWebRequest();
            var requestInfo = new AsyncWebRequest(request);
            requestInfo.Timeout = Timeout;
            requestInfo.UserAgent = UserAgent;
            requestInfo.Id = Id;
            requestInfo.CallbackSuccess  = success;
            requestInfo.CallbackFail = fail;

            return requestInfo.GetAsync();
        }

        /// <summary>
        /// Initiates an asynchronous GET request from an IHttpWebRequest object.
        /// </summary>
        ///
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="success">
        /// A delegate that will be invoked with the response data structure upon successful resolution
        /// of the request.
        /// </param>
        /// <param name="fail">
        /// A delegate that will be invoked with the response data structure upon failure.
        /// </param>
        ///
        /// <returns>
        /// A ManualResetEvent object for this asynchronous operation.
        /// </returns>

        public ManualResetEvent GetAsync(IHttpWebRequest request, Action<ICsqWebResponse> success, Action<ICsqWebResponse> fail)
        {
            var requestInfo = new AsyncWebRequest(request);
            requestInfo.Timeout = Timeout;
            requestInfo.UserAgent = UserAgent;
            requestInfo.Id = Id;
            requestInfo.CallbackSuccess  = success;
            requestInfo.CallbackFail = fail;

            return requestInfo.GetAsync();
        }


        /// <summary>
        /// Initiate a synchronous GET request.
        /// </summary>
        ///
        /// <returns>
        /// The HTML returned by a successful request
        /// </returns>

        public string Get()
        {
            IHttpWebRequest request = GetWebRequest();

            Html=null;

            using (StreamReader responseReader = GetResponseStreamReader(request)) {
                Html = responseReader.ReadToEnd();
            }
            return Html;
        }

        /// <summary>
        /// Initiate a synchronous GET request from an existing IHttpWebRequest object
        /// </summary>
        ///
        /// <param name="request">
        /// The request.
        /// </param>
        ///
        /// <returns>
        /// The HTML returned by a successful request.
        /// </returns>

        public string Get(IHttpWebRequest request)
        {
            Html = null;

            using (StreamReader responseReader = GetResponseStreamReader(request))
            {
                Html = responseReader.ReadToEnd();
            }
            return Html;
        }

        /// <summary>
        /// Gets a new HttpWebRequest object for the URL bound to this CsqWebRequest.
        /// </summary>
        ///
        /// <returns>
        /// An HttpWebRequest.
        /// </returns>

        public IHttpWebRequest GetWebRequest()
        {
            IHttpWebRequest request = WebRequestFactory.Create(new Uri(Url));
            
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }

        /// <summary>
        /// Initiate an http POST request.
        /// </summary>
        ///
        /// <returns>
        /// The data returned by the POST request
        /// </returns>

        public string Post() {
            return Post(Url,PostData);
        }

        /// <summary>
        /// Initiate an http POST request.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// The data returned by the POST request
        /// </returns>

        public string Post(string url) {
            return Post(url,PostData);
        }

        /// <summary>
        /// Initiate an http POST request.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        /// <param name="postData">
        /// The information describing the post data to be sent this request.
        /// </param>
        ///
        /// <returns>
        /// The data returned by the POST request.
        /// </returns>

        public string Post(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(PostDataString);

            IHttpWebRequest request = WebRequestFactory.Create(new Uri(Url));

            request.UserAgent = UserAgent ?? "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
            request.Method = HttpWebRequestMethod.POST;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            using (var response = GetResponseStreamReader(request))
            {
                Html = response.ReadToEnd();
            }
            return Html;
        }
        
        #endregion

        #region private methods

        /// <summary>
        /// Gets response stream from a webrequest using the correct encoding.
        /// </summary>
        ///
        /// <param name="request">
        /// The request.
        /// </param>
        ///
        /// <returns>
        /// The response stream.
        /// </returns>

        protected StreamReader GetResponseStreamReader(IHttpWebRequest request) {
            var response = request.GetResponse();
            
            //var response = request.GetResponse();
            //var encoding = response.Headers["
        
            StreamReader reader;
            var encoding = String.IsNullOrEmpty(response.CharacterSet) ?
                Encoding.UTF8 :
                Encoding.GetEncoding(response.CharacterSet);

            reader = new StreamReader(response.GetResponseStream(), encoding);
            return reader;
        }

        #endregion

    }
}
