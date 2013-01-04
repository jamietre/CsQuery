using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CsQuery;
using CsQuerySite.Helpers;

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


            XmlNode documentation = DocsByReflection.XMLFromType(typeof(CQ));

            Console.Write(documentation.ChildNodes.Count);

            base.Cq_End();
        }
       

    }
}
