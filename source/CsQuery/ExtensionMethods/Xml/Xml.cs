﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text;
using System.Reflection;
using System.Xml;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.ExtensionMethods.Xml
{
    /// <summary>
    /// Extension methods for converting to XML
    /// </summary>
    public static class ExtensionMethods
    {
        public static XmlDocument ToXml(this IDomDocument document) {
            throw new NotImplementedException();
            //return new CqXmlDocument(document);
        }


        public static XmlNodeList ToXml(this INodeList nodeList)
        {
            throw new NotImplementedException();
            //return new CqXmlNodeList(nodeList.Owner.Document.ToXml(), nodeList);
        }

        public static XmlNode ToXml(this IDomObject element)
        {
            throw new NotImplementedException();
            //var xmlDoc = element.Document.ToXml();
            //return element.ToXml(xmlDoc);
        }
        internal static XmlNode ToXml(this IDomObject element, XmlDocument xmlDoc)
        {
            throw new NotImplementedException();
            //return new CqXmlNode(xmlDoc, element);
        }

        
    }

}
