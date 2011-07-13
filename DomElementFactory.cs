using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.ExtensionMethods;
using System.Diagnostics;

namespace Jtc.CsQuery
{
    public class DomElementFactory
    {
        protected string BaseHtml
        {
            set
            {
                _BaseHtml = value;
                _EndPos = -1;
            }
            get
            {
                return _BaseHtml;
            }
        } protected string _BaseHtml = null;
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
        /// <summary>
        /// No literals allowed
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IEnumerable<IDomElement> CreateElements(string html)
        {
            foreach (IDomObject obj in CreateObjectsImpl(html, false))
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
            BaseHtml = html;
            return (IDomElement)Parse(false).First();
        }
        /// <summary>
        /// Returns a list of elements created by parsing the string. If allowLiterals is false, any literal text that is not
        /// inside a tag will be wrapped in span tags.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="allowLiterals"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> CreateObjects(string html)
        {
            return CreateObjectsImpl(html, true);
        }
        protected IEnumerable<IDomObject> CreateObjectsImpl(string html, bool allowLiterals)
        {
            BaseHtml = html;
            return Parse(allowLiterals);
        }
        protected class IterationData
        {
            public IterationData Parent;
            public IDomObject Object
            {
                get
                {
                    return _Object;
                }
                set
                {
                    _Object = value;
                }
            } protected IDomObject _Object;
            public IDomElement Element
            {
                get
                {
                    return (IDomElement)Object;
                }
            }
            public int Pos;
            public int Step = 0;
            public bool Finished;
            public bool AllowLiterals;
            public int HtmlStart = 0;
            /// <summary>
            /// Use this to prepare the iterator object to continue finding siblings. It retains the parent.
            /// </summary>
            public void Reset()
            {
                Step = 0;
                HtmlStart = Pos;
                Object = null;
            }
            public void Reset(int pos)
            {
                Pos = pos;
                Reset();
            }
        }
      
        protected IEnumerable<IDomObject> Parse(bool allowLiterals)
        {
            int pos=0;
            Stack<IterationData> stack = new Stack<IterationData>();

            while (pos < EndPos)
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
                                if (c == '<')
                                {
                                    // found a tag-- it could be a close tag, or a new HTML tag
                                    current.Step = 1;
                                }
                                else
                                {
                                    current.Pos++;
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
                                string newTag = GetTagOpener(current);

                                // when Element exists, it's because a previous iteration created it: it's our parent
                                string parentTag = String.Empty;
                                if (current.Parent != null)
                                {
                                    parentTag = current.Parent.Element.Tag.ToLower();
                                }

                                if (newTag == String.Empty)
                                {
                                    // It's a tag closer. Make sure it's the right one.
                                    current.Pos = tagStartPos + 1;
                                    //Debug.Assert(curPos != 1504);
                                    string closeTag = GetCloseTag(current);
                                    if (closeTag == String.Empty)
                                    {
                                        // ignore empty tags
                                        continue;
                                    } else if (closeTag.ToLower() != parentTag)
                                    {
                                        // it wasn't ours. Assume it's something above us (so go back to previous position) and stop here.
                                        current.Pos = tagStartPos;
                                    }
                                    if (current.Parent.Parent == null)
                                    {
                                        yield return current.Parent.Element;
                                    }
                                    current.Parent.Reset(current.Pos);
                                    current.Finished = true;
                                    // already been returned before we added the children
                                    continue;
                                }

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

                                
                                ISpecialElement specialElement = null;
                                if (newTag.ToLower()=="!doctype")
                                {
                                    specialElement = new DomSpecialElement(NodeType.DOCUMENT_TYPE_NODE);
                                    current.Object = specialElement;
                                } else if (newTag[0] == '!')
                                {
                                    specialElement = new DomComment();
                                    current.Object = specialElement;
                                }
                                else {
                                    current.Object = new DomElement();
                                    current.Element.Tag = newTag;
                                }

                                // Check for informational tag types
                                
                               // Debug.Assert(newTag != "p");
                                if (current.Object is ISpecialElement)
                                {
                                    int tagEndPos = BaseHtml.IndexOf(">", current.Pos);
                                    if (tagEndPos > EndPos)
                                    {
                                        throw new Exception("Unclosed HTML element '" + current.Element.Tag + "'");
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

                                // For some reason I cannot get my head around a way to perform these logical steps with fewer conditional statements
                                // They must be performed in this order.

                                if (current.Parent != null)
                                {
                                    current.Parent.Element.Add(current.Object);
                                } else if (!hasChildren) {
                                    yield return current.Object;
                                }

                                if (!hasChildren)
                                {
                                    current.Reset();
                                }
                                if (!hasChildren)
                                {
                                    continue;
                                }

                                stack.Push(current);
                                //Debug.Assert(current.Object == null);

                                IterationData subItem = new IterationData();
                                
                                subItem.Parent = current;
                                subItem.AllowLiterals = true;
                                subItem.Reset(current.Pos);
                                current = subItem;
                                break;

                        }
                    }
                    // Catchall for unclosed tags -- if there's an "unfinished" carrier here, it's because  top-level tag was unclosed.
                    if (!current.Finished)
                    {

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
                /// Check for any straggling text - typically the case for non-dom-bound data.
                if (!current.Finished && current.Pos > current.HtmlStart)
                {
                    IDomObject literal = GetLiteral(current);
                    if (literal != null)
                    {
                        yield return literal;
                    }
                }

                //yield return current.Element;
                pos = current.Pos;
            }

        }

        protected IDomObject GetLiteral(IterationData current)
        {
            // There's plain text -return it as a literal.
            string text = BaseHtml.SubstringBetween(current.HtmlStart, current.Pos);
            IDomObject textObj = null;
            DomText lit = new DomText(text);
            if (!current.AllowLiterals)
            {
                IDomElement wrapper = new DomElement();
                wrapper.Tag = "span";
                wrapper.Add(lit);
                textObj = wrapper;
            }
            else
            {
                textObj = lit;
            }

            if (current.Parent != null)
            {
                current.Parent.Element.Add(textObj);
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
        /// Move pointer to the first character after the end of this tag. Returns True if there are children.
        /// </summary>
        /// <returns></returns>
        protected bool MoveOutsideTag(IterationData current)
        {
            bool finished = false;
            bool inner = false;
            while (!finished && current.Pos <= EndPos)
            {
                char c = BaseHtml[current.Pos];
                if (c == '>')
                {
                    if (BaseHtml[current.Pos - 1] == '/')
                    {
                        inner = false;
                    }
                    else
                    {
                        inner = current.Object.InnerHtmlAllowed;
                    }
                    finished = true;
                    current.HtmlStart = current.Pos + 1;
                }
                current.Pos++;
            }
            return inner;
        }

        protected string GetCloseTag(IterationData current)
        {
            bool finished = false;
            int step = 0;
            int nameStart = 0;
            string name = String.Empty;
            char c;
            while (!finished && current.Pos <= EndPos)
            {
                c = BaseHtml[current.Pos];
                switch (step)
                {
                    case 0:
                        if (isValidNameChar(c))
                        {
                            nameStart = current.Pos;
                            step = 1;
                        }
                        current.Pos++;
                        break;
                    case 1:
                        if (!isValidNameChar(c))
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
            return name;
        }
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
                        if (isValidNameChar(c))
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
                        if (!isValidNameChar(c))
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
                        if (c == '=')
                        {
                            step = 3;
                            current.Pos++;
                        }
                        else if (c != ' ')
                        {
                            // anything else means new attribute
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 3: // find quote start
                        if (c == '"' || c == '\'')
                        {
                            isQuoted = true;
                            quoteChar = c;
                            step = 4;
                            valStart = current.Pos + 1;
                            current.Pos++;
                        }
                        else if (isValidNameChar(c))
                        {
                            step = 4;
                            valStart = current.Pos;
                            current.Pos++;
                        }
                        else if (c != ' ')
                        {
                            // bad html - no quote
                            finished = true;
                        }
                        else
                        {
                            current.Pos++;
                        }
                        break;
                    case 4: // finished
                        if ((isQuoted && c == quoteChar) || (!isQuoted && !isValidNameChar(c)))
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
                current.Element.SetAttribute(aName, aValue);
                return true;
            }
            else
            {
                return false;
            }
        }
        protected string GetOpenText(IterationData current)
        {
            int pos = BaseHtml.IndexOf('<', current.Pos);
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
                        if (c == '/' || c == ' ' || c == '>')
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

        protected bool isValidNameChar(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c>='0' && c<='9') || c == '_' ;
        }
        protected bool isTagChar(char c)
        {
            return (c == '<' || c == '>' || c == '/');
        }
        // Some tags have inner HTML but are often not closed properly. There are two possible situations. A tag may not have a nested instance of itself, and therefore any
        // recurrence of that tag implies the previous one is closed. Other tag closings are simply optional, but are not repeater tags (e.g. body, html). These should be handled
        // automatically by the logic that bubbles any closing tag to its parent if it doesn't match the current tag. The exception is <head> which technically does not require
        // a close, but we would not expect to find another close tag
        // Complete list of optional closing tags: -</HTML>- </HEAD> -</BODY> -</P> -</DT> -</DD> -</LI> -</OPTION> -</THEAD> </TH> </TBODY> </TR> </TD> </TFOOT> </COLGROUP>

        // body, html don't matter, they will be closed by the document end.
        // 
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
    }
}
