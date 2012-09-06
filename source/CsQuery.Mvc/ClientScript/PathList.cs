using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// A list of virtual paths
    /// </summary>
    
    [Serializable]
    public class PathList: IList<string>
    {
        static PathList()
        {
            AllowedExtensions = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            AllowedExtensions.Add("css");
            AllowedExtensions.Add("js");
            AllowedExtensions.Add("less");

        }

        public static HashSet<string> AllowedExtensions { get; private set; }

        #region private properties

        private List<string> InnerList = new List<string>();
        
        #endregion

        #region public properties

        public int IndexOf(string item)
        {
            return InnerList.IndexOf(NormalizePath(item,true));
        }

        public void Insert(int index, string item)
        {
            string norm = NormalizePath(item,true);
            if (!InnerList.Contains(norm))
            {
                InnerList.Insert(index, norm);
            }
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }

        public void Add(string item)
        {
            string norm = NormalizePath(item,true);
            if (!InnerList.Contains(norm))
            {
                InnerList.Add(norm);
            }
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(string item)
        {
            return InnerList.Contains(NormalizePath(item,true));
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return InnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return InnerList.Remove(NormalizePath(item,true));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Test whether a path maps to something in this PathList; if so, return the name-normalized
        /// path.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Full pathname of the virtual file.
        /// </param>
        /// <param name="name">
        /// [out] The name.
        /// </param>
        ///
        /// <returns>
        /// The name for path.
        /// </returns>

        public bool TryGetName(string virtualPath, out string name)
        {
            string matchPath = NormalizePath(virtualPath);

            foreach (string path in InnerList)
            {
                if (matchPath.StartsWith(path))
                {
                    name = NormalizeName(matchPath.Substring(path.Length));
                    return true;
                }
            }
            name =null;
            
            return false;
        }

        /// <summary>
        /// Given a virtual path, returns the normal name (not including the library root part of the path) if the path is
        /// within the library. If it is not, it returns the original path in name-normalized form.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Full pathname of the virtual file.
        /// </param>
        ///
        /// <returns>
        /// The name.
        /// </returns>

        public string GetName(string virtualPath)
        {
            string name;
            if (TryGetName(virtualPath, out name))
            {
                return name;
            } else {
                return NormalizeName(virtualPath);
            }
        }
        #endregion

        #region static methods

        /// <summary>
        /// Test if the path is a URL, or a local path
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// true if remote url, false if not.
        /// </returns>

        public static bool IsRemoteUrl(string path) {
            // this should probably instead check for *:// but OK for now
             return path.StartsWith("http://") || path.StartsWith("https://") || path.StartsWith("ftp://");
        }
        
        /// <summary>
        /// Normalizes a path name by adding virtual notation and removing trailing slashes.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Full pathname of the virtual file.
        /// </param>
        /// <param name="trailingSlash">
        /// (optional) Adds (or preserves) a trailing slash, e.g. treat this as a location and not a file.
        /// </param>
        ///
        /// <returns>
        /// A normalized path.
        /// </returns>

        public static string NormalizePath(string virtualPath, bool trailingSlash=false)
        {
            string prefix = "";
            int length;
            if (virtualPath[0]=='/') {
                prefix="~";
            } else if (!virtualPath.StartsWith("~/")) {
                prefix = "~/";
            }

            int pos = virtualPath.IndexOf("?");
                length = (pos >= 0 ?
                    pos :
                    virtualPath.Length);

            if (virtualPath.EndsWith("/"))
            {
                length-=1;
            }

            return prefix +
                virtualPath.Substring(0, length).ToLower() +
                    (trailingSlash ?
                        "/" :
                        "");
        }

        /// <summary>
        /// Normalize a path to a name, e.g. remove relative bases, and ensure there is an extension.
        /// </summary>
        ///
        /// <param name="virtualPath">
        /// Full pathname of the virtual file.
        /// </param>
        ///
        /// <returns>
        /// A normalized name like "folder/script.js"
        /// </returns>

        public static string NormalizeName(string virtualPath)
        {
            int offset = 0, length=0;
            
            if (virtualPath.StartsWith("~/"))
            {
                offset += 2;
            }
            if (virtualPath.Substring(offset,1)=="/")
            {
                offset+=1;
            }

            // strip off querystrings
            // 
            int pos = virtualPath.IndexOf("?");
            length = (pos >= 0 ?
                pos :
                virtualPath.Length) - offset;

            

            string output = virtualPath.Substring(offset, length);
            string suffix = output.AfterLast(".").ToLower();

            return output +
                (!AllowedExtensions.Contains(suffix) ? ".js" : "");
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
