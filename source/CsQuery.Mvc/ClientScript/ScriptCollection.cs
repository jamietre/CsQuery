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

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// Collection of scripts used by the ScriptManager, and methods to process them
    /// </summary>

    public class ScriptCollection: ICollection<ScriptRef>
    {
        #region constructor

        public ScriptCollection(PathList libraryPath, Func<string,string> mapPathFunc)
        {
            MapPath = mapPathFunc;
            LibraryPath = libraryPath;
            Scripts = new HashSet<ScriptRef>();
            ResolvedDependencies = new Dictionary<string, ScriptRef>(); 
            DependenciesOrdered = new List<string>();
            //ScriptSources = new HashSet<ScriptRef>();
        }
        #endregion

        #region private properties


        protected PathList LibraryPath;
        protected Func<string,string> MapPath;
        ///protected HashSet<ScriptRef> ScriptSources;
        /// <summary>
        /// The unique set of dependencies (hashset used to prevent duplication).
        /// </summary>
        protected Dictionary<string, ScriptRef> ResolvedDependencies;

        /// <summary>
        /// The file paths at the top level (the members of this collection)
        /// </summary>

        protected HashSet<ScriptRef> Scripts;

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
        /// Adds inputs from all the scripts found in a CQ object
        /// </summary>
        ///
        /// <param name="scripts">
        /// The scripts.
        /// </param>

        public void AddFromCq(CQ scripts)
        {
            foreach (var script in scripts)
            {
                AddPath(script["src"]);
            }

            scripts.RemoveClass("csquery-script");
        }

        /// <summary>
        /// Adds a script reference by path.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Virtual path to the script.
        /// </param>

        public void AddPath(string virtualPath)
        {
            string normalPath = PathList.Normalize(virtualPath);
            Scripts.Add(GetScriptRef(normalPath,normalPath));
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
        /// A hash code for the current <see cref="CsQuery.Mvc.ScriptCollection" />.
        /// </returns>

        public override int GetHashCode()
        {
            return Scripts.Select(item => item.GetHashCode()).Aggregate((cur, next) =>
            {
                return cur + next;
            });
        }

        /// <summary>
        /// Determines whether the specified <see cref="CsQuery.Mvc.ScriptCollection" /> is equal to the current
        /// <see cref="CsQuery.Mvc.ScriptCollection" />. 
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
        /// <param name="ignoreErrors">
        /// true to ignore errors.
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
        /// Gets a script reference given a path
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">
        /// Thrown when the requested file is not present.
        /// </exception>
        ///
        /// <param name="scriptRelativePath">
        /// The relative path to the file to analyze.
        /// </param>
        /// <param name="ignoreErrors">
        /// true to ignore errors.
        /// </param>
        ///
        /// <returns>
        /// The script reference.
        /// </returns>

        protected ScriptRef GetScriptRef(string name,string virtualPath)
        {
            ScriptRef scriptRef;
            virtualPath = NormalizeDependencyName(virtualPath);
            name = NormalizeDependencyName(name);

            if (ResolvedDependencies.TryGetValue(name, out scriptRef))
            {
                return scriptRef;
            }

            string fileName;
            ScriptParser parser;
            try
            {
                fileName = MapPath(virtualPath);
                parser = new ScriptParser(fileName);
            }
            catch (FileNotFoundException e)
            {
                if (IgnoreErrors)
                {
                    return null;
                }
                else
                {
                    throw e;
                }
            }

            scriptRef = new ScriptRef
            {
                Name = name,
                Path = virtualPath
            };
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
                            Name = NormalizeDependencyName(depName),
                            Path=null,
                            NoCombine = optGroup.Captures.Any<string>(item=>item=="nocombine")
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
            ResolvedDependencies.Add(name,scriptRef);
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
        /// <param name="scriptRelativePath">
        /// The relative path to the file to analyze.
        /// </param>
        /// <param name="ignoreErrors">
        /// true to ignore errors.
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
                            throw new FileNotFoundException(String.Format("Unable to find dependency \"{0}\" in the LibraryPath.", dep.Path));
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
        /// Searches the LibraryPath for a match for this file
        /// </summary>
        ///
        /// <param name="name">
        /// The file name to search for.
        /// </param>
        ///
        /// <returns>
        /// The found dependency.
        /// </returns>
        /// <remarks>
        /// TODO: Optimize this to cache information about which files are in which paths so we don't have to look up every time.
        /// (Or is this worth it, since the overall bundles are cached?)
        /// </remarks>


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
                    return (libPath+"/"+ fileName).Replace("//","/");
                }
            }
            return null;
        }

        
        /// <summary>
        /// Normalize dependency name: replaces . with slash and adds .js
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>

        private string NormalizeDependencyName(string path)
        {
            if (path.EndsWith(".js")) {
                path = path.Substring(0,path.Length-3);
            }
            return path.Replace(".", "/") + ".js";
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
