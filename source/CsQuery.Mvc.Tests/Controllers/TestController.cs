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

        public ActionResult LogOn()
        {
            
            return View();
            
        }

        public void Cq_LogOn()
        {
            // Not doing anything useful here, but just showing how easy it is to manipulate HTML.


            var loginForm = Doc["#login-form"];

            loginForm.Parent().Append("<div>").Append("<b>The login form was duplicated from code in the LogOn controller.</b><br />");

            loginForm.Parent().Append(loginForm.Clone());

            // call a shared method that does some common configuration

            //FinishPage(doc);

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
