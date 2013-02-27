using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CsQuerySite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //BundleTable.EnableOptimizations = Config.EnableScriptOptimizations;
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/foundation/modernizr.foundation.js"
            ));


            // these scripts must be loaded on all pages.
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/scripts/jquery-{version}.js"
            ));

            bundles.Add(new StyleBundle("~/Content/foundation/css").Include(
                 "~/Content/foundation/foundation.css",
                 "~/Content/foundation/foundation.mvc.css",
                 "~/Content/foundation/app.css",
                 "~/scripts/syntaxhighlighter/styles/shThemeMidnight.css"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                      "~/Scripts/foundation/jquery.*",
                      "~/Scripts/foundation/app.js",
                      "~/Scripts/cufon.js",
                        "~/Scripts/BaarSophia_400.font.js",
                        "~/scripts/syntaxhighlighter/shCore.js",
                        "~/scripts/syntaxhighlighter/shBrushCsharp.js"
                      ));
        }
    }
}