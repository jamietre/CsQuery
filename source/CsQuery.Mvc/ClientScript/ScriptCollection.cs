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
        
        internal static ConcurrentDictionary<string, ScriptRef> ResolvedDependencies
            = new ConcurrentDictionary<string, ScriptRef>(); 
    

        private PathList LibraryPath;
        private Func<string, string> MapPath;

        /// <summary>
        /// The file paths at the top level (the members of this collection)
        /// </summary>

        private HashSet<ScriptRef> Scripts;

        /// <summary>
        /// The dependencies in the order the were resolved
        /// </summary>

        private List<string> DependenciesOrdered;





        #endregion

        /// <summary>
        /// When true, script dependencies will not be cached, and each script re-parsed every time the
        /// page is loaded. Normally, scripts are only analyzed for dependencies the first time they are
        /// accessed, and the cache is only reset when the application restarts. During development, this
        /// is not convenient. Enabling this option will prevent caching.
        /// </summary>
        ///
        /// <value>
        /// The options.
        /// </value>

        public bool NoCache { get; set; }

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
                if (first==null && scriptRef.Dependencies.Count > 0)
                {
                    first = script;
                }
            }

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
            
            fileName = MapPath(normalizedPath);
            
            parser = new ScriptParser(fileName);
            
            scriptRef = new ScriptRef
            {
                Name = normalizedName,
                Path = normalizedPath
            };
            
            if (parser.IsPhysicalFile)
            {
                string textOnly = "";

                using (parser)
                {
                    string line;
                    while ((line = parser.ReadLine()) != null
                        && !parser.AnyCodeYet)
                    {
                        textOnly += line + System.Environment.NewLine;

                        ScriptRef dependencyRef;
                        if (TryGetDependencyRef_UsingFormat(line, out dependencyRef)) {
                            scriptRef.Dependencies.Add(dependencyRef);
                        }
                    }


                    scriptRef.ScriptHash = parser.FileHash;
                }

                // now parse the entire text again for the XML format

                //var xml = CQ.CreateFragment(textOnly);
                //foreach (var el in xml["reference"])
                //{
                //      ScriptRef dependencyRef;
                //      if (TryGetDependencyRef_CQ(el, out dependencyRef))
                //      {
                //          scriptRef.Dependencies.Add(dependencyRef);
                //      }
                //}
            }

            if (!NoCache)
            {
                ResolvedDependencies[normalizedName] = scriptRef;
            }
            
            return scriptRef;

        }

        /// <summary>
        /// Try get dependency reference from a "reference" element
        /// </summary>
        ///
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="scriptRef">
        /// The relative path to the file to analyze.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        protected bool TryGetDependencyRef_CQ(IDomObject element, out ScriptRef scriptRef)
        {
            bool noCombine = element.HasAttribute("nocombine");
            bool ignore = element.HasAttribute("ignore");

            scriptRef = null;

            string options = element["options"];
            if (!string.IsNullOrEmpty(options))
            {
                var optList = options.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                
                
                ignore =ignore || optList.Contains("ignore", StringComparer.CurrentCultureIgnoreCase);
                noCombine = noCombine || optList.Contains("nocombine", StringComparer.CurrentCultureIgnoreCase);
            }
            if (ignore) {
                return false;
            }
            
            var depName = element["path"];

            scriptRef = new ScriptRef
            {
                Name = PathList.NormalizeName(depName),
                Path = null,
                NoCombine = noCombine
            };
            return true;
        }

        /// <summary>
        /// Analyzes a line and returns a dependency ScriptRef if it matches the "using xxx" format.
        /// </summary>
        ///
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="scriptRef">
        /// The relative path to the file to analyze.
        /// </param>
        ///
        /// <returns>
        /// The dependency reference using format.
        /// </returns>

        protected bool TryGetDependencyRef_UsingFormat(string line, out ScriptRef scriptRef)
        {
            var match = Patterns.Dependency.Match(line);
            //var matchOptions = Patterns.Options.Match(line);

            if (match.Success)
            {
                string depName = match.Groups["dep"].Value;
                var optGroup = match.Groups["opt"];

                //scriptRef.Dependencies.Add(new ScriptRef
                //{
                //    Name = PathList.NormalizeName(depName),
                //    Path = null,
                //    NoCombine = optGroup.Captures.Any<Capture>(item => item.Value == "nocombine")
                //});
                scriptRef =  new ScriptRef
                {
                    Name = PathList.NormalizeName(depName),
                    Path = null,
                    NoCombine = optGroup.Captures.Any<Capture>(item => item.Value == "nocombine")
                };
                return true;
            }  else {
                scriptRef = null;
                return false;
            }

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
                    string virtualPath = PathToDependency(scriptRef.RelativePathRoot, dep.Name);
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

                // When we resolve a dependency, whether it was created or resolved from the cache, update the
                // active one with its settings, except for NoCombine which should only be updated when it's
                // true for the resolved version. That is, we want to use the most conservative NoCombine so we
                // don't combine if the script is included with that setting, or the script itself has that
                // setting. If we just returned the ref we got from the cache, it would have incorrect
                // NoCombine settings, and we don't want to alter the cached settings. 

                dep.UpdateFrom(depRef);
                if (depRef.NoCombine)
                {
                    dep.NoCombine = true;
                }

                foreach (var innerDep in GetDependencies(depRef))
                {
                    yield return innerDep;
                }


                yield return dep;
            }
            
            
        }

        private string PathToDependency(string currentRelativePath,string fileName)
        {
            bool isRelative = IsRelativePath(fileName);
            
            string path = isRelative ?
                currentRelativePath + "/" + fileName :
                fileName;

            string directPath = MapPath(path);


            bool exists = false;
            if (!String.IsNullOrEmpty(path))
            {
                exists = File.Exists(path);
                if (!exists)
                {
                    path = null;
                }
            }

            if (!exists && IsRelativePath(fileName))
            {
                path = PathToLibraryFile(fileName);
            }
            return path;
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

        private string PathToLibraryFile(string fileName)
        {
            string pattern = null;
            if (Patterns.NonLiteralFilenames.IsMatch(fileName))
            {
                pattern = Regex.Escape(fileName).Replace("\\{version}", Patterns.FileVersionRegex);
            }

            foreach (var libPath in LibraryPath.ToList())
            {
                string matchingFile = null;
                string dir = MapPath(libPath);
                if (!Directory.Exists(dir))
                {
                    LibraryPath.Remove(libPath);
                    continue;
                }

                // check if this is a special pattern
                if (pattern!=null)
                {
                    matchingFile = GetBestMatchingFile(dir, pattern);
                }
                else
                {
                    string path = Path.Combine(dir, fileName);

                    if (File.Exists(path))
                    {
                        matchingFile = fileName;
                    }
                }

                if (matchingFile != null)
                {
                    return (libPath + matchingFile).Replace("//", "/");
                }
            }
            return null;
        }

        /// <summary>
        /// Gets best matching file given a path and a regex. 
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname to search
        /// </param>
        /// <param name="regexFilePattern">
        /// A pattern specifying the regular expression pattern to match against files.
        /// </param>
        ///
        /// <returns>
        /// The best matching file, or null if none matches.
        /// </returns>

        private string GetBestMatchingFile(string path, string regexFilePattern)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            Regex pattern = new Regex(regexFilePattern);
            List<string> matches = new List<string>();

            foreach (var fullFilePath in files)
            {
                var file = fullFilePath.AfterLast("\\");
                if (pattern.IsMatch(file))
                {
                    matches.Add(file);
                }
            }
            if (matches.Count > 0)
            {
                return matches.OrderByDescending(item => item, StringComparer.CurrentCultureIgnoreCase).First();
            }
            else
            {
                return null;
            }
        }

        private bool IsRelativePath(string file)
        {
            return !file.StartsWith("/") && !file.StartsWith("~/");
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

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>

        public void Clear()
        {
            Scripts.Clear();
        }

        /// <summary>
        /// Test whether the item is present in the collection
        /// </summary>
        ///
        /// <param name="item">
        /// The ScriptRef to test for containment.
        /// </param>
        ///
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>

        public bool Contains(ScriptRef item)
        {
            return Scripts.Contains(item);
        }

        /// <summary>
        /// Copies the collection to an array
        /// </summary>
        ///
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// Zero-based index of the array.
        /// </param>

        public void CopyTo(ScriptRef[] array, int arrayIndex)
        {
            Scripts.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in the ScriptCollection 
        /// </summary>

        public int Count
        {
            get { return Scripts.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is read only; a ScriptCollection is never read
        /// only.
        /// </summary>

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the item from the script collection
        /// </summary>
        ///
        /// <param name="item">
        /// The item to remove.
        /// </param>
        ///
        /// <returns>
        /// true if the item was removed, false if it didn't exist
        /// </returns>

        public bool Remove(ScriptRef item)
        {
            return Scripts.Remove(item);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        ///
        /// <returns>
        /// The enumerator.
        /// </returns>

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
