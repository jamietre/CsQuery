using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CsQuery;
using CsQuerySite.Helpers.XmlDoc;

namespace CsQuerySite.Controllers
{
    public class CsQueryApiController : BaseController
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }

        public override void Cq_End()
        {

            var docs = new MemberGroupXmlDoc(typeof(CQ), "Create");
            

            Console.Write(docs.Count());

            base.Cq_End();
        }
       

    }
}
