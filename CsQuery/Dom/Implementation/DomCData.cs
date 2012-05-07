using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{

    public class DomCData : DomObject<DomCData>, IDomCData
    {
        public DomCData()
            : base()
        {

        }
        public DomCData(string value)
            : base()
        {
            NodeValue = value;
        }
        public override string NodeValue
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
        public override NodeType NodeType
        {
            get { return NodeType.CDATA_SECTION_NODE; }
        }
        public override string Render()
        {
            return GetHtml(NonAttributeData);
        }
        protected string GetHtml(string innerText)
        {
            return "<![CDATA[" + innerText + ">";
        }
        public override string ToString()
        {
            string innerText = NonAttributeData.Length > 80 ? NonAttributeData.Substring(0, 80) + " ... " : NonAttributeData;
            return GetHtml(innerText);
        }
        #region IDomSpecialElement Members

        public string NonAttributeData
        {
            get;
            set;
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
        public override DomCData Clone()
        {
            DomCData clone = new DomCData();
            clone.NonAttributeData = NonAttributeData;
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
