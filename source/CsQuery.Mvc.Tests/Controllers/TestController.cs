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
            Doc["#index-content"].AddClass("cq-index");
            Doc["header"].AddClass("cq-index");
        }

        public void Cq_Action1()
        {
            Doc["#action1-content"].AddClass("cq-action1");
            Doc["header"].AddClass("cq-action1");
        }


        /// <summary>
        /// Shoud run for all actions in this class
        /// </summary>

        public void Cq_Start()
        {
            Doc["div"].AddClass("cq-start");
        }
        public void Cq_End()
        {
            Doc["div"].AddClass("cq-end");
        }
    }
}
