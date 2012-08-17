using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Web
{
    /// <summary>
    /// Values that represent the current CsqWebRequest state. 
    /// NOT YET IMPLEMENTED
    /// </summary>

    public enum RequestState
    {
        /// <summary>
        /// The request is idle.
        /// </summary>
        Idle = 1,
        /// <summary>
        /// The request is in progress.
        /// </summary>
        Active = 2,
        /// <summary>
        /// The request failed.
        /// </summary>
        Fail = 3,
        /// <summary>
        /// The request .
        /// </summary>
        PartialSuccess = 4,

        /// <summary>
        /// The request finished successfully.
        /// </summary>
        Success = 5
    }
}
