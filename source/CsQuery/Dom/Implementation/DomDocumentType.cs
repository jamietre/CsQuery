using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        private static Regex DocTypeRegex = new Regex(
                @"^\s*([a-zA-Z0-9]+)\s+[a-zA-Z]+(\s+""(.*?)"")*\s*$", 
            RegexOptions.IgnoreCase);
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
            protected set
            {
                _DocType = value;
            }
        }

        protected DocType _DocType = 0;

        public override string Render(DomRenderingOptions options = DomRenderingOptions.Default)
        {
            return "<!DOCTYPE " + NonAttributeData + ">";
        }

        public string NonAttributeData
        {
            get
            {
                return DocTypeName +
                    (!String.IsNullOrEmpty(PublicIdentifier) ? " \"" + PublicIdentifier +"\"": "") +
                    (!String.IsNullOrEmpty(SystemIdentifier) ? " \"" + SystemIdentifier +"\"": "");

            }
            set
            {
                string docTypeName="";
                string publicIdentifier="";
                string systemIdentifier="";

                MatchCollection matches = DocTypeRegex.Matches(value);
                if (matches.Count > 0)
                {
                    docTypeName = matches[0].Groups[1].Value;
                    if (matches[0].Groups.Count ==4 )
                    {
                        var grp = matches[0].Groups[3];
                        publicIdentifier = grp.Captures[0].Value;
                        if (grp.Captures.Count > 1)
                        {
                            systemIdentifier = grp.Captures[1].Value;
                        }
                    }
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
            clone.PublicIdentifier = PublicIdentifier;
            clone.SystemIdentifier = SystemIdentifier;
            clone.DocTypeName = DocTypeName;
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
