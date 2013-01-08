using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{
    /// <summary>
    /// Interface representing a CsQuery web request.
    /// </summary>

    public interface ICsqWebRequest : ICsqWebRequestMetadata
    {
        /// <summary>
        /// The url to load.
        /// </summary>

        string Url { get;  }

        /// <summary>
        /// The CQ object representing the contents of the URL.
        /// </summary>

        CQ Dom { get; }

        /// <summary>
        /// Returns true when this request has finished processing.
        /// </summary>

        bool Complete { get; }

        /// <summary>
        /// An ID that uniquely identifies this request; will be returned with the response for
        /// asynchronous requests.
        /// </summary>

        object Id { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>

        ServerConfig Options { get; set; }
    }
}
