using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Jtc.CsQuery.Utility
{
    public static class HtmlDom
    {
        public static Dictionary<string, CssStyle> StyleDefs = new Dictionary<string, CssStyle>();
        private static char[] StringSep = new char[] { ' ' };
        public static string CssDefs = "css3.xml";
        static HtmlDom()
        {
            XmlDocument xDoc = new XmlDocument();
            Stream dataStream = Support.GetResourceStream("Jtc.CsQuery.Resources." + CssDefs);
            //XDocument data = XDocument.Load(dataStream);
            xDoc.Load(dataStream);

            XmlNamespaceManager nsMan = new XmlNamespaceManager(xDoc.NameTable);
            nsMan.AddNamespace("cssmd", "http://schemas.microsoft.com/Visual-Studio-Intellisense/css");

            var nodes = xDoc.DocumentElement.SelectNodes("cssmd:property-set/cssmd:property-def", nsMan);

            string type;

            foreach (XmlNode  el in nodes)
            {
                CssStyle st = new CssStyle();
                st.Name = el.Attributes["_locID"].Value;
                type = el.Attributes["type"].Value;
                switch (type)
                {
                    case "length": st.Type = CssStyleType.Unit; break;
                    case "color": st.Type = CssStyleType.Color; break;
                    case "composite": st.Type = CssStyleType.Composite;
                        st.Format = el.Attributes["syntax"].Value;
                        break;
                    case "enum":
                    case "enum-length":
                        
                        if (type == "enum-length")
                        {
                            st.Type = CssStyleType.UnitOption;
                        } else {
                            st.Type=CssStyleType.Option;
                        }
                        st.Options = new HashSet<string>(el.Attributes["enum"].Value
                            .Split(StringSep, StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case "font":
                        st.Type = CssStyleType.Font;
                        break;
                    case "string":
                        st.Type = CssStyleType.String;
                        break;
                    case "url":
                        st.Type = CssStyleType.Url;
                        break;
                    default:
                        throw new Exception("Error parsing css xml: unknown type '" + type + "'");
                }
                st.Description = el.Attributes["description"].Value;
                StyleDefs[st.Name] = st;
            }
        }
            
        public  static HashSet<string> BooleanAttributes = new HashSet<string>(new string[] {
            "autobuffer", "autofocus", "autoplay", "async", "checked", "compact", "controls", "declare", "defaultmuted", "defaultselected",
            "defer", "disabled", "draggable", "formNoValidate", "hidden", "indeterminate", "ismap", "itemscope","loop", "multiple",
            "muted", "nohref", "noresize", "noshade", "nowrap", "novalidate", "open", "pubdate", "readonly", "required", "reversed",
            "scoped", "seamless", "selected", "spellcheck", "truespeed"," visible"
        });

    }
}
