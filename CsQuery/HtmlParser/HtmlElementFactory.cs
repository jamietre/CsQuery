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

namespace CsQuery.HtmlParser
{
    public class HtmlElementFactory
    {
        #region constructors

        public HtmlElementFactory(IDomDocument document)
        {
            Document = document;
            SetHtml(document.SourceHtml);
            IsBound = true;
        }

        public HtmlElementFactory(char[] html)
        {
            SetHtml(html);
            IsBound = false;
        }
        public HtmlElementFactory(string html)
        {
            SetHtml(html.ToCharArray());
            IsBound = false;
        }
        #endregion

        #region private properties

        // these are implemented as fields to reduce overhead when referencing from iteration data class

        internal int EndPos;
        internal bool IsBound;
        internal char[] Html;
        internal IDomDocument Document;

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
        public bool IsDocument;

        /// <summary>
        /// When true, text nodes that are not the child of another elment will be wrapped in SPAN tags
        /// </summary>
        public bool WrapRootTextNodes;

        #endregion

        #region public methods


        
        /// <summary>
        /// returns a single element, any html is discarded after that
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        //public IDomElement CreateElement(string html)
        //{
        //    SetBaseHtml(html);
        //    return (IDomElement)Parse(false).First();
        //}
        /// <summary>
        /// Returns a list of unbound elements created by parsing the string. Even if Document is set, this will not return bound elements.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        //public IEnumerable<IDomObject> CreateObjects(string html)
        //{
        //    isBound = false;
        //    return CreateObjectsImpl(html, true);
        //}
        /// <summary>
        /// Returns a list of unbound elements created by parsing the string. Even if Document is set, this will not return bound elements.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        ///// <returns></returns>
        //public IEnumerable<IDomObject> CreateObjects(char[] html)
        //{
        //    isBound = false;
        //    return CreateObjectsImpl(html, true);
        //}
        /// <summary>
        /// Returns a list of elements from the bound Document
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<IDomObject> CreateObjects()
        //{
        //    if (Document == null)
        //    {
        //        throw new InvalidOperationException("This method requires Document be set");
        //    }
        //    isBound = true;

        //    return CreateObjectsImpl(Document.SourceHtml,true);
        //}

        //protected IEnumerable<IDomObject> CreateObjectsImpl(string html, bool allowLiterals)
        //{
        //    SetBaseHtml(html);
        //    return Parse(allowLiterals);
        ////}
        //protected IEnumerable<IDomObject> CreateObjectsImpl(char[] html, bool allowLiterals)
        //{

        //    SetBaseHtml(html);
        //    return Parse(allowLiterals);
        //}

        /// <summary>
        /// Parse with options for a full HTML document. Not that this method WILL NOT handle stranded text nodes (outside
        /// Body) right now. This only works with ParseToDocument.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomObject> ParseAsDocument()
        {
            IsDocument = true;
            GenerateOptionalElements = true;
            WrapRootTextNodes = false;
            return Parse();
        }

        /// <summary>
        /// Parse with options for fragment
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomObject> ParseAsFragment()
        {
            IsDocument = false;
            GenerateOptionalElements = false;
            WrapRootTextNodes = false;
            return Parse();
        }


        /// <summary>
        /// Parse with options for content (generate most optional elements but not document wrapping HTML/BODY)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomObject> ParseAsContent()
        {
            IsDocument = false;
            GenerateOptionalElements = true;
            WrapRootTextNodes = true;
            return Parse();
        }

        /// <summary>
        /// Parse the HTML, and return it, based on options set.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomObject> Parse()
        {
            int pos=0;
            Stack<IterationData> stack = new Stack<IterationData>();

            while (pos <= EndPos)
            {
                IterationData current = new IterationData();
                current.AllowLiterals = !WrapRootTextNodes;
                current.Reset(pos);
                stack.Push(current);

                while (stack.Count != 0)
                {

                    current = stack.Pop();
                    //Debug.Assert(current.Object == null);

                    while (!current.Finished && current.Pos <= EndPos)
                    {
                        char c = Html[current.Pos];
                        switch (current.Step)
                        {
                            case 0:
                                if (current.FindNextTag(Html)) {

                                    // even if we fell through from ReadTextOnly (e.g. was never closed), we should proceeed to finish
                                    current.Step=1;
                                }
                                break;
                            case 1:
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
                                    ushort closeTagId = HtmlData.TokenID(current.GetCloseTag(Html));

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
                                            current.Invalid = true;
                                            continue;
                                        }
                                    }
                                   // element is closed 
                                    
                                    if (current.Parent.Parent == null)
                                    {
                                        yield return current.Parent.Element;
                                    }
                                    current.Finished = true;
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
                                        current.Object = specialElement;
                                    }
                                    else if (newTagUpper.StartsWith("![CDATA["))
                                    {
                                        specialElement = new DomCData();
                                        current.Object = specialElement;
                                        current.Pos = tagStartPos + 9;
                                    }
                                    else 
                                    {
                                        specialElement = new DomComment();
                                        current.Object = specialElement;
                                        if (newTag.StartsWith("!--"))
                                        {
                                            ((DomComment)specialElement).IsQuoted = true;
                                            current.Pos = tagStartPos + 4;
                                        } else {
                                            current.Pos = tagStartPos+1;
                                        }
                                    }

                                    string endTag = (current.Object is IDomComment && ((IDomComment)current.Object).IsQuoted) ? "-->" : ">";

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

                                    ushort newTagId = HtmlData.TokenID(newTag);
                                    
                                    // Before we keep going see if this is an implicit close
                                    ushort parentTagId = current.ParentTagID();
                                    
                                    if (parentTagId ==0 && IsDocument) {
                                        if (newTagId != HtmlData.tagHTML) {
                                            current.Object = new DomElement(HtmlData.tagHTML);
                                            current = current.AddNewChild();
                                            parentTagId = HtmlData.tagHTML;
                                        }
                                    }

                                    if (parentTagId != 0)
                                    {
                                        ushort action = HtmlData.SpecialTagAction(parentTagId, newTagId, IsDocument);

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

                                                current.Finished = true;

                                                if (newNode != null && newNode.Parent != null && newNode.Parent.Element != null)
                                                {
                                                    action = HtmlData.SpecialTagAction(newNode.Parent.Element.NodeNameID, newTagId,IsDocument);
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
                                                    current = current.AddNewParent(action);

                                                }
                                                action = HtmlData.tagActionNothing;

                                            }
                                        }

                                        if (current.Finished)
                                        {
                                            current.Parent.Reset(tagStartPos);
                                            continue;
                                        }
                                    }
                                    
                                    current.Object = new DomElement(newTagId);
                                    

                                    if (!current.Element.InnerHtmlAllowed && current.Element.InnerTextAllowed)
                                    {
                                        current.ReadTextOnly = true;
                                        current.Step = 0;
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

                    if (!current.Finished)
                    {
                        foreach (var el in current.CloseElement(this)) {
                            yield return el;
                        }

                    }
                }
                pos = current.Pos;
            }

        }

        /// <summary>
        /// Parse the HTML into the bound document
        /// </summary>
        public void ParseToDocument()
        {
            foreach (IDomObject obj in ParseAsDocument())
            {
                Document.ChildNodes.AddAlways(obj);
            }
            ReorganizeStrandedTextNodes();
        }

        #endregion

        #region private methods


        /// <summary>
        /// In the future I will update the parser to do this directly, since this requires binding to a Document to work.
        /// </summary>
        private void ReorganizeStrandedTextNodes()
        {
            // ignore everything before <html> except text; if found, start adding to <body>
            // if there's anything before <doctype> then it gets trashed

            IDomElement html = Document.GetElementByTagName("html");

            if (html!= null && Document.GetElementByTagName("head") == null)
            {
                html.ChildNodes.Insert(0,Document.CreateElement("head"));
            }


            IDomElement body = Document.GetElementByTagName("body");
            if (body != null)
            {

                bool textYet = false;
                bool anythingYet = false;
                int bodyIndex = 0;
                int index = 0;
                // there should only be DocType & HTML.
                while (index < Document.ChildNodes.Count)
                {
                    IDomObject obj = Document.ChildNodes[index];
                    switch (obj.NodeType)
                    {
                        case NodeType.DOCUMENT_TYPE_NODE:
                            if (!anythingYet)
                            {
                                index++;
                            }
                            else
                            {
                                Document.ChildNodes.RemoveAt(index);
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
                                    Document.ChildNodes.RemoveAt(index);
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
