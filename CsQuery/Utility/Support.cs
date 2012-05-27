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
        /// If unset, will be set when GetFile is called to the guessed path
        /// </summary>
        //public static string RootPath
        //{ get; set; }
        // Read a path, or from the calling app root
        public static string GetFile(string fileName)
        {
           string filePath = GetFilePath(fileName);
           return File.ReadAllText(filePath); 

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

                //string file = "\\" + filePath.AfterLast("\\");
                string callingAssPath = GetFirstExternalAssembly().Location;
                filePath = callingAssPath.FindPathTo(cleanFileName);
                return filePath;

            }
        }
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
    }
}
