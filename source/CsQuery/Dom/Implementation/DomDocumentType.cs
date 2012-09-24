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

        #region constructors

        /// <summary>
        /// Default constructor.
        /// </summary>

        public DomDocumentType()
            : base()
        {

        }

        /// <summary>
        /// Constructor to create based on one of several common predefined types.
        /// </summary>
        ///
        /// <param name="docType">
        /// Type of the document.
        /// </param>

        public DomDocumentType(DocType docType)
            : base()
        {
            SetDocType(docType);
        }

        /// <summary>
        /// Constructor to create a specific document type node
        /// </summary>
        ///
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="publicIdentifier">
        /// Identifier for the public.
        /// </param>
        /// <param name="systemIdentifier">
        /// Identifier for the system.
        /// </param>

        public DomDocumentType(string type, string publicIdentifier, string systemIdentifier)
            : base()
        {

            SetDocType(type, publicIdentifier, systemIdentifier);
        }
        #endregion

        #region private properties

        private string DocTypeName { get; set; }
        private string PublicIdentifier {get; set;}
        private string SystemIdentifier { get; set; }

        #endregion
        public override NodeType NodeType
        {
            get { return NodeType.DOCUMENT_TYPE_NODE; }
        }
        
        public override string NodeName
        {
            get
            {
                return "DOCTYPE";
            }
        }

        public DocType DocType
        {
            get
            {
                if (_DocType == 0)
                {
                    throw new InvalidOperationException("The doc type has not been set.");
                }

                return _DocType;
            }
            set
            {
                SetDocType(value);
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
                //if (_DocType == 0)
                //{
                //    return _NonAttributeData;
                //}
                //else
                //{
                //    switch (_DocType)
                //    {
                //        case DocType.HTML5:
                //            return "html";
                //        case DocType.XHTML:
                //            return "html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"";
                //        case DocType.HTML4:
                //            return "html PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\"";
                //        default:
                //            throw new NotImplementedException("Unimplemented doctype");
                //    }

                //}
                return DocTypeName +
                    (!String.IsNullOrEmpty(PublicIdentifier) ? " " + PublicIdentifier : "") +
                    (!String.IsNullOrEmpty(SystemIdentifier) ? " " + SystemIdentifier : "");

            }
            set
            {
                string docTypeName;
                string publicIdentifier="";
                string systemIdentifier="";

                string[] parts = value.Split(' ');
                if (parts.Length >= 1)
                {
                    docTypeName = parts[0];
                } else {
                    throw new InvalidOperationException("The DocType must have a name, e.g. html");
                }

                if (parts.Length >= 3)
                {
                    publicIdentifier = parts[2];
                }
                if (parts.Length >= 4)
                {
                    systemIdentifier = String.Join(" ", parts.Skip(3));
                }
                SetDocType(docTypeName,publicIdentifier,systemIdentifier);
            }
        }

        private void SetDocType(string type, string publicIdentifier, string systemIdentifier)
        {
            DocTypeName = type.ToLower();
            PublicIdentifier = publicIdentifier==null ? "" : publicIdentifier.ToLower();
            SystemIdentifier = systemIdentifier==null ? "" : systemIdentifier.ToLower();

            if (DocTypeName == null || DocTypeName != "html")
            {
                DocType = DocType.Unknown;
                return;
            }
            if (PublicIdentifier == "")
            {
                DocType = DocType.HTML5;
                return;
            }
            else if (PublicIdentifier.Contains("html 4"))
            {
                DocType = DocType.HTML4;
            }
            else if (PublicIdentifier.Contains("xhtml 1"))
            {
                 DocType = DocType.XHTML;
            } else {
                DocType = DocType.Unknown;
            }
        }

        /// <summary>
        /// Sets document type data values from a doc type
        /// </summary>

        private void SetDocType(DocType type)
        {
            _DocType = type;
            switch (type)
            {
                case DocType.HTML5:
                    DocTypeName = "html";
                    SystemIdentifier = null;
                    PublicIdentifier = null;
                    break;
                case DocType.XHTML:
                    DocTypeName = "html PUBLIC";
                    break;
                    //SystemIdentifier = 
                    //return "html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"";
                case DocType.HTML4:
                    //return "html PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\"";
                    break;
                default:
                    throw new NotImplementedException("Unimplemented doctype");
            }
        }
        protected string _NonAttributeData = String.Empty;
        //protected DocType GetDocType()
        //{
        //    string data = NonAttributeData.Trim().ToLower();
        //    // strip off trailing slashes - easy mistake to make
        //    if (data.LastIndexOf("/") == data.Length - 1)
        //    {
        //        data = data.Substring(0, data.Length - 1).Trim();
        //    }
        //    if (data == "html")
        //    {
        //        return DocType.HTML5;
        //    }
        //    else if (data.IndexOf("xhtml 1") >= 0)
        //    {
        //        return DocType.XHTML;
        //    }
        //    else if (data.IndexOf("html 4") >= 0)
        //    {
        //        return DocType.HTML4;
        //    }
        //    else
        //    {
        //        return DocType.Unknown;
        //    }
        //}


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
