using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CsQuery.Mvc.Tests
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcTestApp : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // [CsQuery] The following code must be added to enable to CsQuery engine

            ViewEngines.Engines.Clear();

            ViewEngine = new CsQueryViewEngine();

            ViewEngine.Options = ViewEngineOptions.EnableScriptManager | ViewEngineOptions.ResolveXmlReferences;
            ViewEngine.LibraryPath.Add("~/scripts/libs");
            ViewEngine.LibraryPath.Add("~/scripts/libs2");
            ViewEngines.Engines.Add(ViewEngine);
        }

        public static CsQueryViewEngine ViewEngine { get; protected set; }
    }
}