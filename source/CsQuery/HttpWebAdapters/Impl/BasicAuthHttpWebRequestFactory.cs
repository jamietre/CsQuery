using System;
using System.Net;
using System.Text;
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {
    /// <summary>
    /// Creates a web request that does basic auth
    /// </summary>
    public class BasicAuthHttpWebRequestFactory : IHttpWebRequestFactory {
        private readonly string username;
        private readonly string password;

        /// <summary>
        /// Creates a web request that does basic auth.
        /// </summary>
        ///
        /// <param name="username">
        /// HTTP username.
        /// </param>
        /// <param name="password">
        /// HTTP password.
        /// </param>

        public BasicAuthHttpWebRequestFactory(string username, string password) {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Creates a web request for a url
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// A new IHttpWebRequest
        /// </returns>

        public IHttpWebRequest Create(string url) {
            return Create(new Uri(url));
        }

        /// <summary>
        /// Creates a concrete HttpWebRequest for a URI
        /// </summary>
        ///
        /// <param name="url">
        /// URI of the document.
        /// </param>
        ///
        /// <returns>
        /// A new IHttpWebRequest.
        /// </returns>

        public IHttpWebRequest Create(Uri url) {
            var req = (HttpWebRequest) WebRequest.Create(url);
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            req.Headers.Add("Authorization", "Basic " + credentials);
            return new HttpWebRequestAdapter(req);
        }
    }
}