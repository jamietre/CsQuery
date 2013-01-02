using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;
using CsQuery;

namespace CsQuerySite.Controllers
{
    public class BaseController : CsQueryController
    {

        public virtual void Cq_End()
        {
            // get all scripts that aren't found in the header or footer
            var scripts = Doc["script"].Not(Doc["#footer-scripts,head"].Find("script"));

            var head = Doc["head"];
            var foot = Doc["#footer-scripts"];

            // move them to the right place

            foreach (var script in scripts)
            {
                if (script.HasClass("head"))
                {
                    head.Append(script);
                }
                else
                {
                    foot.Append(script);
                }
            }

            //if (PageData != null)
            //{
            //    var script = CQ.Create("<script />")
            //        .AddClass("page-data")
            //        .Attr("type", "application/json");
            //    script.Append(CQ.ToJSON(PageData));
            //    Doc["body"].Append(script);
            //}
        }
    }
}
