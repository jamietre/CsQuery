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
            MapPath = mapPathFunc ?? HttpContext.Current.Server.MapPath;
            ResolveUrl = resolveUrlFunc ?? DefaultResolveUrlFunc;


            LibraryPath = libraryPath ?? new PathList();
        }

        private string DefaultResolveUrlFunc(string url)
        {
            return UrlHelper.GenerateContentUrl(url, new HttpContextWrapper(HttpContext.Current));
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
        private Func<string, string> ResolveUrl;

        #endregion

        /// <summary>
        /// Gets or sets options that control the operation of the ScriptManager.
        /// </summary>

        public ViewEngineOptions Options { get; set; }

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
           

            string scriptSelector = "script[src][type='text/javascript'], link[type='text/css']";
                
            //+ (Options.HasFlag(ViewEngineOptions.ProcessAllScripts) ?
            //        "" : ".csquery-script")
            //        + "[src]";

            // Filter out non-relative paths (remote URLs)
            CQ scripts = doc[scriptSelector].Filter(item =>
            {
                return !PathList.IsRemoteUrl(item.UrlSource());
            });



            if (scripts.Length == 0)
            {
                return;
            }
            
            // move scripts to head as needed first

            var toMove = scripts.Filter("[data-location='head']");
            var head = doc["head"];

            if (toMove.Length > 0)
            {
                
                foreach (var item in toMove)
                {
                    if (item.ParentNode != head)
                    {
                        head.Append(item);
                        item.RemoveAttribute("data-location");
                    }
                }
            }

            // resolve dependencies

            ScriptCollection coll = new ScriptCollection(LibraryPath,MapPath);
            coll.NoCache = Options.HasFlag(ViewEngineOptions.NoCache);
            coll.IgnoreErrors = Options.HasFlag(ViewEngineOptions.IgnoreMissingScripts);

            // identify the insertion point for the script bundle
            var firstScriptEl = coll.AddFromCq(scripts);
            var firstScript = firstScriptEl == null ? 
                head.Children().First() : 
                firstScriptEl.Cq();

            string bundleUrl;
            List<ScriptRef> dependencies = coll.GetDependencies()
                .Where(item=>!coll.Contains(item))
                .ToList();

            // find the first script with dependencies
            

            // Now add scripts directly for dependencies marked as NoCombine.


            
            var inlineScripts = Options.HasFlag(ViewEngineOptions.NoBundle) ?
                dependencies :
                dependencies.Where(item => item.NoCombine);


            foreach (var item in inlineScripts)
            {
                firstScript.Before(GetScriptHtml(item.Path, item.ScriptHash));
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

                    string bundleAlias = "~/cqbundle" + ScriptID;
                    var bundle = GetScriptBundle(bundleAlias);

                    var activeDependencies = dependencies.Where(item => !item.NoCombine);
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

                var scriptPlaceholder = scripts.First();



                // add bundle after all noncombined scripts

                firstScript.Before(GetScriptHtml(bundleUrl));
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

            
            return CQ.CreateFragment(String.Format(template, ResolveUrl(url)));
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
