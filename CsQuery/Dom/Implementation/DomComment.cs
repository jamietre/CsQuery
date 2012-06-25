using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    
    public class DomComment : DomObject<DomComment>, IDomComment
    {
        public DomComment()
            : base()
        {
        }

        public DomComment(string text): base()
        {
            //ParentNode = document;
            Text = text;
        }
        public override NodeType NodeType
        {
            get { return NodeType.COMMENT_NODE; }
        }
        public bool IsQuoted { get; set; }
        protected string TagOpener
        {
            get { return IsQuoted ? "<!--" : "<!"; }
        }
        protected string TagCloser
        {
            get { return IsQuoted ? "-->" : ">"; }
        }
        public override string Render()
        {
            if (Document != null
                && Document.DomRenderingOptions.HasFlag(DomRenderingOptions.RemoveComments))
            {
                return String.Empty;
            }
            else
            {
                return GetComment(NonAttributeData);
            }
        }
        protected string GetComment(string innerText)
        {
            return TagOpener + innerText + TagCloser;
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
            get { return true; }
        }
        public override string ToString()
        {
            string innerText = NonAttributeData.Length > 80 ? NonAttributeData.Substring(0, 80) + " ... " : NonAttributeData;
            return GetComment(innerText);
        }
        #region IDomSpecialElement Members

        public string NonAttributeData
        {
            get;
            set;
        }

        public string Text
        {
            get
            {
                return NonAttributeData;
            }
            set
            {
                NonAttributeData = value;
            }
        }
        public override DomComment Clone()
        {
            DomComment clone = new DomComment();
            clone.NonAttributeData = NonAttributeData;
            clone.IsQuoted = IsQuoted;
            return clone;
        }

        IDomNode IDomNode.Clone()
        {
            return Clone();
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }
}
