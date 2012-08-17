using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    /// <summary>
    /// A comment node
    /// </summary>

    public class DomComment : DomObject<DomComment>, IDomComment
    {
        #region constructors

        /// <summary>
        /// Default constructor.
        /// </summary>

        public DomComment()
            : base()
        {
        }

        /// <summary>
        /// Constructor for a comment containing the specified text.
        /// </summary>
        ///
        /// <param name="text">
        /// The text.
        /// </param>

        public DomComment(string text): base()
        {
            //ParentNode = document;
            Text = text;
        }

        #endregion

        #region private properties

        // there must be some reason for this... something to do with the parser
        // ahh, old code. TODO: figure this out, probably refactor/remove

        private string TagOpener
        {
            get { return IsQuoted ? "<!--" : "<!"; }
        }
        private string TagCloser
        {
            get { return IsQuoted ? "-->" : ">"; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the type of the node (COMMENT_NODE)
        /// </summary>
        ///
        /// <value>
        /// The type of the node.
        /// </value>

        public override NodeType NodeType
        {
            get { return NodeType.COMMENT_NODE; }
        }

        /// <summary>
        /// The node (tag) name, in upper case. For a 
        /// </summary>
        ///
        /// <value>
        /// The name of the node.
        /// </value>

        public override string NodeName
        {
            get
            {
                return "#comment";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is quoted.
        /// </summary>
        ///
        /// <remarks>
        /// TODO: R&amp;R. This has to do with GetTagOpener etc.
        /// </remarks>
        ///
        /// <value>
        /// true if this object is quoted, false if not.
        /// </value>

        public bool IsQuoted { get; set; }


        /// <summary>
        /// Gets a value indicating whether HTML is allowed as a child of this element (false)
        /// </summary>
        ///
        /// <value>
        /// true if inner HTML allowed, false if not.
        /// </value>

        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this object has children (false)
        /// </summary>
        ///
        /// <value>
        /// true if this object has children, false if not.
        /// </value>

        public override bool HasChildren
        {
            get { return false; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Renders this object.
        /// </summary>
        ///
        /// <returns>
        /// A string of HTML
        /// </returns>

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



        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        ///
        /// <returns>
        /// This object as a string.
        /// </returns>

        public override string ToString()
        {
            string innerText = NonAttributeData.Length > 80 ? NonAttributeData.Substring(0, 80) + " ... " : NonAttributeData;
            return GetComment(innerText);
        }

        #endregion

        #region private methods


        private string GetComment(string innerText)
        {
            return TagOpener + innerText + TagCloser;
        }

        #endregion

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
