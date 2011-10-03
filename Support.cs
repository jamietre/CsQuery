using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery
{
    public static class Support
    {
        /// <summary>
        /// If unset, will be set when GetFile is called to the guessed path
        /// </summary>
        public static string RootPath
        { get; set; }
        // Read a path, or from the calling app root
        public static string GetFile(string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                if (String.IsNullOrEmpty(RootPath))
                {
                    string file = "\\" + fileName.AfterLast("\\");
                    string callingAssPath = Assembly.GetCallingAssembly().Location;
                    RootPath = fileName.CommonStart(callingAssPath);
                    if (RootPath == String.Empty)
                    {
                        RootPath = callingAssPath.BeforeLast("\\");
                    }
                }
                return File.ReadAllText(RootPath + "\\" + fileName);

            }
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
