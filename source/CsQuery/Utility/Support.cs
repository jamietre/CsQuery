using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Utility
{
    /// <summary>
    /// Some static methods that didn't fit in anywhere else. 
    /// </summary>
    public static class Support
    {
        /// <summary>
        /// Read all text of a file, trying to find it from the execution location if not rooted.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFile(string fileName)
        {
            string filePath = FindPathTo(fileName);
            return File.ReadAllText(filePath);
        }
        /// <summary>
        /// Open a stream for a file, trying to find it from the execution location if not rooted.
        /// </summary>
        /// <param name="fileName"></param>
        public static FileStream GetFileStream(string fileName)
        {
            string filePath = FindPathTo(fileName);
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return stream;

        }

        /// <summary>
        /// Given a partial path to a folder or file, try to find the full rooted path. The topmost part
        /// of the partial path must be part of the current application path; e.g. there must be an
        /// overlapping part on which to match.
        /// </summary>
        ///
        /// <param name="partialPath">
        /// The partial path to find
        /// </param>
        ///
        /// <returns>
        /// The file path.
        /// </returns>

        public static string FindPathTo(string partialPath)
        {
            if (Path.IsPathRooted(partialPath))
            {
                return partialPath;
            }
            else
            {
                string filePath;
                string cleanFileName = partialPath.Replace("/", "\\");

                if (cleanFileName.StartsWith(".\\"))
                {
                    cleanFileName = cleanFileName.Substring(1);
                }

                string callingAssPath = AppDomain.CurrentDomain.BaseDirectory;

                filePath = FindPathTo(cleanFileName, callingAssPath);
                return filePath;

            }
        }

        /// <summary>
        /// Given a rooted path to look within, and a partial path to a file, the full path to the file.
        /// </summary>
        ///
        /// <param name="sourcePath">
        /// The rooted path to match within
        /// </param>
        /// <param name="find">
        /// The path/filename to find
        /// </param>
        ///
        /// <returns>
        /// The full rooted path the the file
        /// </returns>

        public static string FindPathTo(string partialPath, string basePath)
        {
            List<string> rootedPath = new List<string>(basePath.ToLower().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> findPath = new List<string>(partialPath.ToLower().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

            int start = rootedPath.IndexOf(findPath[0]);
            if (start < 0)
            {
                throw new ArgumentException(String.Format("Unable to find path to \"{0}\" in base path \"{1}\" no matching parts.", 
                    partialPath, 
                    basePath));
            }
            else
            {
                int i = 0;
                while (rootedPath[start++] == findPath[i++]
                    && i < findPath.Count
                    && start < rootedPath.Count)
                    ;

                string output = string.Join("\\", rootedPath.GetRange(0, start - 1)) + "\\"
                    + string.Join("\\", findPath.GetRange(i - 1, findPath.Count - i + 1));

                return CleanFilePath(output);
            }
        }

        /// <summary>
        /// Gets the first assembly that is not the assembly that this method belongs to
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested operation is invalid.
        /// </exception>
        ///
        /// <returns>
        /// The first external assembly.
        /// </returns>

        public static Assembly GetFirstExternalAssembly()
        {
            Assembly me = Assembly.GetExecutingAssembly();

            StackTrace st = new StackTrace(false);
            foreach (StackFrame frame in st.GetFrames())
            {
                MethodBase m = frame.GetMethod();
                if (m != null && m.DeclaringType != null &&
                    m.DeclaringType.Assembly != me)
                {
                    return m.DeclaringType.Assembly;
                }
            }
            throw new InvalidOperationException("Never found an external assembly.");
        }
        /// <summary>
        ///  Gets a resource from the calling assembly
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Stream GetResourceStream(string resourceName)
        {
            return GetResourceStream(resourceName, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets a resource name using the assembly and resource name
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Stream GetResourceStream(string resourceName, Assembly assembly)
        {

            Stream fileStream = assembly.GetManifestResourceStream(resourceName);
            return (fileStream);
        }
        public static Stream GetResourceStream(string resourceName, string assembly)
        {
            Assembly loadedAssembly = Assembly.Load(assembly);
            return GetResourceStream(resourceName, loadedAssembly);
        }

        public static string StreamToString(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(data, 0, (int)stream.Length);
            return (Encoding.ASCII.GetString(data));
        }

        /// <summary>
        /// Convert slashes to backslashes; make sure there's one (or zero, if not rooted) leading or
        /// trailing backslash; resolve parent and current folder references. Missing values are
        /// returned as just one backslash.
        /// </summary>
        ///
        /// <param name="path">
        /// The path to clean
        /// </param>
        ///
        /// <returns>
        /// A cleaned/resolved path
        /// </returns>

        public static string CleanFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            string output = path.Replace("/", "\\");
            while (output.IndexOf("\\\\") > 0)
            {
                output = output.Replace("\\\\", "\\");
            }
            //if (Path.IsPathRooted(output))
            //{
            //    return output;
            //}


            // parse parents

            int pos = output.IndexOf("\\..\\");
            while (pos > 0)
            {
                int prevPos = output.Substring(0, pos).LastIndexOf("\\");
                if (prevPos > 0)
                {
                    output = output.Substring(0, prevPos) + output.Substring(pos + 3);
                    pos = output.IndexOf("\\..\\");
                }
                else
                {
                    pos = -1;
                }

            }
            while (output.LastIndexOf("\\") == output.Length - 1)
            {
                output = output.Substring(0, output.Length - 1);
            }
            return output + "\\";
        }

        /// <summary>
        /// Get a fully qualified namespaced path to a member
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static string MethodPath(MemberInfo mi)
        {
            return TypePath(mi.ReflectedType) + "." + mi.Name;
        }

        /// <summary>
        /// Get a fully qualified namespaced path to a member.
        /// </summary>
        ///
        /// <param name="type">
        /// The type to inspect.
        /// </param>
        /// <param name="memberName">
        /// Name of the member.
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>


        public static string MethodPath(Type type, string memberName)
        {
            return TypePath(type) + "." + memberName;
        }

        /// <summary>
        /// Get a fully qualified namespaced path to a type, e.g. "CsQuery.Utility.Support.TypePath"
        /// </summary>
        ///
        /// <param name="type">
        /// The type to inspect
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>

        public static string TypePath(Type type)
        {
            return type.Namespace + "." + type.Name;
        }

        /// <summary>
        /// Conver a stream to a character array.
        /// </summary>
        ///
        /// <param name="stream">
        /// The stream.
        /// </param>
        ///
        /// <returns>
        /// A character array.
        /// </returns>

        public static char[] StreamToCharArray(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            long len = stream.Length;

            if (len > 0 && len < int.MaxValue)
            {
                char[] arr = new char[stream.Length];
                reader.Read(arr, 0, Convert.ToInt32(len));
                return arr;
            }
            else
            {
                return reader.ReadToEnd().ToCharArray();
            }
        }

        /// <summary>
        /// Convert a string to a char array, if not null.
        /// </summary>
        ///
        /// <param name="html">
        /// The string.
        /// </param>
        ///
        /// <returns>
        /// The converted string, or null
        /// </returns>

        public static char[] StringToCharArray(string html)
        {
            return String.IsNullOrEmpty(html) ?
                null :
                html.ToCharArray();
        }

        /// <summary>
        /// Copies files matching a pattern.
        /// </summary>
        ///
        /// <param name="source">
        /// Source for the.
        /// </param>
        /// <param name="destination">
        /// Destination for the.
        /// </param>
        /// <param name="overwrite">
        /// true to overwrite, false to preserve.
        /// </param>

        public static void CopyFiles(DirectoryInfo source,
                       DirectoryInfo destination,
                       bool overwrite,
                        params string[] patterns)
        {
            if (source == null)
            {
                throw new ArgumentException("No source directory specified.");
            }
            if (destination == null)
            {
                throw new ArgumentException("No destination directory specified.");
            }
            foreach (var pattern in patterns)
            {
                FileInfo[] files = source.GetFiles(pattern);

                foreach (FileInfo file in files)
                {
                    file.CopyTo(destination.FullName + "\\" + file.Name, overwrite);
                }
            }
        }

        public static void CopyFiles(DirectoryInfo source,
                    DirectoryInfo destination,
                     params string[] patterns)
        {
            CopyFiles(source, destination, true, patterns);
        }

        /// <summary>
        /// Deletes the files in a directory matching one or more patterns (nonrecursive)
        /// </summary>
        ///
        /// <param name="source">
        /// Directory where files are located
        /// </param>
        /// <param name="patterns">
        /// A variable-length parameters list containing patterns.
        /// </param>

        public static void DeleteFiles(DirectoryInfo directory, params string[] patterns)
        {
            if (directory == null)
            {
                throw new ArgumentException("No directory specified.");
            }
            foreach (var pattern in patterns)
            {
                FileInfo[] files = directory.GetFiles(pattern);

                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
        }
    }

}
