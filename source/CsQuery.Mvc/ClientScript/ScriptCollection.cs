using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.IO;
using System.Text.RegularExpressions;
using CsQuery.Mvc;

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// Collection of scripts used by the ScriptManager, and methods to process them
    /// </summary>

    public class ScriptCollection: ICollection<ScriptRef>
    {
        #region constructor

        /// <summary>
        /// Constructor for Script Collection.
        /// </summary>
        ///
        /// <param name="libraryPath">
        /// A PathList object containing the libraries to search for dependencies.
        /// </param>
        /// <param name="mapPathFunc">
        /// The MapPath function, e.g. HttpContext.Current.MapPath
        /// </param>

        public ScriptCollection(PathList libraryPath, Func<string,string> mapPathFunc)
        {
            MapPath = mapPathFunc;
            LibraryPath = libraryPath;
            Scripts = new HashSet<ScriptRef>();
            DependenciesOrdered = new List<string>();
            //ScriptSources = new HashSet<ScriptRef>();
        }
        #endregion

        #region private properties
        
        private static ConcurrentDictionary<string, ScriptRef> ResolvedDependencies
            = new ConcurrentDictionary<string, ScriptRef>(); 
    

        private  PathList LibraryPath;
        private Func<string, string> MapPath;

        /// <summary>
        /// The unique set of dependencies (hashset used to prevent duplication).
        /// </summary>


        /// <summary>
        /// The file paths at the top level (the members of this collection)
        /// </summary>

        private HashSet<ScriptRef> Scripts;

        /// <summary>
        /// The dependencies in the order the were resolved
        /// </summary>

        protected List<string> DependenciesOrdered;
        
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to ignore errors.
        /// </summary>
        ///
        /// <value>
        /// true if ignore errors, false if not.
        /// </value>

        public bool IgnoreErrors { get; set; }

        #region public methods

        /// <summary>
        /// Adds inputs from all the scripts found in a CQ object.
        /// </summary>
        ///
        /// <param name="scripts">
        /// The scripts.
        /// </param>
        ///
        /// <returns>
        /// The first script with dependencies
        /// </returns>

        public IDomObject AddFromCq(CQ scripts)
        {
            IDomObject first=null;
            foreach (var script in scripts)
            {
                var scriptRef = AddPath(script.UrlSource());
                if (scriptRef.Dependencies.Count > 0)
                {
                    first = script;
                }
            }

            scripts.RemoveClass("csquery-script");
            return first;
        }

        /// <summary>
        /// Adds a script reference by path.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Virtual path to the script.
        /// </param>

        public ScriptRef AddPath(string virtualPath)
        {
            string name;

            // if the path maps to something in our known libraries, 
            // create a reference using its generic name, and not the full path.
            
            if (!LibraryPath.TryGetName(virtualPath,out name)) {
                name=virtualPath;
            }

            var scriptRef = GetScriptRef(name, virtualPath);
            Scripts.Add(scriptRef);
            return scriptRef;
        }


        /// <summary>
        /// Return dependencies found in the document.
        /// </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get dependencies in this collection.
        /// </returns>

        public IEnumerable<ScriptRef> GetDependencies()
        {
            return GetDependencies(Scripts);
        }


        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        ///
        /// <returns>
        /// A hash code for the current <see cref="CsQuery.Mvc.ClientScript.ScriptCollection" />.
        /// </returns>

        public override int GetHashCode()
        {
            return Scripts.Select(item => item.GetHashCode()).Aggregate((cur, next) =>
            {
                return cur + next;
            });
        }

        /// <summary>
        /// Determines whether the specified <see cref="CsQuery.Mvc.ClientScript.ScriptCollection" /> is equal to the current
        /// <see cref="CsQuery.Mvc.ClientScript.ScriptCollection" />.
        /// </summary>
        ///
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        ///
        /// <returns>
        /// true if the objects are the same; false otherwise.
        /// </returns>

        public override bool Equals(object obj)
        {
            ScriptCollection other = obj as ScriptCollection;
            return other != null &&
                other.Scripts.Count == Count && 
                other.Scripts.OrderBy(item=>item.Path).SequenceEqual(Scripts.OrderBy(item=>item.Path));
        }

        #endregion

        #region private methods

        /// <summary>
        /// Return dependencies found in a sequence of files.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">
        /// Thrown when the requested file is not present.
        /// </exception>
        ///
        /// <param name="sources">
        /// The sources.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get dependencies in this collection.
        /// </returns>

        protected IEnumerable<ScriptRef> GetDependencies(IEnumerable<ScriptRef> sources)
        {
            HashSet<string> FoundDependencies = new HashSet<string>();
            List<ScriptRef> OrderedDependencies = new List<ScriptRef>();

            foreach (var script in sources)
            {
                try
                {
                    foreach (var dep in GetDependencies(script))
                    {
                        if (FoundDependencies.Add(dep.Name))
                        {
                            OrderedDependencies.Add(dep);
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    throw new FileNotFoundException(String.Format("Unable to resolve dependencies for \"{0}\": " + e.Message,
                        script));
                }
            }
            return OrderedDependencies;
        }

        /// <summary>
        /// Gets a script reference given a path.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">
        /// Thrown when the requested file is not present.
        /// </exception>
        ///
        /// <param name="name">
        /// The name to use for this script.
        /// </param>
        /// <param name="virtualPath">
        /// Virtual path to the script.
        /// </param>
        ///
        /// <returns>
        /// The script reference.
        /// </returns>

        protected ScriptRef GetScriptRef(string name,string virtualPath)
        {
            ScriptRef scriptRef;
            string normalizedPath = PathList.NormalizePath(virtualPath);
            string normalizedName= PathList.NormalizeName(name);

            if (ResolvedDependencies.TryGetValue(normalizedName, out scriptRef))
            {
                return scriptRef;
            }

            string fileName;
            ScriptParser parser=null;
            try
            {
                fileName = MapPath(normalizedPath);
                parser = new ScriptParser(fileName);
            }
            catch (FileNotFoundException e)
            {
                if (!IgnoreErrors)
                {
                    throw e;
                }
            }

            scriptRef = new ScriptRef
            {
                Name = normalizedName,
                Path = normalizedPath
            };

            // Parser can be null if there was an error loading the script, but IgnoreErrors=true. If this
            // is the case just skip everything, there will be no dependencies. 
            // 
            if (parser != null)
            {
                var options = new HashSet<string>();

                using (parser)
                {
                    string line;
                    while ((line = parser.ReadLine()) != null
                        && !parser.AnyCodeYet)
                    {
                        var match = Patterns.Dependency.Match(line);
                        var matchOptions = Patterns.Options.Match(line);

                        if (match.Success)
                        {
                            string depName = match.Groups["dep"].Value;
                            var optGroup = match.Groups["opt"];

                            scriptRef.Dependencies.Add(new ScriptRef
                            {
                                Name = PathList.NormalizeName(depName),
                                Path = null,
                                NoCombine = optGroup.Captures.Any<Capture>(item => item.Value == "nocombine")
                            });
                        }
                        else if (matchOptions.Success)
                        {
                            foreach (Group grp in matchOptions.Groups)
                            {
                                options.Add(grp.Value.ToLower());
                            }
                        }
                    }

                }

                scriptRef.NoCombine = options.Contains("nocombine");
            }
            
            ResolvedDependencies[normalizedName] = scriptRef;
            
            return scriptRef;

        }

        /// <summary>
        /// Return dependencies in the form of a virtual file path, e.g "~/scripts/something.js" for each
        /// "//using xxx" found in a single file.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">
        /// Thrown when the requested file is not present.
        /// </exception>
        ///
        /// <param name="scriptRef">
        /// The relative path to the file to analyze.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get dependencies in this collection.
        /// </returns>

        protected IEnumerable<ScriptRef> GetDependencies(ScriptRef scriptRef)
        {
            foreach (var dep in scriptRef.Dependencies)
            {

                ScriptRef depRef;
                if (!ResolvedDependencies.TryGetValue(dep.Name, out depRef))
                {
                    string virtualPath = FindInLibaryPath(dep.Name);
                    if (virtualPath == null)
                    {
                        if (IgnoreErrors)
                        {
                            continue;
                        }
                        else
                        {
                            throw new FileNotFoundException(String.Format("Unable to find dependency \"{0}\" in the LibraryPath.", dep.Name));
                        }
                    }

                    depRef = GetScriptRef(dep.Name, virtualPath);
                }
                // always update the path of the dependency to what we got from the cache -- it might get used.
                dep.Path = depRef.Path;

                foreach (var innerDep in GetDependencies(depRef))
                {
                    yield return innerDep;
                }

                // when returning the parent script, if NoCombine is not set on the cached version, then return the parent one instead,
                // since it may have its own nocombine setting.

                yield return depRef.NoCombine ?
                    depRef :
                    dep;
            }
            
            
        }

        /// <summary>
        /// Searches the LibraryPath for a match for this file.
        /// </summary>
        ///
        /// <remarks>
        /// TODO: Optimize this to cache information about which files are in which paths so we don't
        /// have to look up every time. (Or is this worth it, since the overall bundles are cached?)
        /// </remarks>
        ///
        /// <param name="fileName">
        /// Filename of the file.
        /// </param>
        ///
        /// <returns>
        /// The found dependency.
        /// </returns>

        private string FindInLibaryPath(string fileName)
        {
            
            foreach (var libPath in LibraryPath.ToList())
            {

                string dir = MapPath(libPath);
                if (!Directory.Exists(dir))
                {
                    LibraryPath.Remove(libPath);
                }

                string path = Path.Combine(dir,fileName);

                if (File.Exists(path))
                {
                    return (libPath+fileName).Replace("//","/");
                }
            }
            return null;
        }

        #endregion

        #region ICollection methods

        /// <summary>
        /// Adds a script path.
        /// </summary>
        ///
        /// <param name="item">
        /// The item to add.
        /// </param>

        public void Add(ScriptRef item)
        {
            Scripts.Add(item);
        }

        public void Clear()
        {
            Scripts.Clear();
        }

        public bool Contains(ScriptRef item)
        {
            return Scripts.Contains(item);
        }

        public void CopyTo(ScriptRef[] array, int arrayIndex)
        {
            Scripts.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Scripts.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ScriptRef item)
        {
            return Scripts.Remove(item);
        }

        public IEnumerator<ScriptRef> GetEnumerator()
        {
            return Scripts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
    
}
