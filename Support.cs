using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jtc.CsQuery
{
    public static class Support
    {
        public static string GetFile(string fileName)
        {

            byte[] filedata;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string filePath = string.Empty;
            // if appears to be relative, treat it as such
            if (!(filePath.IndexOf(":")>0 || filePath.StartsWith("\\\\"))) 
            {
                filePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "\\";
            } 
            

            FileStream fs = System.IO.File.Open(filePath  + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            filedata = new byte[fs.Length];
            fs.Read(filedata, 0, (int)fs.Length);
            string result = enc.GetString(filedata).Replace("\r", " ").Replace("\n", " ");
            fs.Close();
            return (result);
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
