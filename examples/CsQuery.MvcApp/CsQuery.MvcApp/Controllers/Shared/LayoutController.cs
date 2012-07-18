using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.MvcApp.Controllers
{
    public class LayoutController : CsQueryController
    {


        public void Cq_Start()
        {
            Doc["body"].Append("<hr /><div>A footer. This has a red border around it because it was created before the code that adds the red border.</div>");
        }

        public void Cq_End()
        {
            Doc["body"].Append("<hr /><div>A late-bound footer. This is the last method to execute, so there's no border.</div>");
        }

    }
}
