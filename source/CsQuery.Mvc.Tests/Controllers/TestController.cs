using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests.Controllers
{
    public class TestController : CsQueryController
    {

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Action1()
        {
            
            return View();
            
        }

        public void Cq_Index()
        {
            Doc["#index-content"].Text("cq-index ran");
        }

    
        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }



        //protected override ViewResult View(IView view, object model)
        //{
        //    return base.View(view, model);
        //}

        //protected override ViewResult View(string viewName, string masterName, object model)
        //{
        //    return base.View(ViewName, MasterName, model);
        //}
    }
}
