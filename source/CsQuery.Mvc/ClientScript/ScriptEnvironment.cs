using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;


namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// A class encapsulating the environment in which a script exists.
    /// </summary>

    public class ScriptEnvironment
    {
        private Func<string, string> _MapPath;
        private Func<string, string> _ResolveUrl;

        private static string _FileSystemRootPath;
        private static string _AppRootPath;

        /// <summary>
        /// The MapPath function to convert a path to a hard filesystem path
        /// </summary>

        public Func<string, string> MapPath
        {
            get
            {
                return _MapPath ?? HttpContext.Current.Server.MapPath;
            }
            set
            {
                _MapPath = value;
            }
        }

        /// <summary>
        /// Library path list to search
        /// </summary>

        public PathList LibraryPath { get; set; }

        /// <summary>
        /// The app relative path to the script, e.g. "~/" for the application root
        /// </summary>

        public string RelativePathRoot { get; set; }

        /// <summary>
        /// Gets or sets the function to resolve a URL.
        /// </summary>

        public Func<string, string> ResolveUrl
        {
            get
            {
                return _ResolveUrl ?? DefaultResolveUrl;
            }
            set
            {
                _ResolveUrl = value;
            }
        }

        /// <summary>
        /// Converts a relative path or an app relative path to a rooted path (from the application root). Always starts with "/"
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname to search.
        /// </param>
        ///
        /// <returns>
        /// path as a string.
        /// </returns>

        public string MapToAppRelativePath(string path)
        {
            path = ResolveParents(NormalizeSlashes(path));

            
            if (IsUrl(path) || path.StartsWith("~/"))
            {
                return path;
            }

            // it's a file-relative path, add the ScriptPath
            if (!path.StartsWith("/")) {
                return RelativePathRoot+path;
            }


            // if we're here, the path starts with a slash so it's rooted.
            // the virtual root is the same - just add a tilde

            if (FileSystemRootPath.Length == AppRootPath.Length)
            {
                return "~" + path;
            }
            else
            {
                int folders = AppRootPath.Substring(FileSystemRootPath.Length).Split('\\').Length;

                string prefix = "";
                for (int i = 0; i < folders; i++)
                {
                    prefix += "/..";
                }
                return  "~" + prefix + path;
            }

        }

        /// <summary>
        /// Remove ../ when possible
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname to search.
        /// </param>
        ///
        /// <returns>
        /// A cleaner path
        /// </returns>

        public static string ResolveParents(string path) {
            var paths = new List<string>(path.Split('/'));
            for (int i=0;i<paths.Count;i++)
            {
                if (paths[i] == ".." && i > 0)
                {
                    paths.RemoveRange(i - 1, 2);
                }
            }
            return string.Join("/", paths);
        }
        /// <summary>
        /// Converts all backslashses to forward slashes
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname to search.
        /// </param>
        ///
        /// <returns>
        /// .
        /// </returns>

        public static string NormalizeSlashes(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// Test if the path appears to be a remote URL.
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// true if url, false if not.
        /// </returns>

        public static bool IsUrl(string path)
        {
            return Patterns.UriProtocol.IsMatch(path);
        }

        /// <summary>
        /// Maps a path to a unique path: converts relative paths to the app rooted path (~/), and leaves
        /// external paths alone.
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname to search.
        /// </param>
        ///
        /// <returns>
        /// A path.
        /// </returns>

        public string UniquePath(string path)
        {
            return IsUrl(path) ?
                path :
                MapToAppRelativePath(path);
        }
        private string DefaultResolveUrl(string url)
        {
            return System.Web.Mvc.UrlHelper.GenerateContentUrl(url, new HttpContextWrapper(HttpContext.Current));
        }
        
        /// <summary>
        /// Test if the path appears to be a valid file name
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// true if valid file name, false if not.
        /// </returns>

        public static bool IsValidFileName(string path)
        {
            int lastDotIndex = path.LastIndexOf(".");
            int lastSlashIndex = path.LastIndexOf("\\");

            return (lastDotIndex == 0
                || lastDotIndex > lastSlashIndex);
        }

        /// <summary>
        /// Test if Path is a physical file
        /// </summary>
        ///
        /// <param name="path">
        /// Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// true if physical file, false if not.
        /// </returns>

        public bool IsPhysicalFile(string path)
        {
            return !IsUrl(path)
                    && IsValidFileName(path)
                    && File.Exists(path);
        }

        /// <summary>
        /// Gets the full pathname of the IIS root in the file system
        /// </summary>

        private string FileSystemRootPath
        {
            get
            {
                if (_FileSystemRootPath == null)
                {
                    _FileSystemRootPath = MapPath("/");
                }
                return _FileSystemRootPath;
            }

        }

        /// <summary>
        /// The application root path in the file system
        /// </summary>

        private string AppRootPath
        {
            get
            {
                if (_AppRootPath == null)
                {
                    _AppRootPath = MapPath("~/");
                }
                return _AppRootPath;
            }

        }
    }
}
