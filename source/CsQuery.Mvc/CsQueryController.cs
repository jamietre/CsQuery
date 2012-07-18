using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery;

namespace CsQuery.Mvc
{
    /// <summary>
    /// An MVC controller providing access to a CQ object created from the HTML output of an MVC page.
    /// </summary>

    public class CsQueryController : Controller, ICsQueryController
    {
        /// <summary>
        /// The CsQuery representation of the HTML before rendering.
        /// </summary>
        public CQ Doc { get; set; }

    }
}
