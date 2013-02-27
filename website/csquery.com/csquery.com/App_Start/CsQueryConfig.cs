using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery;
using CsQuery.Mvc;

namespace CsQuerySite
{
    public static class CsQueryConfig
    {
        public static void Initialize()
        {
            ViewEngines.Engines.Clear();

            var engine = new CsQueryViewEngine();

            engine.Options = ViewEngineOptions.EnableScriptManager;

            if (SiteConfig.IsDebug)
            {
                //if (HttpContext.Current.IsDebuggingEnabled) {
                // only when debugging, disable the optimizations.
                engine.Options |= ViewEngineOptions.NoMinifyScripts
                                | ViewEngineOptions.NoCache
                                | ViewEngineOptions.NoBundle;
                //}
            }

            engine.LibraryPath.Add("~/scripts");
            engine.LibraryPath.Add("~/scripts/JsLib");

            ViewEngines.Engines.Add(engine);

        }
    }
}