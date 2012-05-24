using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Implementation;
using CsQuery.Utility.StringScanner;

namespace CsQuery.HtmlParser
{
    public class DomElementFactory
    {
        public DomElementFactory(IDomDocument document)
        {
            Document = document;
        }
        public IDomDocument Document { get; set; }
        protected char[] BaseHtml;
        protected int EndPos
        {
            get
            {
                if (_EndPos == -1)
                {
                    _EndPos = BaseHtml.Length - 1;
                }
                return _EndPos;
            }
        } protected int _EndPos = -1;
        protected void SetBaseHtml(char[] baseHtml)
        {
            _EndPos = -1;
            BaseHtml = baseHtml;
        }
        protected void SetBaseHtml(string baseHtml)
        {
            SetBaseHtml(baseHtml.ToCharArray());
        }
        /// <summary>
        /// No literals allowed
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IEnumerable<IDomElement> CreateElements(string html)
        {
            foreach (IDomObject obj in CreateObjectsImpl(html.ToCharArray(), false))
            {
                yield return (IDomElement)obj;
            }
        }
        /// <summary>
        /// returns a single element, any html is discarded after that
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IDomElement CreateElement(string html)
        {
            SetBaseHtml(html);
            return (IDomElement)Parse(false).First();
        }
        /// <summary>
        /// Returns a list of unbound elements created by parsing the string. Even if Document is set, this will not return bound elements.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> CreateObjects(string html)
        {
            isBound = false;
            return CreateObjectsImpl(html, true);
        }
        /// <summary>
        /// Returns a list of unbound elements created by parsing the string. Even if Document is set, this will not return bound elements.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> CreateObjects(char[] html)
        {
            isBound = false;
            return CreateObjectsImpl(html, true);
        }
        /// <summary>
        /// Returns a list of elements from the bound Document
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomObject> CreateObjects()
        {
            if (Document == null)
            {
                throw new InvalidOperationException("This method requires Document be set");
            }
            isBound = true;

            return CreateObjectsImpl(Document.SourceHtml,true);
        }

        protected IEnumerable<IDomObject> CreateObjectsImpl(string html, bool allowLiterals)
        {
            SetBaseHtml(html);
            return Parse(allowLiterals);
        }
        protected IEnumerable<IDomObject> CreateObjectsImpl(char[] html, bool allowLiterals)
        {

            SetBaseHtml(html);
            return Parse(allowLiterals);
        }

       

        //protected CsQuery Owner;
        protected bool isBound;
        /// <summary>
        /// When CsQuery is provided, an initial indexing context can be used
        /// </summary>
        /// <param name="csq"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        protected IEnumerable<IDomObject> Parse(bool allowLiterals)
        {
    
            int pos=0;
            Stack<IterationData> stack = new Stack<IterationData>();

            while (pos <= EndPos)
            {
                IterationData current = new IterationData();
                current.AllowLiterals = allowLiterals;
                current.Reset(pos);
                stack.Push(current);

                while (stack.Count != 0)
                {

                    current = stack.Pop();
                    //Debug.Assert(current.Object == null);

                    while (!current.Finished && current.Pos <= EndPos)
                    {
                        char c = BaseHtml[current.Pos];
                        switch (current.Step)
                        {
                            case 0:
                                current.Pos = CharIndexOf(BaseHtml, '<', current.Pos);
                                if (current.Pos  < 0)
                                {
                                    // done - no new tags found
                                    current.Pos = EndPos + 1;
                                }
                                else {
                                    // deal with when we're in a literal block (script/textarea)
                                    if (current.ReadTextOnly)
                                    {
                                        int endPos = current.Pos;
                                        while (endPos >= 0)
                                        {
                                            // keep going until we find the closing tag for this element
                                            int caretPos = CharIndexOf(BaseHtml, '>', endPos + 1);
                                            if (caretPos > 0)
                                            {
                                                string tag = BaseHtml.SubstringBetween(endPos + 1, caretPos).Trim().ToLower();
                                                if (tag == "/" +current.Parent.Element.NodeName)
                                                {
                                                    // this is the end tag -- exit the block
                                                    current.Pos=endPos;
                                                    break;
                                                }
                                            }
                                            endPos = CharIndexOf(BaseHtml, '<', endPos + 1);
                                        }
                                    }
                                    // even if we fell through from ReadTextOnly (e.g. was never closed), we should proceeed to finish
                                    current.Step=1;
                                }
                                break;
                            case 1:
                                if (current.Pos > current.HtmlStart)
                                {
                                    IDomObject literal = GetLiteral(current);
                                    if (literal != null)
                                    {
                                        yield return literal;
                                    }
 
                                    continue;
                                }

                                int tagStartPos = current.Pos;
                                string newTag;
                                
                                newTag = GetTagOpener(current);
                                
                                string newTagLower = newTag.ToLower();
                                
                                // when Element exists, it's because a previous iteration created it: it's our parent
                                string parentTag = String.Empty;
                                if (current.Parent != null)
                                {
                                    parentTag = current.Parent.Element.NodeName.ToLower();
                                }

                                if (newTag == String.Empty)
                                {
                                    // It's a tag closer. Make sure it's the right one.
                                    current.Pos = tagStartPos + 1;
                                    string closeTag = GetCloseTag(current);

                                    // Ignore empty tags, or closing tags found when no parent is open
                                    bool isProperClose = closeTag.ToLower() == parentTag;
                                    if (closeTag == String.Empty)
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
                                            while (actualParent != null && actualParent.Element.NodeName.ToLower() != closeTag.ToLower())
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
                                // Before we keep going see if this is an implicit close
                                if (parentTag != String.Empty)
                                {
                                    if (TagHasImplicitClose(parentTag,newTag)
                                        && parentTag == newTag)
                                    {
                                        // same tag for a repeater like li occcurred - treat like a close tag
                                        if (current.Parent.Parent == null)
                                        {
                                            yield return current.Parent.Element;
                                        }
                                        current.Parent.Reset(tagStartPos);
                                        current.Finished = true;

                                        continue;
                                    }
                                }
                                // seems to be a new tag. Parse it
                                
                                IDomSpecialElement specialElement = null;
                                
                                if (newTagLower[0] == '!')
                                {
                                    if (newTagLower.StartsWith("!doctype"))
                                    {
                                        specialElement = new DomDocumentType();
                                        current.Object = specialElement;
                                    }
                                    else if (newTagLower.StartsWith("![cdata["))
                                    {
                                        specialElement = new DomCData();
                                        current.Object = specialElement;
                                        current.Pos = tagStartPos + 9;
                                    }
                                    else 
                                    {
                                        specialElement = new DomComment();
                                        current.Object = specialElement;
                                        if (newTagLower.StartsWith("!--"))
                                        {
                                            ((DomComment)specialElement).IsQuoted = true;
                                            current.Pos = tagStartPos + 4;
                                        } else {
                                            current.Pos = tagStartPos+1;
                                        }
                                    }
                                }
                                else
                                {
                                    current.Object = new DomElement(newTag);
                                    
                                    if (!current.Element.InnerHtmlAllowed && current.Element.InnerTextAllowed)
                                    {
                                        current.ReadTextOnly = true;
                                        current.Step = 0;
                                    }
                                }
                                
                                // Handle non-element/text types -- they have data inside the tag construct
                                
                                if (current.Object is IDomSpecialElement)
                                {
                                    string endTag = (current.Object is IDomComment && ((IDomComment)current.Object).IsQuoted) ? "-->" : ">";

                                    int tagEndPos = BaseHtml.Seek(endTag, current.Pos);
                                    if (tagEndPos <0)
                                    {
                                        // if a tag is unclosed entirely, then just find a new line.
                                        tagEndPos = BaseHtml.Seek(System.Environment.NewLine, current.Pos);
                                    }
                                    if (tagEndPos < 0)
                                    {
                                        // Never closed, no newline - junk, treat it like such
                                        tagEndPos = EndPos;
                                    }

                                    specialElement.NonAttributeData = BaseHtml.SubstringBetween(current.Pos, tagEndPos);
                                    current.Pos = tagEndPos;
                                }
                                else
                                {
                                    // Parse attribute data
                                    while (current.Pos <= EndPos)
                                    {
                                        if (!GetTagAttribute(current)) break;
                                    }
                                }

                                bool hasChildren = MoveOutsideTag(current);

                                // tricky part: if there are children, push ourselves back on the stack and start with a new object
                                // from this position. The children will add themselves as they are created, avoiding recursion.
                                // When the close tag is found, the parent will be yielded if it's a root element.
                                // I think there's a slightly better way to do this, capturing all the yield logic at the end of the
                                // stack but it works for now.

                                if (current.Parent != null)
                                {
                                    current.Parent.Element.AppendChild(current.Object);
                                } else if (!hasChildren) {
                                    yield return current.Object;
                                }

                                if (!hasChildren)
                                {
                                    current.Reset();
                                    continue;
                                }

                                stack.Push(current);

                                IterationData subItem = new IterationData();
                                subItem.Parent = current;
                                subItem.AllowLiterals = true;
                                subItem.Reset(current.Pos);
                                subItem.ReadTextOnly = current.ReadTextOnly;
                                current = subItem;
                                break;

                        }
                    }
                    // Catchall for unclosed tags -- if there's an "unfinished" carrier here, it's because  top-level tag was unclosed.
                    // THis will wrap up any straggling text and close any open tags after it.
                    if (!current.Finished)
                    {
                        if (current.Pos > current.HtmlStart)
                        {
                            IDomObject literal = GetLiteral(current);
                            if (literal != null)
                            {
                                yield return literal;
                            }
                        }

                        if (current.Parent != null)
                        {
                            if (current.Parent.Parent == null)
                            {
                                yield return current.Parent.Element;
                            }
                            current.Parent.Reset(current.Pos);
                            current.Finished = true;
                        }
                       
                    }
                }
                pos = current.Pos;
            }

        }
        /// <summary>
        /// Returns a literal object for the text between HtmlStart (the last position of the end of a tag) and the current position.
        /// If !AllowLiterals then it's wrapped in a span.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        protected IDomObject GetLiteral(IterationData current)
        {
            // There's plain text -return it as a literal.
            
            IDomObject textObj;
            DomText lit;
            if (current.Invalid) {
                lit = new DomInvalidElement();
            }
            else if (current.ReadTextOnly)
            {
                current.ReadTextOnly = false;
                lit = new DomInnerText();
            } else {
                lit = new DomText();
            }
            
            if (isBound)
            {
                lit.SetTextIndex(Document, Document.TokenizeString(current.HtmlStart, current.Pos - current.HtmlStart));
            }
            else
            {
                string text = BaseHtml.SubstringBetween(current.HtmlStart, current.Pos);
                lit.NodeValue = Objects.HtmlDecode(text);
            }
             
            if (!current.AllowLiterals)
            {
                IDomElement wrapper = new DomElement("span");
                wrapper.AppendChild(lit);
                textObj = wrapper;
            }
            else
            {
                textObj = lit;
            }

            if (current.Parent != null)
            {
                current.Parent.Element.AppendChild(textObj);
                current.Reset();
                return null;
            }
            else
            {
                current.Finished = true;
                return textObj;
            }
        }
        /// <summary>
        /// Move pointer to the first character after the closing caret of this tag. 
        /// </summary>
        /// <returns>
        /// Returns True if there are children
        /// </returns>
        protected bool MoveOutsideTag(IterationData current)
        {
            int endPos = CharIndexOf(BaseHtml, '>', current.Pos);

            current.HtmlStart = current.Pos + 1;
            if (endPos > 0)
            {
                current.Pos = endPos + 1;
                return BaseHtml[endPos - 1] == '/' ? false :
                    current.Object.InnerHtmlAllowed || current.Object.InnerTextAllowed;

            }
            else
            {
                current.Pos = EndPos + 1;
                return false;
            }

        }
        /// <summary>
        /// Start: Expects the position to be after an opening caret for a close tag, and returns the tag name.
        /// End: Position after closing caret
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        protected string GetCloseTag(IterationData current)
        {
            bool finished = false;
            int step = 0;
            int nameStart = 0;
            string name=null;
            char c;
            while (!finished && current.Pos <= EndPos)
            {
                c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0:
                        if (CharacterData.IsType(c,CharacterType.HtmlTagNameStart))
                        {
                            nameStart = current.Pos;
                            step = 1;
                        }
                        current.Pos++;
                        break;
                    case 1:
                        if (!CharacterData.IsType(c, CharacterType.HtmlTagNameExceptStart))
                        {
                            name = BaseHtml.SubstringBetween(nameStart, current.Pos);
                            step = 2;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 2:
                        if (c == '>')
                        {
                            finished = true;
                        }
                        current.Pos++;
                        break;
                }
            }
            return name ?? "";
        }
        /// <summary>
        /// Start: Position inside a tag opening construct
        /// End: position after last character of tag construct {x=["|']y["|]]} or just {x}) and adds attribute if successful
        ///      position ON closing caret of tag opener if failed
        /// </summary>
        /// <param name="current"></param>
        /// <returns>
        /// Returns true if an attribute was added, false if no more attributes were found
        /// </returns>
        protected bool GetTagAttribute(IterationData current)
        {
            bool finished = false;
            int step = 0;
            string aName = null;
            string aValue = null;
            int nameStart = -1;
            int valStart = -1;
            bool isQuoted = false;
            char quoteChar = ' ';

            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0: // find name
                        if (CharacterData.IsType(c, CharacterType.HtmlTagNameStart))
                        {
                            step = 1;
                            nameStart = current.Pos;
                            current.Pos++;
                        }
                        else if (isTagChar(c))
                        {
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }

                        break;
                    case 1:
                        if (!CharacterData.IsType(c, CharacterType.HtmlTagNameExceptStart))
                        {
                            step = 2;
                            aName = BaseHtml.SubstringBetween(nameStart, current.Pos);
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 2: // find value
                        switch(c) {
                            case '=':
                                step = 3;
                                current.Pos++;
                                break;
                            case ' ':
                                current.Pos++;
                                break;
                            default:
                                // anything else means new attribute
                                finished = true;                               
                                break;
                        }
                        break;
                    case 3: // find quote start
                        switch(c) {
                            case '\\':
                            case '>':
                                finished = true;
                                break;
                            case ' ':
                                current.Pos++;
                                break;
                            case '"':
                            case '\'':
                                isQuoted = true;
                                valStart = current.Pos+1;
                                current.Pos++;
                                quoteChar = c;
                                step = 4;
                                break;
                            default:
                                valStart = current.Pos;
                                step = 4;
                                break;
                        }                        
                        // any non-whitespace is part of the attribute   
                        
                        break;
                    case 4: // parse the attribute until whitespace or closing quote
                        if ((isQuoted && c == quoteChar) || 
                            (!isQuoted && isHtmlTagEnd(c)))
                        {
                            aValue = BaseHtml.SubstringBetween(valStart, current.Pos);
                            if (isQuoted)
                            {
                                isQuoted = false;
                                current.Pos++;
                            }
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                }
            }
            if (aName != null)
            {
                // 12-15-11 - don't replace a valid attribute with a bad one

                var curVal = current.Element.GetAttribute(aName);
                if (string.IsNullOrEmpty(curVal))
                {
                    if (aValue == null)
                    {
                        current.Element.SetAttribute(aName);
                    }
                    else
                    {
                        current.Element.SetAttribute(aName, aValue);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        protected string GetOpenText(IterationData current)
        {
            int pos = CharIndexOf(BaseHtml, '<', current.Pos);
            if (pos > current.Pos)
            {
                int startPos = current.Pos;
                current.Pos = pos;
                return BaseHtml.SubstringBetween(startPos, pos);
            }
            else if (pos == -1)
            {
                int oldPos = current.Pos;
                current.Pos = BaseHtml.Length;
                return BaseHtml.SubstringBetween(oldPos, current.Pos);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Start: the opening caret of a tag
        /// End: the first stop character (e.g. space after the tag name)
        /// </summary>
        /// <param name="current"></param>
        /// <returns>Tag name</returns>
        protected string GetTagOpener(IterationData current)
        {
            bool finished = false;
            int step = 0;
            int tagStart = -1;

            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0:
                                                        
                        if (c == '<')
                        {
                            tagStart = current.Pos + 1;
                            step = 1;
                        }
                        current.Pos++;
                        break;
                    case 1:
                        // skip whitespace between opening caret and text -- probably not allowed but can't hurt to do this
                        if (c == ' ')
                        {
                            current.Pos++;
                        }
                        else
                        {
                            step = 2;
                        }
                        break;
                    case 2:
                        if (isHtmlTagEnd(c))
                        {
                            return BaseHtml.SubstringBetween(tagStart, current.Pos).Trim();
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                }
            }
            return String.Empty;
        }

        protected bool isHtmlTagEnd(char c)
        {
            return c == '/' || c == ' ' || c == '>';
        }
        protected bool isTagChar(char c)
        {
           // return CharacterData.IsType(c, CharacterType.HtmlTagAny);
            return c == '<' || c == '>' || c == '/';
        }

        /* Some tags have inner HTML but are often not closed properly. There are two possible situations. A tag may not 
           have a nested instance of itself, and therefore any recurrence of that tag implies the previous one is closed. 
           Other tag closings are simply optional, but are not repeater tags (e.g. body, html). These should be handled
           automatically by the logic that bubbles any closing tag to its parent if it doesn't match the current tag. The 
           exception is <head> which technically does not require a close, but we would not expect to find another close tag
           Complete list of optional closing tags: -</HTML>- </HEAD> -</BODY> -</P> -</DT> -</DD> -</LI> -</OPTION> -</THEAD> 
           </TH> </TBODY> </TR> </TD> </TFOOT> </COLGROUP>

           body, html will be closed by the document end and are also not required
          
        */

        protected bool TagHasImplicitClose(string tag, string newTag)
        {
            switch (tag)
            {
                case "li":
                case "option":
                case "p":
                case "tr":
                case "td":
                case "th":

                    // simple case: repeater-like tags should be closed by another occurence of itself
                    return tag == newTag;
                case "head":
                    return (newTag == "body");
                case "dt":
                    return tag == newTag || newTag == "dd";
                case "colgroup":
                    return tag == newTag || newTag == "tr";
                default:
                    return false;

            }
        }
        int pos;
        int end;
        protected int CharIndexOf(char[] charArray, char seek, int start)
        {
            //return Array.IndexOf<char>(charArray, seek, start);


            pos = start - 1;
            end = charArray.Length;
            while (++pos < end && charArray[pos] != seek)
                ;
            return pos == end ? -1 : pos;

            // This is substantially faster than Array.IndexOf, cut down load time by about 10% on text heavy dom test

            //int pos = start;
            //int end = charArray.Length;
            //while (pos < end && charArray[pos++] != seek)
            //    ;
            // return pos == end && charArray[end-1] != seek ? -1 : pos-1;
        
        }
    }
}
