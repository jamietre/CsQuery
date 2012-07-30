using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{

    /// <summary>
    /// A group of async web requests. 
    /// NOT YET IMPLEMENTED
    /// </summary>
    /// 
    public interface IAsyncRequestQueue : ICsqWebRequest
    {
        /// <summary>
        /// Adds a request to the queue.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>

        void AddRequest(string url);

        /// <summary>
        /// Adds a request to the queue.
        /// </summary>
        ///
        /// <param name="request">
        /// The request.
        /// </param>

        void AddRequest(ICsqWebRequest request);

        /// <summary>
        /// A sequence of responses from the completed requests
        /// </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process results in this collection.
        /// </returns>

        IEnumerable<ICsqWebResponse> Results();

        /// <summary>
        /// Gets the state of the request queue.
        /// </summary>

        RequestState State { get; }

    }
}
