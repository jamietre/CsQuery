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
            //Initialize();
        }

        public DomText(string text)
            : base()
        {
            //Initialize();
            Text = text;
        }

        //public DomText(CsQuery owner)
        //    : base()
        //{
        //    _Owner = owner;
        //    Initialize();
        //}
        //protected void Initialize()
        //{
        //    Text = String.Empty;
        //}
        public override NodeType NodeType
        {
            get { return NodeType.TEXT_NODE; }
        }
        private int textIndex=-1;
        // for use during initial construction from char array
        public void SetTextIndex(DomRoot dom, int index)
        {
            textIndex = index;
            // create a hard reference to the DOM from which we are mapping our string data. Otherwise if this
            // is moved to another dom, it will break
            stringRef = dom;
        }
        public string Text
        {
            get
            {
                return textIndex >= 0 ? 
                    stringRef.GetString(textIndex) 
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
            domText.stringRef = stringRef;
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
