using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;


namespace Jtc.CsQuery.Tests
{
    /// <summary>
    /// Miscellaneous support for use in testing.
    /// </summary>
    public class Support
    {
        /// <summary>
        /// Locate & load a file in the project heirarchy 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFile(string fileName)
        {

            byte[] filedata;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string filePath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            FileStream fs = System.IO.File.Open(filePath + "\\" + fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            filedata = new byte[fs.Length];
            fs.Read(filedata, 0, (int)fs.Length);
            string result = enc.GetString(filedata).Replace("\r", " ").Replace("\n", " ");
            fs.Close();
            return (result);
        }
    }
}
