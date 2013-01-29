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

        private string _AppRootFilesystemPath;
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

        public PathList c { get; set; }

        /// <summary>
        /// The relative path to the script
        /// </summary>

        public string ScriptPath { get; set; }

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

        public string MapToAppRootPath(string path)
        {
            if (IsUrl(path) || !IsValidFileName(path))
            {
                throw new ArgumentException("'" + path + "' is not a valid file path.");
            }

            int hardPathRootLen = AppRootFilesystemPath.Length;
            string fsRelPath;

            if (path.StartsWith("~/") || path.StartsWith("~\\"))
            {
                fsRelPath = MapPath(path).Substring(hardPathRootLen);
            }
            else if (path.StartsWith("/") || path.StartsWith("\\"))
            {
                fsRelPath = path.Substring(1).Replace("/", "\\");
            }
            else
            {
                fsRelPath = MapPath(ScriptPath).Substring(hardPathRootLen) + "\\" + path.Replace("/", "\\");
            }
            return "\\" + fsRelPath;
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
        /// Maps a path to a unique path: converts relative paths to the app rooted path, and leaves
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
                MapToAppRootPath(path);
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
        /// Test if the path represents a physical file
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
        private string AppRootFilesystemPath
        {
            get
            {
                if (_AppRootFilesystemPath == null)
                {
                    _AppRootFilesystemPath = MapPath("/");
                }
                return _AppRootFilesystemPath;
            }

        }
    }
}
