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

            return View();
        }

       
        public ActionResult About()
        {

            var list = new KeyValuePair<string, string>[] {
                new KeyValuePair<string,string>("1","Item 1"),
                new KeyValuePair<string,string>("2","Item 2"),
                new KeyValuePair<string,string>("3","Item 3")
            };

            // [CsQuery] An inline/anonymous function for handling the general page rendering

            SetCqHandler((doc) =>
            {
                doc["div"].Css("border", "1px solid red;");
                MakePickList(doc["#select-list"], list);
            });

            // [CsQuery] Bind a handler targeted only to the LogOnPartial view
            SetCqHandler(OnlyLogOnHandler, "LogOnPartial");

            return View();
        }

        /// <summary>
        /// [CsQuery] Handler that will be called only after rendering LogOnPartial view
        /// </summary>
        /// <param name="doc"></param>
        private void OnlyLogOnHandler(CQ doc)
        {
            doc["a"].Text("NEW LOG ON!");
        }

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
