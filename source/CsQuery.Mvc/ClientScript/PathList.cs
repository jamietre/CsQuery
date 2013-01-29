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

        /// <summary>
        /// A set of file extensions that are allowed for included files. These should not include the
        /// dot, only the extension.
        /// </summary>

        public static HashSet<string> AllowedExtensions { get; private set; }

        #region private properties

        private List<string> InnerList = new List<string>();
        
        #endregion

        #region public properties

        /// <summary>
        /// Return the ordinal index of the item
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>
        ///
        /// <returns>
        /// An integer index
        /// </returns>

        public int IndexOf(string item)
        {
            return InnerList.IndexOf(NormalizePath(item,true));
        }

        /// <summary>
        /// Inserts the item into the PathList at the specified position
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>

        public void Insert(int index, string item)
        {
            string norm = NormalizePath(item,true);
            if (!InnerList.Contains(norm))
            {
                InnerList.Insert(index, norm);
            }
        }

        /// <summary>
        /// Removes at the item found at the specified index
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the.
        /// </param>

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the entry to access.
        /// </param>
        ///
        /// <returns>
        /// The indexed item.
        /// </returns>

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

        /// <summary>
        /// Adds the item to the PathList.
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>

        public void Add(string item)
        {
            string norm = NormalizePath(item,true);
            if (!InnerList.Contains(norm))
            {
                InnerList.Add(norm);
            }
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>

        public void Clear()
        {
            InnerList.Clear();
        }

        /// <summary>
        /// Test if the item is found in the PathList 
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>
        ///
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>

        public bool Contains(string item)
        {
            return InnerList.Contains(NormalizePath(item,true));
        }

        /// <summary>
        /// Copies the contents of the PathList collection to an array
        /// </summary>
        ///
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// Zero-based index of the array.
        /// </param>

        public void CopyTo(string[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in the PathList collection
        /// </summary>

        public int Count
        {
            get { return InnerList.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is read only.
        /// </summary>

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the given item from the PathList collection
        /// </summary>
        ///
        /// <param name="item">
        /// The item.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public bool Remove(string item)
        {
            return InnerList.Remove(NormalizePath(item,true));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        ///
        /// <returns>
        /// The enumerator.
        /// </returns>

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
