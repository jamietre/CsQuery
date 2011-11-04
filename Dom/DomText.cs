using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jtc.CsQuery
{
    /// <summary>
    /// Defines an interface for elements whose defintion (not innerhtml) contain non-tag or attribute formed data
    /// </summary>

    public interface IDomText : IDomObject
    {
        string Text { get; set; }
    }


    /// <summary>
    /// Used for literal text (not part of a tag)
    /// </summary>
    public class DomText : DomObject<DomText>, IDomText
    {
        public DomText()
        {
            Initialize();
        }

        public DomText(string text)
            : base()
        {
            Initialize();
            Text = text;
        }
        public DomText(CsQuery owner)
            : base()
        {
            _Owner = owner;
            Initialize();
        }
        protected void Initialize()
        {
            Text = String.Empty;
        }
        public override NodeType NodeType
        {
            get { return NodeType.TEXT_NODE; }
        }
        public string Text
        {
            get
            {
                return TextID == -1 ? "" :
                    TextID == -2 ? unboundText :
                    Dom.TextContent[TextID];
            }
            set
            {
                if (Dom != null)
                {
                    if (TextID >= 0)
                    {
                        Dom.TextContent[TextID] = value;
                    }
                    else
                    {
                        Dom.TextContent.Add(value);
                        TextID = Dom.TextContent.Count - 1;
                    }
                    
                }
                else
                {
                    unboundText = value;
                    TextID = -2;
                }
            }
        }
        string unboundText;
        protected int TextID=-1;
        
        public override string NodeValue
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }
        public override string Render()
        {
            return Text;
        }
        public override DomText Clone()
        {
            DomText domText = base.Clone();
            domText.Text = Text;
            return domText;
        }

        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }
        public override bool HasChildren
        {
            get { return false; }
        }
        public override bool Complete
        {
            get { 
                //return !String.IsNullOrEmpty(Text); 
                return TextID != -1;
            }
        }
        public override string ToString()
        {
            return Text;
        }
        public override string InnerText
        {
            get
            {
                return HttpUtility.HtmlDecode(Text);
            }
            set
            {
                Text = HttpUtility.HtmlEncode(value);
            }
        }
    }
}
