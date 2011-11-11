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
        /// <summary>
        /// Create a text node from an original DOM load (refernces the intial character array)
        /// </summary>
        /// <param name="domTextIndex"></param>
        public DomText(int domTextIndex)
            : base()
        {
            Initialize();
            textIndex = domTextIndex;
        }
        //public DomText(CsQuery owner)
        //    : base()
        //{
        //    _Owner = owner;
        //    Initialize();
        //}
        protected void Initialize()
        {
            Text = String.Empty;
        }
        public override NodeType NodeType
        {
            get { return NodeType.TEXT_NODE; }
        }
        protected int textIndex=-1;
        // for use during initial construction from char array
        public void SetTextIndex(int index)
        {
            textIndex = index;
        }
        public string Text
        {
            get
            {
                return textIndex >= 0 ? 
                    (stringRef != null ? stringRef : Dom).GetString(textIndex) 
                        : unboundText;
            }
            set
            {
                unboundText = value;
                textIndex = -1;
            }
        }
        DomRoot stringRef = null;
        string unboundText;
        //protected int TextID=-1;
        
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
            domText.textIndex = textIndex;
            domText.unboundText = unboundText;
            domText.stringRef = stringRef != null? stringRef: Dom;
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
                return textIndex >=0;
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
