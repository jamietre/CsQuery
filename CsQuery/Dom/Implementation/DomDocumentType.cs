using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{

    /// <summary>
    /// A special type for the DOCTYPE node
    /// </summary>

    public class DomDocumentType : DomObject<DomDocumentType>, IDomDocumentType
    {
        public DomDocumentType()
            : base()
        {

        }
        public DomDocumentType(DocType docType)
            : base()
        {
            DocType = docType;
        }
        public override NodeType NodeType
        {
            get { return NodeType.DOCUMENT_TYPE_NODE; }
        }
        public DocType DocType
        {
            get
            {
                if (_DocType != 0)
                {
                    return _DocType;
                }
                else
                {
                    return GetDocType();
                }
            }
            set
            {
                _DocType = value;
            }
        }
        protected DocType _DocType = 0;

        public override string Render()
        {
            return "<!DOCTYPE " + NonAttributeData + ">";
        }

        public string NonAttributeData
        {
            get
            {
                if (_DocType == 0)
                {
                    return _NonAttributeData;
                }
                else
                {
                    switch (_DocType)
                    {
                        case DocType.HTML5:
                            return "html";
                        case DocType.XHTML:
                            return "html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"";
                        case DocType.HTML4:
                            return "html PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\"";
                        default:
                            throw new NotImplementedException("Unimplemented doctype");
                    }

                }
            }
            set
            {
                _NonAttributeData = value.Trim();
                DocType = GetDocType();
            }
        }
        protected string _NonAttributeData = String.Empty;
        protected DocType GetDocType()
        {
            string data = NonAttributeData.Trim().ToLower();
            // strip off trailing slashes - easy mistake to make
            if (data.LastIndexOf("/") == data.Length - 1)
            {
                data = data.Substring(0, data.Length - 1).Trim();
            }
            if (data == "html")
            {
                return DocType.HTML5;
            }
            else if (data.IndexOf("xhtml 1") >= 0)
            {
                return DocType.XHTML;
            }
            else if (data.IndexOf("html 4") >= 0)
            {
                return DocType.HTML4;
            }
            else
            {
                return DocType.Unknown;
            }
        }


        public override bool InnerHtmlAllowed
        {
            get { return false; }
        }
        public override bool HasChildren
        {
            get { return false; }
        }

        #region interface Members

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

        public override DomDocumentType Clone()
        {
            DomDocumentType clone = new DomDocumentType();
            clone.NonAttributeData = NonAttributeData;
            clone.DocType = DocType;
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
