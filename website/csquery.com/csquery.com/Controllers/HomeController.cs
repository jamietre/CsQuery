using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CsQuerySite.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}
