using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Mvc.Tests.TestObjects
{
    public class TestCsQueryController: CsQueryController
    {
        protected override System.Web.Mvc.ViewResult View(string viewName, string masterName, object model)
        {
            return base.View(viewName, masterName, model);
        }
        protected override System.Web.Mvc.ViewResult View(System.Web.Mvc.IView view, object model)
        {
            return base.View(view, model);
        }
    }
}
