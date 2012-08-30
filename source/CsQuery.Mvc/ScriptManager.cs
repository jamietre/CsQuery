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
    public class ScriptManager
    {
        static int ScriptID;
        ConcurrentDictionary<ScriptCollection, string> Bundles = new ConcurrentDictionary<ScriptCollection, string>();

        CQ Doc;
        WebViewPage Page;

        public ScriptManager(CQ doc, WebViewPage page)
        {
            Doc = doc;
            Page = page;
        }

        public void ResolveScripts(HttpContext context)
        {
            // 1) See if we have already build a bundle based on this set of scripts.

            HashSet<string> libraries = new HashSet<string>();

            ScriptCollection coll = new ScriptCollection();
            coll.AddFromCq(Doc);
            string bundleUrl;

            if (!Bundles.TryGetValue(coll, out bundleUrl))
            {

                string bundleAlias = "~/cq_" + ScriptID;
                var bundle = new ScriptBundle(bundleAlias);
                ScriptID++;

                foreach (var item in coll.GetDependencies(HttpContext.Current))
                {
                    bundle.Include(item);
                }
                BundleTable.Bundles.Add(bundle);
                bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleAlias);
                Bundles[coll] = bundleUrl;
            }
            CQ scp = CQ.Create(String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>",bundleUrl));

            Doc["script[src]:first"].Before(scp);
        }
    }

    class ScriptCollection 
    {
        protected CQ FirstDependentScript;
        protected HashSet<string> ScriptSources;
        protected HashSet<string> ResolvedDependencies;
        protected HashSet<string> Dependencies;
        protected List<string> DependenciesOrdered;

        protected string Root;

        public void AddFromCq(CQ doc)
        {
            ResolvedDependencies = new HashSet<string>();
            Dependencies = new HashSet<string>();
            DependenciesOrdered = new List<string>();
            ScriptSources = new HashSet<string>();

            var rootScript = doc["scripts[csquery][root]"];

            Root = "~/";
            if (rootScript.Length > 0)
            {
                Root = rootScript[0]["root"] + "/";
                rootScript.Remove();
            }

            var scripts = doc["script[src]"];
            foreach (var script in scripts)
            {
                ScriptSources.Add(script["src"]);
            }
        }
        public IEnumerable<string> GetDependencies(HttpContext context)
        {
            return GetDependencies(context, ScriptSources);
        }
        public IEnumerable<string> GetDependencies(HttpContext context, IEnumerable<string> sources)
        {

            foreach (string script in sources)
            {
                if (!ResolvedDependencies.Contains(script))
                {
                    foreach (var dep in GetDependency(context, script))
                    {
                        
                        if (Dependencies.Add(dep))
                        {
                            DependenciesOrdered.Add(dep);
                        }
                    }
                    ResolvedDependencies.Add(script);
                }
            }
            return ((IEnumerable<string>)DependenciesOrdered).Reverse();
        }

        /// <summary>
        /// Return dependencies found in a single file
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private IEnumerable<string> GetDependency(HttpContext context, string source)
        {
            string line;
            bool started = false;
            bool finished = false;

            string fileName = (source.StartsWith("~/") ? "" : "~/") 
                + source 
                + (source.EndsWith(".js") ? "" : ".js");
            
            using (StreamReader reader = new StreamReader(context.Server.MapPath(fileName)))
            {
                while (!finished && (line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("//using "))
                    {
                        started = true;
                        string depName= line.Substring(8).Trim();
                        yield return Root
                            + depName
                            + (depName.EndsWith(".js") ? "" : ".js");
                    }
                    else if (line != "")
                    {
                        if (started)
                        {
                            finished = true;
                        }
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            return ScriptSources.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ScriptCollection &&
                ((ScriptCollection)obj).ScriptSources == ScriptSources;
        }
    }
    
}
