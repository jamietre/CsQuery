using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{
    /// <summary>
    /// Data about a web request.
    /// </summary>

    public interface ICsqWebRequestMetadata
    {
        /// <summary>
        /// The time, in milliseconds, after which to abort an incompete request.
        /// </summary>

        int Timeout { get; set; }

        /// <summary>
        /// The UserAgent string to present to the remote server.
        /// </summary>

        string UserAgent { get; set; }
    }
}
