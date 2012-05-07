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

namespace CsQuery.Web
{
    public class CsqWebRequest: ICsqWebRequest 
    {
        public CsqWebRequest(string url)
        {
            Url = url;
        }
        public string Url { get; protected set; }
        public string UserAgent {get;set;}
        public bool Async { get; set; }
        public bool Complete { get; protected set; }
        public CQ Dom { get; protected set; }
        public int Timeout { get; set; }
        public object Id {get;set;}

        //public string Get(string url) {
        //    Url=url;
        //    return Get();
        //}
        public string Html
        {
            get;set;
        }
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


        public List<KeyValuePair<string,string>> PostData 
        {
            get {
                return _PostData.Value;
            }
        }
        Lazy<List<KeyValuePair<string, string>>> _PostData = new Lazy<List<KeyValuePair<string, string>>>();

        public ManualResetEvent GetAsync(Action<ICsqWebResponse> callback)
        {
            HttpWebRequest request = GetWebRequest();
            var requestInfo = new AsyncWebRequest(request);
            requestInfo.Id = Id;
            requestInfo.Callback = callback;

            return requestInfo.GetAsync();
        }
        
        /// <summary>
        /// Initiate an http GET request
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
           // HttpWebRequest webRequest = default(HttpWebRequest);
           // var async = WebRequest.
            HttpWebRequest request = GetWebRequest();
            Html=null;
            using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream())) {
                Html = responseReader.ReadToEnd();
            }
            return Html;
        }
        protected HttpWebRequest GetWebRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            return request;
        }
        /// <summary>
        /// Initiaite an http POST request
        /// </summary>
        /// <returns></returns>
        public string Post() {
            return Post(Url,PostData);
        }
        public string Post(string url) {
            return Post(url,PostData);
        }
        public string Post(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(PostDataString);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.UserAgent = UserAgent ?? "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
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
   
  
}
