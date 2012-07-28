using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests.Controllers
{
    public class LayoutController : CsQueryController
    {


        public void Cq_Start()
        {
            Doc["footer"].Append("<hr /><div id='cq-footer-1'>A footer. This has a red border around it because it was created before the code that adds the red border.</div>");
        }

        public void Cq_End()
        {
            Doc["footer"].Append("<hr /><div id='cq-footer-2'>A late-bound footer. This is the last method to execute, so there's no border.</div>");
        }

    }
}
