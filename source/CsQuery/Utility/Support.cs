using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.StringScanner;

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
        /// Given a partial path to a folder or file, try to find the full rooted path. The topmost part
        /// of the partial path must be part of the current application path; e.g. there must be an
        /// overlapping part on which to match.
        /// </summary>
        ///
        /// <param name="partialPath">
        /// The partial path to find.
        /// </param>
        /// <param name="filePath">
        /// [out] Full pathname of the file.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public static bool TryGetFilePath(string partialPath, out string filePath)
        {
            try {
                filePath = GetFilePath(partialPath);
                return true;
            }
            catch(Exception) {
                filePath = "";
                return false;
            }
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

        public static string GetFilePath(string partialPath)
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

                filePath = GetFilePath(cleanFileName, callingAssPath);
                return filePath;

            }
        }

        /// <summary>
        /// Given a rooted path to look within, and a partial path to a file, the full path to the file.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <param name="partialPath">
        /// The partial path to find.
        /// </param>
        /// <param name="basePath">
        /// The rooted path to match within
        /// </param>
        ///
        /// <returns>
        /// The full rooted path the the file.
        /// </returns>


        public static string GetFilePath(string partialPath, string basePath)
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
            // add trailing slashes if it's not a file

            if (output.LastIndexOf(".") < output.LastIndexOf("\\"))
            {
                return output + "\\";
            }
            else
            {
                return output;
            }
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
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
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
        /// <param name="patterns">
        /// One or more file matching patterns to match.
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

        /// <summary>
        /// Copies files matching a pattern. Existing files will be overwritten.
        /// </summary>
        ///
        /// <param name="source">
        /// Source directory for the files
        /// </param>
        /// <param name="destination">
        /// Destination directory.
        /// </param>
        /// <param name="patterns">
        /// One or more file matching patterns to match.
        /// </param>

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
        /// <exception cref="ArgumentException">
        /// Thrown when the directory is missing
        /// </exception>
        ///
        /// <param name="directory">
        /// Directory where files are located.
        /// </param>
        /// <param name="patterns">
        /// One or more file matching patterns to delete
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

     

        /// <summary>
        ///Convert a string value to a double, or zero if non-numeric
        /// </summary>
        ///
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>

        public static double DoubleOrZero(string value)
        {
            double dblVal;
            if (double.TryParse(value, out dblVal))
            {
                return dblVal;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Convert a string value to an integer, or zero if non-numeric
        /// </summary>
        ///
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// An integer
        /// </returns>

        public static int IntOrZero(string value)
        {
            int intVal;
            if (int.TryParse(value, out intVal))
            {
                return intVal;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Return an int or double from any number.
        /// </summary>
        ///
        /// <param name="value">
        /// The number to convert
        /// </param>
        ///
        /// <returns>
        /// The converted number
        /// </returns>

        public static IConvertible NumberToDoubleOrInt(IConvertible value)
        {
            double val = (double)System.Convert.ChangeType(value, typeof(double));
            if (val == Math.Floor(val))
            {
                return (int)val;
            }
            else
            {
                return val;
            }
        }


        /// <summary>
        /// Given a string, convert each uppercase letter to a "-" followed by the lower case letter.
        /// E.g. "fontSize" becomes "font-size".
        /// </summary>
        ///
        /// <param name="name">
        /// The string to uncamelcase
        /// </param>
        ///
        /// <returns>
        /// A string
        /// </returns>

        public static string FromCamelCase(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return "";
            }

            int pos = 0;
            StringBuilder output = new StringBuilder();

            while (pos < name.Length)
            {
                char cur = name[pos];
                if (cur >= 'A' && cur <= 'Z')
                {
                    if (pos > 0 && name[pos - 1] != '-')
                    {
                        output.Append("-");
                    }
                    output.Append(cur.ToLower());
                }
                else
                {
                    output.Append(cur);
                }
                pos++;
            }
            return output.ToString();

        }

        /// <summary>
        /// Converts a name from dashed-separators to camelCase.
        /// </summary>
        ///
        /// <param name="name">
        /// The string to camelCase.
        /// </param>
        /// <param name="capFirst">
        /// (optional) when true, the first letter of the resuling word is captalized.
        /// </param>
        ///
        /// <returns>
        /// a dased-separated string.
        /// </returns>

        public static string ToCamelCase(string name, bool capFirst = false)
        {
            if (String.IsNullOrEmpty(name))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            int pos = 0;
            bool first = capFirst;
            while (pos < name.Length)
            {
                char c = name[pos];

                if (c == '-' &&
                        pos > 0 &&
                        pos < name.Length - 1 &&
                        CharacterData.IsType(name[pos - 1], CharacterType.Alpha) &&
                        CharacterData.IsType(name[pos + 1], CharacterType.Alpha))
                {
                    c = name[++pos];
                    sb.Append(first ? c : char.ToUpper(c));
                    first = false;

                }
                else
                {
                    sb.Append(first ? char.ToUpper(c) : c);
                    if (CharacterData.IsType(c, CharacterType.Alpha))
                    {
                        first = false;
                    }
                }
                pos++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a value to an enum, assuming the enum is camelcased.
        /// </summary>
        ///
        /// <typeparam name="TEnum">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// value as a T.
        /// </returns>

        public static TEnum AttributeToEnum<TEnum>(string value) where TEnum : struct
        {
            TEnum enumValue;
            if (Enum.TryParse<TEnum>(ToCamelCase(value, true), out enumValue))
            {
                return enumValue;
            }
            else
            {
                return (TEnum)(IConvertible)0;
            }
        }

        /// <summary>
        /// Convert an enum to a lowercased attribute value
        /// </summary>
        ///
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// The attribute value of a string
        /// </returns>

        public static string EnumToAttribute(Enum value)
        {
            return value.ToString().ToLower();
        }
    }

}
