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
        /// Default constructor; creates this ScriptManager for the active HttpContext.MapPath
        /// </summary>

        public ScriptManager()
        {
            Initialize(null, null,null);
        }
        static ScriptManager()
        {

            //BundleTable.EnableOptimizations = false;
        }

        /// <summary>
        /// Creates a ScriptManager for the ScriptEnvironment data passed
        /// </summary>
        ///
        /// <param name="env">
        /// The environment.
        /// </param>

        public ScriptManager(ScriptEnvironment env)
        {
            ScriptEnvironment = env;
        }

        /// <summary>
        /// Default constructor; creates this ScriptManager for the specified library path &amp; MapPath
        /// function.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <param name="libraryPath">
        /// The paths in the library search path.
        /// </param>
        /// <param name="mapPathFunc">
        /// The map path function.
        /// </param>
        /// <param name="resolveUrlFunc">
        /// A function to resolve relative URLs
        /// </param>

        public ScriptManager(PathList libraryPath, Func<string,string> mapPathFunc, Func<string,string> resolveUrlFunc)
        {
            if (mapPathFunc == null)
            {
                throw new ArgumentException("The MapPath function cannot be null.");
            }
            if (libraryPath == null)
            {
                throw new ArgumentException("The LibraryPath cannot be null.");
            }
            Initialize(libraryPath, mapPathFunc, resolveUrlFunc);   
        }

        /// <summary>
        /// Creates this ScriptManager with an empty LibraryPath.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <param name="mapPathFunc">
        /// The map path function.
        /// </param>
        /// <param name="resolveUrlFunc">
        /// A function to resolve relative URLs.
        /// </param>

        public ScriptManager(Func<string, string> mapPathFunc, Func<string, string> resolveUrlFunc)
        {
            if (mapPathFunc == null)
            {
                throw new ArgumentException("The mapPath function cannot be null.");
            }
            if (resolveUrlFunc == null)
            {
                throw new ArgumentException("The resolveUrlFunc function cannot be null.");
            }

                 
            Initialize(null, mapPathFunc, resolveUrlFunc);
        }

        private void Initialize(PathList libraryPath, Func<string, string> mapPathFunc, Func<string, string> resolveUrlFunc)
        {
            ScriptEnvironment = new ScriptEnvironment
            {
                MapPath = mapPathFunc,
                LibraryPath = libraryPath,
                ResolveUrl = resolveUrlFunc
            };

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

        private ScriptEnvironment ScriptEnvironment;


        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets options that control the operation of the ScriptManager.
        /// </summary>

        public ViewEngineOptions Options { get; set; }

        
        #endregion

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
           

            string scriptSelector = "script[src][type='text/javascript'], script[src]:not([type]), link[type='text/css']";

            CQ scripts = doc[scriptSelector];

            if (scripts.Length == 0)
            {
                return;
            }
            
            // move scripts first
            // TODO: Optimize using a query caching mechanism so
                
            foreach (var item in scripts.Filter("[data-moveto]"))
            {
                var target = doc.Select(item["data-moveto"]);
                if (target.Length>0) {
                    target.First().Append(item);
                    item.RemoveAttribute("data-moveto");
                }
            }
            

            // resolve dependencies

            ScriptCollection coll = new ScriptCollection(ScriptEnvironment);
            coll.Options= Options;

            // identify the insertion point for the script bundle. AddFromCq returns the first script with dependencies,
            // so scripts should be added right before that one. Otherwise they should be added
            // at the end of head.
            
            var firstScriptEl = coll.AddFromCq(scripts);
            CQ firstScript=null;

            if (firstScriptEl != null)
            {
                firstScript = firstScriptEl.Cq();
            }
           
            string bundleUrl;
            List<ScriptRef> dependencies = coll.GetDependencies()
                .Where(item=>!coll.Contains(item))
                .ToList();

            // Now add scripts directly for dependencies marked as NoCombine.

            
            var inlineScripts = Options.HasFlag(ViewEngineOptions.NoBundle) ?
                dependencies :
                dependencies.Where(item => item.NoCombine);


            foreach (var item in inlineScripts)
            {
                var script = GetScriptHtml(item.Path, item.ScriptHash);
                if (firstScript != null)
                {
                    firstScript.Before(script);
                }
                else
                {
                    firstScript = script;
                    doc["body"].Append(script);
                }
           }

            // Before creating the bundle, remove any duplicates of the same script on the page


            if (!Options.HasFlag(ViewEngineOptions.NoBundle))
            {

                bool hasBundle = Bundles.TryGetValue(coll, out bundleUrl);

                if (hasBundle)
                {
                    // when nocache is set, we will regenerate the bundle, but not change the script ID. The v=
                    // flag will be changed by BundleTable. 
                    
                    if (Options.HasFlag(ViewEngineOptions.NoCache))
                    {
                        string removeUrl = "~" + bundleUrl.Before("?");
                        BundleTable.Bundles.Remove(BundleTable.Bundles.GetBundleFor(removeUrl));
                        hasBundle = false;
                        ScriptID++;
                        
                        // this code attempts to un-cache the bundle, it doesn't work.
                        // leaving it here until some permanent solution is found as a reminder
                        // 
                        // http://stackoverflow.com/questions/12317391/how-to-force-bundlecollection-to-flush-cached-script-bundles-in-mvc4
                        // 
                        //var bundleList = BundleTable.Bundles.ToList();
                        //BundleTable.Bundles.Clear();
                        //BundleTable.Bundles.ResetAll();
                        //BundleTable.EnableOptimizations = false;

                        //foreach (var oldBundle in bundleList)
                        //{
                        //    BundleTable.Bundles.Add(oldBundle);
                        //}

                    }
                }
                else
                {
                    ScriptID++;
                }

                if (!hasBundle)
                {
                    var activeDependencies = dependencies.Where(item => !item.NoCombine).ToList();
                    if (activeDependencies.Count > 0)
                    {
                        string bundleAlias = "~/cqbundle" + ScriptID;
                        var bundle = GetScriptBundle(bundleAlias);



                        foreach (var item in activeDependencies)
                        {
                            bundle.Include(item.Path);
                        }


                        BundleTable.Bundles.Add(bundle);
                        if (HttpContext.Current != null)
                        {
                            bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleAlias, true);
                        }
                        else
                        {
                            bundleUrl = bundleAlias + "_no_http_context";
                        }
                        Bundles[coll] = bundleUrl;
                    }
                }

                var scriptPlaceholder = scripts.First();



                // add bundle after all noncombined scripts
                if (!String.IsNullOrEmpty(bundleUrl))
                {
                    firstScript.Before(GetScriptHtml(bundleUrl));
                }
            }
               
            
        }

        private CQ GetScriptHtml(string url, string hash=null)
        {
            string template;
            
            if (url.EndsWith(".css"))
            {
                template = "<link rel=\"stylesheet\" type=\"text/css\" class=\"csquery-generated\" href=\"{0}\" />";
            }
            else
            {
                template = "<script type=\"text/javascript\" class=\"csquery-generated\" src=\"{0}\"></script>";
            }

            if (!String.IsNullOrEmpty(hash))
            {
                url = url + "?v=" + hash;
            }

            
            return CQ.CreateFragment(String.Format(template, ScriptEnvironment.ResolveUrl(url)));
        }
        private ScriptBundle GetScriptBundle(string bundleAlias)
        {
            var bundle = new ScriptBundle(bundleAlias);

            if (Options.HasFlag(ViewEngineOptions.NoMinifyScripts))
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
