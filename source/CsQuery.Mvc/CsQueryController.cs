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
        private CQ _Doc;

        /// <summary>
        /// The CsQuery representation of the HTML before rendering.
        /// </summary>

        public CQ Doc
        {
            get
            {
                return _Doc != null ?
                    _Doc :
                    Deferred != null ?
                    Deferred.Dom :
                    null;
            }
            set
            {
                _Doc = value;
            }
        }

        /// <summary>
        /// Gets or sets the deferred CQ object; this optimizes the parsing of pages so nothing happens
        /// if the object is never accessed.
        /// </summary>

        internal DeferredCq Deferred { get; set; }
    }
}
