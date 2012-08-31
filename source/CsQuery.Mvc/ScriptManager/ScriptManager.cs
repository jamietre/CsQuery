using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.IO;

namespace CsQuery.Mvc
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
        /// <summary>
        /// Identifier used to generate unique IDs for the generated script bundles
        /// </summary>

        static int ScriptID;

        ConcurrentDictionary<ScriptCollection, string> Bundles = new ConcurrentDictionary<ScriptCollection, string>();

        CQ Doc;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="doc">
        /// The CQ document to be processed
        /// </param>

        public ScriptManager(CQ doc)
        {
            Doc = doc;
        }

        /// <summary>
        /// Gets or sets options that control the operation of the ScriptManager.
        /// </summary>

        public CsQueryViewEngineOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the paths in the library search path
        /// </summary>
        
        public PathList LibraryPath { get; set; }

        /// <summary>
        /// Resolve all script dependencies in the bound CQ document
        /// </summary>
        ///
        /// <param name="context">
        /// The context.
        /// </param>

        public void ResolveScriptDependencies(HttpContext context)
        {
            // 1) See if we have already build a bundle based on this set of scripts.

            HashSet<string> libraries = new HashSet<string>();

            ScriptCollection coll = new ScriptCollection(context, LibraryPath);

            //string rootSelector = "script[data-root]";
            //coll.AddRootFromCq(Doc[rootSelector]);

            string scriptSelector = "script"
                + (Options.HasFlag(CsQueryViewEngineOptions.ProcessAllScripts) ?
                    "" : ".csquery-script")
                    + "[src]";

            CQ scripts = Doc[scriptSelector];
            if (scripts.Length > 0)
            {
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
                    bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleAlias);
                    Bundles[coll] = bundleUrl;
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
