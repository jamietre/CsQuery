using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CsQuery.ExtensionMethods.Xml
{
    public class CqXmlNodeList: XmlNodeList
    {
        public CqXmlNodeList(XmlDocument xmlDocument, INodeList nodeList)
        {
            NodeList = nodeList;
            XmlDocument = xmlDocument;
        }

        private INodeList NodeList;
        private XmlDocument XmlDocument;
        public override int Count
        {
            get { return NodeList.Count; }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {

            return Nodes().GetEnumerator();
        }

        private IEnumerable<XmlNode> Nodes()
        {
            foreach (var node in NodeList)
            {
                yield return node.ToXml(XmlDocument);
            }
        }

        public override XmlNode Item(int index)
        {
            return NodeList[index].ToXml();
        }
    }
}
