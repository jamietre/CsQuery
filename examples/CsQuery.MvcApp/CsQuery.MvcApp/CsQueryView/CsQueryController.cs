using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CsQuery.MvcApp.Models;
using CsQuery;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Implementation of CsQuery controller
    /// </summary>
    public class CsQueryController : Controller, ICsQueryController
    {
        /// <summary>
        /// The CsQuery representation of the HTML before rendering.
        /// </summary>
        public CQ Doc { get; set; }

    }
}
