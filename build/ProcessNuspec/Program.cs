using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace ProcessNuspec
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG 
            args = new string[] { "d:\\projects\\csharp\\csquery\\build\\csquery.nuspec", "d:\\projects\\csharp\\csquery\\build\\csquery.test.nuspec", "-version", "1" };
#endif
            if (args.Length < 4 || args.Length % 2 != 0)
            {
                Console.WriteLine("Call with: ProcessNuspec input output [-param value] [-param value] ...");
                Console.WriteLine("e.g. ProcessNuspec../source/project.nuspec.template ../source/project.nuspec -version 1.3.3 -id csquery");
            }


            string input = Path.GetFullPath(args[0]);
            string output = Path.GetFullPath(args[1]);

            int argPos = 2;

            var dict = new Dictionary<string, string>();
            while (argPos < args.Length)
            {
                var argName = args[argPos++];
                var argValue = args[argPos++];
                if (!argName.StartsWith("-")) {
                    throw new Exception("Every argument must be a -name/value pair.");
                }
                dict[argName.Substring(1)]=argValue;
            }
           

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(input);

            foreach (var item in dict) {
                
                var nodes = xDoc.DocumentElement.SelectNodes("//" + item.Key );
                if (nodes.Count == 1)
                {
                    var node = nodes[0];
                    if (dict.ContainsKey(node.Name) && node.ChildNodes.Count==1)
                    {
                        node.ChildNodes[0].Value = item.Value;
                    }
                }
            }
            //string outputText = "<?xml version=\"1.0\"?>";
            XmlWriter writer = XmlWriter.Create(output);
            xDoc.WriteContentTo(writer);
            writer.Flush();
            writer.Close();

        }
    }
}
