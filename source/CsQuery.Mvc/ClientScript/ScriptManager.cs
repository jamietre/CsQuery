using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.IO;

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// A class to manage JavaScript dependencies. Comments at the top of the file in the form
    ///   //using somelibrary
    /// 
    /// will be resolved into "~/somelibrary.js", and themselves searched for other dependencies. All
    /// dependencies found will be bundled, and the bundle URL inserted as a new script.
    /// </summary>

    public class ScriptManager
    {
        #region constructor

        /// <summary>
        /// Default constructor; creates this ScriptManager for the active HttpContext
        /// </summary>

        public ScriptManager()
        {
            MapPath = HttpContext.Current.Server.MapPath;
        }

        public ScriptManager(Func<string,string> mapPathFunc)
        {
            MapPath = mapPathFunc;
        }
        

        /// <summary>
        /// Identifier used to generate unique IDs for the generated script bundles
        /// </summary>

        static int ScriptID;

        /// <summary>
        /// Cache of bundles. If a ScriptCollection is built for a page that matches one built previously,
        /// we will reuse the bundle rather than recreating the dependency table.
        /// </summary>

        static ConcurrentDictionary<ScriptCollection, string> Bundles = new ConcurrentDictionary<ScriptCollection, string>();
        
        #endregion

        #region private properties

        private Func<string, string> MapPath;

        #endregion

        /// <summary>
        /// Gets or sets options that control the operation of the ScriptManager.
        /// </summary>

        public CsQueryViewEngineOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the paths in the library search path
        /// </summary>
        
        public PathList LibraryPath { get; set; }

        /// <summary>
        /// Resolve all script dependencies in the bound CQ document. Scripts that cotain a "data-
        /// location='head'" attribute will be moved to the head.
        /// </summary>
        ///
        /// <param name="doc">
        /// The document to resolve.
        /// </param>

        public void ResolveScriptDependencies(CQ doc)
        {
            // 1) See if we have already build a bundle based on this set of scripts.

            HashSet<string> libraries = new HashSet<string>();

            string scriptSelector = "script"
                + (Options.HasFlag(CsQueryViewEngineOptions.ProcessAllScripts) ?
                    "" : ".csquery-script")
                    + "[src]";

            CQ scripts = doc[scriptSelector];

            if (scripts.Length > 0)
            {
                // move scripts to head as needed first

                var toMove = scripts.Filter("[data-location='head']");
                if (toMove.Length > 0)
                {
                    var head = doc["head"];
                    toMove.Each((el) =>
                    {
                        head.Append(el);
                    });
                }

                // resolve dependencies

                ScriptCollection coll = new ScriptCollection(LibraryPath,MapPath);
                coll.AddFromCq(scripts);

                string bundleUrl;

                if (!Bundles.TryGetValue(coll, out bundleUrl))
                {
                    string bundleAlias = "~/cq_" + ScriptID;
                    var bundle = GetScriptBundle(bundleAlias);
                    
                    ScriptID++;

                    foreach (var item in coll.GetDependencies(Options.HasFlag(CsQueryViewEngineOptions.IgnoreMissingScripts)))
                    {
                        bundle.Include(item);
                    }

                    BundleTable.Bundles.Add(bundle);
                    if (HttpContext.Current != null)
                    {
                        bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleAlias);
                    }
                    else
                    {
                        bundleUrl = bundleAlias + "_no_http_context";
                    }

                    
                    if (!Options.HasFlag(CsQueryViewEngineOptions.NoCacheBundles))
                    {
                        Bundles[coll] = bundleUrl;
                    }
                }
                CQ scp = CQ.Create(String.Format("<script type=\"text/javascript\" class=\"csquery-generated\" src=\"{0}\"></script>", bundleUrl));

                scripts.First().Before(scp);
            }
        }

        private ScriptBundle GetScriptBundle(string bundleAlias)
        {
            var bundle = new ScriptBundle(bundleAlias);

            if (Options.HasFlag(CsQueryViewEngineOptions.NoMinifyScripts))
            {
                bundle.Transforms.Clear();
            }
            else
            {
                if (!bundle.Transforms.Any(item => item is JsMinify))
                {
                    bundle.Transforms.Add(new JsMinify());
                }
            }
            return bundle;
        }
    }

 
}
