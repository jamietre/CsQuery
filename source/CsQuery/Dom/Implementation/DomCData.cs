using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    /// <summary>
    /// A CDATA node.
    /// </summary>

    public class DomCData : DomObject<DomCData>, IDomCData
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public DomCData()
            : base()
        {

        }

        /// <summary>
        /// Constructor, creates a new CDATA node with the specified value
        /// </summary>
        ///
        /// <param name="value">
        /// The value.
        /// </param>

        public DomCData(string value)
            : base()
        {
            NodeValue = value;
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>

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

        /// <summary>
        /// Gets the type of the node. For CDATA nodes, this is always NodeType.CDATA_SECTION_NODE
        /// </summary>

        public override NodeType NodeType
        {
            get { return NodeType.CDATA_SECTION_NODE; }
        }


        #region IDomSpecialElement Members

        /// <summary>
        /// Gets or sets the information describing the non attribute.
        /// </summary>

        public string NonAttributeData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether HTML is allowed as a child of this element. It is possible
        /// for this value to be false but InnerTextAllowed to be true for elements which can have inner
        /// content, but no child HTML markup, such as &lt;textarea&gt; and &lt;script&gt;
        /// </summary>

        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this object has children.
        /// </summary>

        public override bool HasChildren
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the text of the CData node
        /// </summary>

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

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        ///
        /// <returns>
        /// A copy of this object.
        /// </returns>

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
