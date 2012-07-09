using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CsQuery.Implementation;

namespace CsQuery.ExtensionMethods.Xml
{
    public class CqXmlDocument: XmlDocument
    {
        public CqXmlDocument(IDomDocument document)
        {
            //var root = CreateElement("", "ROOT", "");
            CqXmlNode docNode=null;
            

            foreach (var child in document.ChildNodes)
            {
                var node = new CqXmlNode(this, child);
                if (node.NodeType == XmlNodeType.DocumentType)
                {
                    this.AppendChild(node);
                }
                else
                {
                    if (docNode == null)
                    {
                        docNode = new CqXmlNode(this, document);
                        this.AppendChild(docNode);
                    }
                    docNode.AppendChild(node);
                }
            }
        }

        //private XmlDocument XmlDocument;
        //private CqXmlNode InnerNode;

        //public override XmlNodeList ChildNodes
        //{
        //    get
        //    {
        //        return InnerNode.ChildNodes;
        //    }
        //}
        //public override XmlAttributeCollection Attributes
        //{
        //    get
        //    {
        //        return InnerNode.Attributes;
        //    }
        //}
    }
}
