using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace HtmlParserSharp.Tests
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    public static class ExtensionMethods
    {
        public static string WriteString(this XmlDocument doc)
        {
            using (TextWriter writer = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    doc.WriteContentTo(xmlWriter);
                }
                return writer.ToString();
            }
        }

    }
}
