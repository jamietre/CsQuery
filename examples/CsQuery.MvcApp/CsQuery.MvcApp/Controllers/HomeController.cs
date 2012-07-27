using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery.Mvc;

namespace CsQuery.MvcApp.Controllers
{
    public class HomeController : CsQueryController
    {


        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            var x = View();
            return x;
        }

       
        public ActionResult About()
        {
             return View();
        }

        #region CsQuery methods

        public void Cq_Start()
        {
            Doc["div"].Css("border", "1px solid red;");
        }

        public void Cq_About()
        {
            var list = new KeyValuePair<string, string>[] {
                new KeyValuePair<string,string>("1","Item 1"),
                new KeyValuePair<string,string>("2","Item 2"),
                new KeyValuePair<string,string>("3","Item 3")
            };

            MakePickList(Doc["#select-list"], list);
        }

        public void Cq_End()
        {
            Doc["option"].Css("font-weight", "bold");
        }

        /// <summary>   Cq log on partial. </summary>
        ///
        /// <remarks>   James Treworgy, 7/1/2012. </remarks>

        public void Cq__LogOnPartial(CQ doc)
        {
            doc["a"].Text("Link Text Always");
        }

        public void Cq_About__LogOnPartial(CQ doc)
        {
            doc["a"].Text("Link Text About Only");

        }
        #endregion


        /// <summary>
        /// [CsQuery] A simple function to turn a sequence of Key/Value pairs into a pick list
        /// </summary>
        /// <param name="select"></param>
        /// <param name="list"></param>
        private void MakePickList(CQ select, IEnumerable<KeyValuePair<string,string>> list) 
        {
            foreach (var item in list)
            {
                var opt = select["<option />"]
                    .Attr("value",item.Key)
                    .Text(item.Value);

                select.Append(opt);
            }

        }
    }
}
