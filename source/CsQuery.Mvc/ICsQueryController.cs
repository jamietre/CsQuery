using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CsQuery.Mvc
{
    /// <summary>
    /// An interface that identfies a controller as a target for CsQuery HTML processing
    /// </summary>
    public interface ICsQueryController
    {
        /// <summary>
        /// The CsQuery representation of the HTML before rendering.
        /// </summary>
        CQ Doc { get; set; }
    }
}
