using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CsQuery.MvcApp.Models;
using CsQuery;

namespace CsQuery.Mvc
{
    public class CsQueryController : Controller
    {
        /// <summary>
        /// This must match your layout view name; you can also set it to null and the first invocation will always match.
        /// </summary>
        public static string LayoutViewName = "ASP._Page_Views_Home_Index_cshtml";
        private Action<CQ> RenderDelegate;
        
        private int InvocationCount = 0;
        private IDictionary<string, Action<CQ>> _PartialDelegates;
        private IDictionary<string, Action<CQ>> PartialDelegates
        {

            get
            {
                if (_PartialDelegates == null) {
                    _PartialDelegates = new Dictionary<string, Action<CQ>>();
                }
                return _PartialDelegates;
            }
        }


        protected void SetCqHandler(Action<CQ> renderDelegate)
        {
            SetCqHandler(renderDelegate, null);
        }
        protected void SetCqHandler(Action<CQ> renderDelegate, string viewName)
        {
            if (String.IsNullOrEmpty(viewName))
            {
                RenderDelegate = renderDelegate;
            }
            else
            {
                if (!viewName.EndsWith("_cshtml"))
                {
                    viewName += "_cshtml";
                }
                PartialDelegates.Add(viewName, renderDelegate);
            }
        }

        /// <summary>
        /// Return the main CQ handler
        /// </summary>
        /// <returns></returns>
        public Action<CQ> GetCqHandler()
        {
            return RenderDelegate;
        }
        /// <summary>
        /// Return the CQ handler for a partial view
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public Action<CQ> GetCqHandler(string viewName)
        {

            if (_PartialDelegates == null)
            {
                return null;
            }
            else
            {
                string key = PartialDelegates.Keys.Where(item => viewName.EndsWith(item)).FirstOrDefault();
                Action<CQ> del;
                if (PartialDelegates.TryGetValue(key, out del))
                {
                    return del;
                } else {
                    throw new InvalidOperationException(String.Format("There was no delegate bound to the view name \"{0}\"",viewName));
                }
            }
            
        }
    }
}
