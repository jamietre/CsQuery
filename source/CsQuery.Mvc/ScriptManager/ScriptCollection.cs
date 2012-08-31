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

namespace CsQuery.Mvc
{
    /// <summary>
    /// Collection of scripts used by the ScriptManager
    /// </summary>

    class ScriptCollection 
    {
        public ScriptCollection(PathList libraryPath, Func<string,string> mapPathFunc)
        {
            MapPath = mapPathFunc;
            LibraryPath = libraryPath;
            ResolvedDependencies = new HashSet<string>();
            Dependencies = new HashSet<string>();
            DependenciesOrdered = new List<string>();
            ScriptSources = new HashSet<string>();

        }

        
        protected Regex RegexDependency = new Regex(@"^\s*using (?<dep>.+)\s*;*$");


        protected PathList LibraryPath;
        protected Func<string,string> MapPath;
        protected HashSet<string> ScriptSources;
        /// <summary>
        /// The file paths that have already been resolved
        /// </summary>
        protected HashSet<string> ResolvedDependencies;

        /// <summary>
        /// The unique set of dependencies (hashset used to prevent duplication).
        /// </summary>

        protected HashSet<string> Dependencies;

        /// <summary>
        /// The dependencies in the order the were resolved
        /// </summary>

        protected List<string> DependenciesOrdered;

        
        /// <summary>
        /// Configures the inputs from a CQ object containing script elements
        /// </summary>
        ///
        /// <param name="scripts">
        /// The scripts.
        /// </param>

        public void AddFromCq(CQ scripts)
        {
            foreach (var script in scripts)
            {
                ScriptSources.Add(PathList.Normalize(script["src"]));
            }

            scripts.RemoveClass("csquery-script");
        }

        /// <summary>
        /// Return dependencies found in the document.
        /// </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process get dependencies in this collection.
        /// </returns>

        public IEnumerable<string> GetDependencies(bool ignoreErrors)
        {
            return GetDependencies(ScriptSources, ignoreErrors);
        }

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

        protected IEnumerable<string> GetDependencies(IEnumerable<string> sources, bool ignoreErrors)
        {

            foreach (string script in sources)
            {
                try
                {
                    foreach (var dep in GetDependencies(script, ignoreErrors))
                    {
                        if (Dependencies.Add(dep))
                        {
                            DependenciesOrdered.Add(dep);
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    throw new FileNotFoundException(String.Format("Unable to resolve dependencies for \"{0}\": " + e.Message,
                        script));
                }
            }
            return DependenciesOrdered;
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

        protected IEnumerable<string> GetDependencies(string scriptRelativePath, bool ignoreErrors)
        {
            if (ResolvedDependencies.Contains(scriptRelativePath))
            {
                yield break;
            }

            string fileName;
            ScriptParser parser;
            try
            {
                fileName = MapPath(scriptRelativePath);
                parser = new ScriptParser(fileName);
            }
            catch(FileNotFoundException e)
            {
                if (ignoreErrors)
                {
                    yield break;
                }
                else
                {
                    throw e;
                }
            }

            using (parser) 
            {
                string line;
                while ((line = parser.ReadLine()) != null 
                    && !parser.AnyCodeYet)
                {
                    var match = RegexDependency.Match(line);
                    if (match.Success)
                    {
                        string depName= match.Groups["dep"].Value;

                        string virtualPath = FindInLibaryPath(NormalizeDependencyName(depName));
                        if (virtualPath == null)
                        {
                            if (ignoreErrors)
                            {
                                continue;
                            }
                            else
                            {
                                throw new FileNotFoundException(String.Format("Unable to find dependency \"{0}\" in the LibraryPath.", depName));
                            }
                        }

                        // yield inner dependencies first, since they should be loaded first.
                        
                        foreach (var innerDep in GetDependencies(virtualPath, ignoreErrors)) {
                            yield return innerDep;
                        }

                        yield return virtualPath;
                    }
                }
            }
            ResolvedDependencies.Add(scriptRelativePath);
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
                    return Path.Combine(libPath, fileName);
                }
            }
            return null;
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
            return ScriptSources.Select(item => item.GetHashCode()).Aggregate((cur, next) =>
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
            return obj is ScriptCollection &&
                ((ScriptCollection)obj).ScriptSources.SetEquals(ScriptSources);
        }

        #region private methods


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
            return path.Replace(".", "/") + ".js";
        }

        #endregion
    }
    
}
