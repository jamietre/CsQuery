using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using CsQuery;

namespace CsQuery.Web
{
    /// <summary>
    /// Interface for a CsQuery async web response.
    /// </summary>

    public interface ICsqWebResponse: ICsqWebRequest
    {
        /// <summary>
        /// The HTML returned by the response
        /// </summary>

        string Html { get; }

        /// <summary>
        /// Gets the time the request began
        /// </summary>

        DateTime? Started { get; }

        /// <summary>
        /// Gets the time the request finished.
        /// </summary>

        DateTime? Finished { get; }

        /// <summary>
        /// Gets a value indicating whether the request completed successfully.
        /// </summary>

        bool Success { get; }

        /// <summary>
        /// The HTTP status code for the response.
        /// </summary>

        int HttpStatus { get; }

        /// <summary>
        /// The HTTP status description for the response.
        /// </summary>

        string HttpStatusDescription { get; }

        /// <summary>
        /// Text of any error that occurred
        /// </summary>

        string Error { get; }

        /// <summary>
        /// If the request resulted in an exception, the exception.
        /// </summary>

        WebException WebException { get; }

        /// <summary>
        /// Gets the response character set encoding identified by the response header
        /// </summary>

        Encoding ResponseEncoding { get; }
    }
}
