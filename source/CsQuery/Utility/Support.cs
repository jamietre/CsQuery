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
           string filePath = GetFilePath(fileName);
           return File.ReadAllText(filePath); 
        }
        /// <summary>
        /// Open a stream for a file, trying to find it from the execution location if not rooted.
        /// </summary>
        /// <param name="fileName"></param>
        public static FileStream GetFileStream(string fileName)
        {
            string filePath = GetFilePath(fileName);
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return stream;

        }
        /// <summary>
        /// Try to find the path to a file based on the execution location of the calling assembly
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePath(string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }
            else
            {
                string filePath;
                string cleanFileName= fileName.Replace("/","\\");

                if (cleanFileName.StartsWith(".\\"))
                {
                    cleanFileName = cleanFileName.Substring(1);
                }

                string callingAssPath = AppDomain.CurrentDomain.BaseDirectory;
                
                filePath = FindPathTo(callingAssPath,cleanFileName);
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

        public static string FindPathTo(string sourcePath, string find)
        {
            List<string> rootedPath = new List<string>(sourcePath.ToLower().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> findPath = new List<string>(find.ToLower().Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

            int start = rootedPath.IndexOf(findPath[0]);
            if (start < 0)
            {
                return "";
            }
            else
            {
                int i = 0;
                while (rootedPath[start++] == findPath[i++] 
                    && i<findPath.Count
                    && start< rootedPath.Count)
                    ;

                return string.Join("\\", rootedPath.GetRange(0, start)) + "\\"
                    + string.Join("\\", findPath.GetRange(i, findPath.Count - i));

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
        /// Make sure there's one (or zero, if not rooted) leading or trailing slash, 
        /// and convert slashes to backslashes. Missing values are returned as just one backslash.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
            return output+"\\";
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

        
    }



}
