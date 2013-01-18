using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.Mvc.Tests.Controllers
{
    public class TestController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }


        public ActionResult Index4()
        {
            return View();
        }


        public ActionResult Index5()
        {
            return View();
        }

        public ActionResult Action1()
        {
            return View();            
        }
        public ActionResult InvalidScripts()
        {
            return View();
        }
        public ActionResult UnresolvedScripts()
        {
            return View();
        }
        public ActionResult DepsOutsideLibrarypath()
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

        public override void Cq_Start()
        {
            base.Cq_Start();
            Doc["div"].AddClass("cq-start");
        }
        public override void Cq_End()
        {
            Doc["div"].AddClass("cq-end");
            base.Cq_End();
        }
    }
}
