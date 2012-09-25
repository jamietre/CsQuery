using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Implementation;
using CsQuery.StringScanner;

namespace CsQuery.HtmlParser.Deprecated
{
    /// <summary>
    /// The HTML parser
    /// </summary>

    public class HtmlElementFactory
    {
        #region constructors

        public HtmlElementFactory(IDomDocument document)
        {
            Document = document;
            //SetHtml(document.DocumentIndex.SourceHtml);
            IsBound = true;
        }

        public HtmlElementFactory(char[] html)
        {
            Initialize();
            SetHtml(html);
            IsBound = false;
            
        }
        public HtmlElementFactory(string html)
        {
            Initialize();
            SetHtml(html.ToCharArray());
            IsBound = false;
        }

        private void Initialize()
        {
            IsDocument = false;
        }
        #endregion

        #region private properties

        // these are implemented as fields to reduce overhead when referencing from iteration data class

        internal int EndPos;
        internal bool IsBound;
        internal char[] Html;
        internal IDomDocument Document;
        private bool _IsDocument;
        private Func<ushort, ushort, ushort> SpecialTagActionDelegate;
        #endregion

        #region public properties

        public bool AddToIndex;

        /// <summary>
        /// When true, HTML5 optional elements will be created automatically. This spefically excludes 
        /// HTML, HEAD, TITLE, and BODY. Use IsFragment=true to create those.
        /// </summary>
        public bool GenerateOptionalElements;

        /// <summary>
        /// When true, HTML5 optional elements will be created automatically. This should be false for fragments.
        /// </summary>
        public bool IsDocument
        {
            get
            {
                return _IsDocument;
            }
            set
            {
                _IsDocument = value;
                SpecialTagActionDelegate = value ?
                    (Func<ushort,ushort,ushort>)HtmlData.SpecialTagActionForDocument :
                    (Func<ushort, ushort, ushort>)HtmlData.SpecialTagAction;
            }

        }

        /// <summary>
        /// When true, text nodes that are not the child of another elment will be wrapped in SPAN tags
        /// </summary>
        public bool WrapRootTextNodes;

        #endregion

        #region public methods

        /// <summary>
        /// Parse with options for a full HTML document.
        /// </summary>
        ///
        /// <returns>
        /// A list of IDomObject elements; the topmost sequence of the DOM.
        /// </returns>

        public List<IDomObject> ParseAsDocument()
        {
            IsDocument = true;
            GenerateOptionalElements = true;
            WrapRootTextNodes = false;
            return Parse();

        }

        /// <summary>
        /// Parse with options for fragment.
        /// </summary>
        ///
        /// <returns>
        /// A list of IDomObject elements; the topmost sequence of the fragment.
        /// </returns>

        public List<IDomObject> ParseAsFragment()
        {
            IsDocument = false;
            GenerateOptionalElements = false;
            WrapRootTextNodes = false;
            return Parse();
        }

        /// <summary>
        /// Parse with options for content (generate most optional elements but not document wrapping
        /// HTML/BODY)
        /// </summary>
        ///
        /// <returns>
        /// A list of IDomObject elements; the topmost sequence of the document.
        /// </returns>

        public List<IDomObject> ParseAsContent()
        {
            IsDocument = false;
            GenerateOptionalElements = true;
            WrapRootTextNodes = false;
            return Parse();
        }

        /// <summary>
        /// Parse the HTML, and return it, based on options set.
        /// </summary>
        ///
        /// <exception cref="NotImplementedException">
        /// Thrown when the requested parsing mode is unknown.
        /// </exception>
        ///
        /// <param name="htmlParsingMode">
        /// The HTML parsing mode.
        /// </param>
        ///
        /// <returns>
        /// A List of IDomObject elements; the topmost sequence of the document.
        /// </returns>

        public List<IDomObject> Parse(HtmlParsingMode htmlParsingMode)
        {
            switch (htmlParsingMode)
            {
                case HtmlParsingMode.Content:
                    return ParseAsContent();
                case HtmlParsingMode.Document:
                    return ParseAsDocument();
                case HtmlParsingMode.Fragment:
                    return ParseAsFragment();
                default:
                    throw new NotImplementedException("Unknown HTML parsing mode");
            }

        }

        /// <summary>
        /// Parse the HTML, and return it, based on options set.
        /// </summary>
        ///
        /// <returns>
        /// A List of IDomObject elements.
        /// </returns>
        ///
        /// <implementation>
        /// Because the output can be assigned directly to another object without being enumerated, we
        /// force it to do so with ToList() here. Otherwise the HTML could potentially be parsed
        /// repeatedly as the original enumerator is accessed directly by clients..
        /// </implementation>

        public List<IDomObject> Parse()
        {
            return ParseImplementation().ToList();
        }

        /// <summary>
        /// Parse the HTML, and return it, based on options set.
        /// </summary>
        ///
        /// <returns>
        /// An enumerator of the top-level elements.
        /// </returns>

        protected IEnumerable<IDomObject> ParseImplementation()
        {
            int pos=0;
            Stack<IterationData> stack = new Stack<IterationData>();

            while (pos <= EndPos)
            {
                IterationData current = new IterationData();
                if (WrapRootTextNodes)
                {
                    current.WrapLiterals = true;
                }

                current.Reset(pos);
                stack.Push(current);

                while (stack.Count != 0)
                {

                    current = stack.Pop();

                    while (current.TokenizerState != TokenizerState.Finished && current.Pos <= EndPos)
                    {
                        char c = Html[current.Pos];
                        switch (current.TokenizerState)
                        {
                            case TokenizerState.Default:
                                if (current.FindNextTag(Html)) {

                                    // even if we fell through from ReadTextOnly (e.g. was never closed), we should proceeed to finish
                                    current.TokenizerState = TokenizerState.TagStart;
                                }
                                break;
                            case TokenizerState.TagStart:
                                IDomObject literal;
                                if (current.TryGetLiteral(this, out literal))
                                {
                                    yield return literal;
                                }
                                
                                int tagStartPos = current.Pos;
                                
                                string newTag=current.GetTagOpener(Html);

                                if (newTag == String.Empty)
                                {
                            
                            
                                    // It's a tag closer. Make sure it's the right one.
                                    current.Pos = tagStartPos + 1;
                                    ushort closeTagId = HtmlData.Tokenize(current.GetCloseTag(Html));

                                    // Ignore empty tags, or closing tags found when no parent is open
                                    bool isProperClose = closeTagId == current.ParentTagID();
                                    if (closeTagId == 0)
                                    {
                                        // ignore empty tags
                                        continue;
                                    }
                                    else
                                    {
                                        // locate match for this closer up the heirarchy
                                        IterationData actualParent =null;
                                        
                                        if (!isProperClose)
                                        {
                                            actualParent = current.Parent;
                                            while (actualParent != null && actualParent.Element.NodeNameID != closeTagId)
                                            {
                                                actualParent = actualParent.Parent;
                                            }
                                        }
                                        // if no matching close tag was found up the tree, ignore it
                                        // otherwise always close this and repeat at the same position until the match is found
                                        if (!isProperClose && actualParent == null)
                                        {
                                            current.InsertionMode = InsertionMode.Invalid;
                                            continue;
                                        }
                                    }
                                   // element is closed 
                                    
                                    if (current.Parent.Parent == null)
                                    {
                                        yield return current.Parent.Element;
                                    }
                                    current.TokenizerState = TokenizerState.Finished ;
                                    if (isProperClose)
                                    {
                                        current.Parent.Reset(current.Pos);
                                    }
                                    else
                                    {
                                        current.Parent.Reset(tagStartPos);
                                    }
                                    // already been returned before we added the children
                                    continue;
                                } 
                                else if (newTag[0] == '!')
                                {
                                    IDomSpecialElement specialElement = null;
                                    string newTagUpper = newTag.ToUpper();
                                    if (newTagUpper.StartsWith("!DOCTYPE"))
                                    {
                                        specialElement = new DomDocumentType();
                                        current.Element = specialElement;
                                    }
                                    else if (newTagUpper.StartsWith("![CDATA["))
                                    {
                                        specialElement = new DomCData();
                                        current.Element = specialElement;
                                        current.Pos = tagStartPos + 9;
                                    }
                                    else 
                                    {
                                        specialElement = new DomComment();
                                        current.Element = specialElement;
                                        if (newTag.StartsWith("!--"))
                                        {
                                            ((DomComment)specialElement).IsQuoted = true;
                                            current.Pos = tagStartPos + 4;
                                        } else {
                                            current.Pos = tagStartPos+1;
                                        }
                                    }

                                    string endTag = (current.Element is IDomComment && ((IDomComment)current.Element).IsQuoted) ? "-->" : ">";

                                    int tagEndPos = Html.Seek(endTag, current.Pos);
                                    if (tagEndPos < 0)
                                    {
                                        // if a tag is unclosed entirely, then just find a new line.
                                        tagEndPos = Html.Seek(System.Environment.NewLine, current.Pos);
                                    }
                                    if (tagEndPos < 0)
                                    {
                                        // Never closed, no newline - junk, treat it like such
                                        tagEndPos = EndPos;
                                    }

                                    specialElement.NonAttributeData = Html.SubstringBetween(current.Pos, tagEndPos);
                                    current.Pos = tagEndPos;

                                }
                                else
                                {

                                    // seems to be a new element tag, parse it.

                                    ushort newTagId = HtmlData.Tokenize(newTag);
                                    
                                    // Before we keep going see if this is an implicit close
                                    ushort parentTagId = current.ParentTagID();

                                    int lastPos = current.Pos;

                                    if (parentTagId ==0 && IsDocument) {
                                        if (newTagId != HtmlData.tagHTML) {
                                            current.Element =DomElement.Create(HtmlData.tagHTML);
                                            current = current.AddNewChild();
                                            parentTagId = HtmlData.tagHTML;
                                        }
                                    }
                                    
                                    if (parentTagId != 0)
                                    {
                                        ushort action = SpecialTagActionDelegate(parentTagId, newTagId);

                                        while (action != HtmlData.tagActionNothing)
                                        {
                                            if (action == HtmlData.tagActionClose)
                                            {

                                                // track the next parent up the chain

                                                var newNode = (current.Parent != null) ?
                                                    current.Parent : null;

                                                // same tag for a repeater like li occcurred - treat like a close tag

                                                if (current.Parent.Parent == null)
                                                {
                                                    yield return current.Parent.Element;
                                                }

                                                current.TokenizerState = TokenizerState.Finished;
                                                //current.Parent.Reset(tagStartPos);

                                                if (newNode != null && newNode.Parent != null && newNode.Parent.Element != null)
                                                {
                                                    action = SpecialTagActionDelegate(newNode.Parent.Element.NodeNameID, newTagId);
                                                    if (action != HtmlData.tagActionNothing)
                                                    {
                                                        current = newNode;
                                                    }
                                                }
                                                else
                                                {
                                                    action = HtmlData.tagActionNothing;
                                                }
                                            }
                                            else 
                                            {
                                                if (GenerateOptionalElements)
                                                {
                                                    stack.Push(current);
                                                    current = current.AddNewParent(action, lastPos);

                                                }
                                                action = HtmlData.tagActionNothing;

                                            }
                                        }
                                        if (current.TokenizerState == TokenizerState.Finished)
                                        {
                                            current.Parent.Reset(tagStartPos);
                                            continue;
                                        }

                                    }

                                    
                                    current.Element = DomElement.Create(newTagId);


                                    if (!current.Element.InnerHtmlAllowed && current.Element.InnerTextAllowed)
                                    {
                                        current.InsertionMode = InsertionMode.Text;
                                        current.TokenizerState = TokenizerState.Default;
                                    }

                                    // Parse attribute data
                                    while (current.Pos <= EndPos)
                                    {
                                        if (!current.GetTagAttribute(Html)) break;
                                    }
                                }

                                IDomObject el;
                                
                                if (current.FinishTagOpener(Html, out el))
                                {
                                    stack.Push(current);
                                    current = current.AddNewChild();
                                }

                                if (el != null)
                                {
                                    yield return el;
                                }

                                break;

                        }
                    }


                    // Catchall for unclosed tags -- if there's an "unfinished" carrier here, it's because  top-level tag was unclosed.
                    // THis will wrap up any straggling text and close any open tags after it.

                    if (current.TokenizerState != TokenizerState.Finished)
                    {
                        foreach (var el in current.CloseElement(this)) {
                            yield return el;
                        }

                    }
                }
                pos = current.Pos;
            }

        }

        #endregion

        #region private methods


        /// <summary>
        /// In the future I will update the parser to do this directly, since this requires binding to a Document to work.
        /// </summary>
        public static void  ReorganizeStrandedTextNodes(IDomDocument document)
        {

            if (document.DocTypeNode == null)
            {
                var docType = new DomDocumentType(DocType.HTML5);
                document.ChildNodes.Insert(0, docType);

            }

            // ignore everything before <html> except text; if found, start adding to <body>
            // if there's anything before <doctype> then it gets trashed

            IDomElement html = (IDomElement)document.GetElementsByTagName("html").FirstOrDefault();

            if (html != null && document.GetElementsByTagName("head").FirstOrDefault() == null)
            {
                html.ChildNodes.Insert(0, document.CreateElement("head"));
            }



            IDomElement body = (IDomElement)document.GetElementsByTagName("body").FirstOrDefault();
            if (body != null)
            {

                bool textYet = false;
                bool anythingYet = false;
                int bodyIndex = 0;
                int index = 0;
                // there should only be DocType & HTML.
                while (index < document.ChildNodes.Count)
                {
                    IDomObject obj = document.ChildNodes[index];
                    switch (obj.NodeType)
                    {
                        case NodeType.DOCUMENT_TYPE_NODE:
                            if (!anythingYet)
                            {
                                index++;
                            }
                            else
                            {
                                document.ChildNodes.RemoveAt(index);
                            }
                            break;
                        case NodeType.ELEMENT_NODE:
                            if (obj.NodeName == "HTML")
                            {
                                bodyIndex = body.ChildNodes.Length;
                                index++;
                            }
                            else
                            {
                                if (textYet)
                                {
                                    body.ChildNodes.Insert(bodyIndex++, obj);
                                }
                                else
                                {
                                    index++;
                                }
                                continue;
                            }
                            break;
                        case NodeType.TEXT_NODE:
                            if (!textYet)
                            {
                                // if a node is only whitespace and there has not yet been a non-whitespace text node,
                                // then ignore it.

                                var scanner = StringScanner.Scanner.Create(obj.NodeValue);
                                scanner.SkipWhitespace();
                                if (scanner.Finished)
                                {
                                    document.ChildNodes.RemoveAt(index);
                                    continue;
                                }
                                else
                                {
                                    textYet = true;
                                }
                            }

                            body.ChildNodes.Insert(bodyIndex++, obj);
                            break;
                        default:
                            body.ChildNodes.Insert(bodyIndex++, obj);
                            break;
                    }
                }
            }
        }

        private void SetHtml(char[] html)
        {
            Html = html;
            SetLength();
        }

        private void SetLength()
        {
            EndPos = Html==null  ? 
                -1 :  
                Html.Length - 1;
        }

        #endregion

    }
}
