using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests.Controllers
{
    /// <summary>
    /// An example of a common class used as the base for all controllers to perform actions on every page
    /// </summary>

    public abstract class BaseController : CsQueryController
    {

        public virtual void Cq_Start()
        {
            Doc["footer"]
                .AddClass("cq-layout-start")
                .Append("<hr /><div id='cq-footer-1'>A footer. This should have class 'cq-start' because it was created before that code runs.</div>");
        }

        public virtual void Cq_End()
        {
            Doc["footer"]
                .AddClass("cq-layout-end")
                .Append("<hr /><div id='cq-footer-2'>A late-bound footer. This should have no extra markup since this is the last method to execute.</div>");
        }

    }
}
