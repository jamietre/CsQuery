using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.StringScanner;

namespace CsQuery.HtmlParser
{
    public class IterationData
    {
        public IterationData Parent;
        public IDomObject Object;
        public DomElement Element
        {
            get
            {
                return (DomElement)Object;
            }
        }
        // when true, the contents will be treated as text until the next close tag
        public bool ReadTextOnly;
        public int Pos;
        public byte Step;
        public bool Finished;
        public bool AllowLiterals;
        public bool Invalid;
        public int HtmlStart;
        
        /// <summary>
        /// Use this to prepare the iterator object to continue finding siblings. It retains the parent. It just avoids having to recreate
        /// an instance of this object for the next tag.
        /// </summary>
        public void Reset()
        {
            Step = 0;
            HtmlStart = Pos;
            ReadTextOnly = false;
            Object = null;
        }
        public void Reset(int pos)
        {
            Pos = pos;
            Reset();
        }

        /// <summary>
        /// Read content from the current position as text only (if ReadTextOnly=true)
        /// </summary>
        public void ReadText(char[] html)
        {
            // deal with when we're in a literal block (script/textarea)
            if (ReadTextOnly)
            {
                int endPos = Pos;
                while (endPos >= 0)
                {
                    // keep going until we find the closing tag for this element
                    int caretPos = html.CharIndexOf('>', endPos + 1);
                    if (caretPos > 0)
                    {
                        string tag = html.SubstringBetween(endPos + 1, caretPos).Trim().ToUpper();
                        if (tag == "/" + Parent.Element.NodeName)
                        {
                            // this is the end tag -- exit the block
                            Pos = endPos;
                            break;
                        }
                    }
                    endPos = html.CharIndexOf('<', endPos + 1);
                }
            }
        }
        /// <summary>
        /// Advance the pointer to the next caret, or past the end if none is found
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public bool FindNextTag(char[] html)
        {
            Pos = html.CharIndexOf('<', Pos);
            if (Pos < 0)
            {
                // done - no new tags found
                //Pos = EndPos + 1;
                Pos = html.Length;
                return false;
            }
            else
            {
                ReadText(html);
                return true;
            }
        }


        /// <summary>
        /// Returns a literal object for the text between HtmlStart (the last position of the end of a tag) and the current position.
        /// If !AllowLiterals then it's wrapped in a span.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool TryGetLiteral(HtmlElementFactory factory, out IDomObject literal)
        {


            if (Pos <= HtmlStart)
            {
                literal = null;
                return false;
            }

            // There's plain text -return it as a literal.
            
            DomText lit;
            if (Invalid)
            {
                lit = new DomInvalidElement();
            }
            else if (ReadTextOnly)
            {
                ReadTextOnly = false;
                lit = new DomInnerText();
            }
            else
            {
                lit = new DomText();
            }
            literal = lit;

            if (factory.IsBound)
            {
                lit.SetTextIndex(factory.Document, factory.Document.TokenizeString(HtmlStart, Pos - HtmlStart));
            }
            else
            {
                string text = factory.Html.SubstringBetween(HtmlStart, Pos);
                literal.NodeValue = HtmlData.HtmlDecode(text);
            }

            if (!AllowLiterals)
            {
                DomElement wrapper = new DomElement("span");
                wrapper.ChildNodes.AddAlways(literal);
                literal = wrapper;
            }
        

            if (Parent != null)
            {
                Parent.Element.ChildNodes.AddAlways(literal);
                Reset();
                return false;
            }
            else
            {
                Finished = true;
                return true;
            }
        }

        /// <summary>
        ///  Close out this element. This method will return true if something can be yielded; this
        ///  this means it's got a parent at the top of the heirarchy. Otherwise it's just closed but 
        ///  false is returned.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<IDomObject> CloseElement(HtmlElementFactory factory)
        {
            IDomObject element = null;

            if (TryGetLiteral(factory, out element))
            {
                yield return element;
            }
                
            if (Parent != null)
            {
                if (Parent.Parent == null)
                {
                    yield return Parent.Element;
                } 
                Parent.Reset(Pos);
                Finished = true;
            }
        }

        /// <summary>
        /// Call when attribute parsing is finished; either adds this to the parent, or yields it if complete & no parent.
        /// The return value indicates whether there are any children.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool FinishTagOpener(char[] html,out IDomObject completeElement)
        {

            bool hasChildren = MoveOutsideTag(html);

            // tricky part: if there are children, push ourselves back on the stack and start with a new object
            // from this position. The children will add themselves as they are created, avoiding recursion.
            // When the close tag is found, the parent will be yielded if it's a root element.
            // I think there's a slightly better way to do this, capturing all the yield logic at the end of the
            // stack but it works for now.

            if (Parent != null)
            {
                Parent.Element.ChildNodes.AddAlways(Object);
                completeElement = null;
            }
            else if (!hasChildren)
            {
                completeElement = Object;
            }
            else
            {
                completeElement = null;
            }

            if (!hasChildren)
            {
                Reset();
            }
            return hasChildren;

        }
        /// <summary>
        /// Start: the opening caret of a tag
        /// End: the first stop character (e.g. space after the tag name)
        /// </summary>
        /// <param name="current"></param>
        /// <returns>Tag name</returns>
        public string GetTagOpener(char[] html)
        {
            bool finished = false;
            int step = 0;
            int tagStart = -1;
            int len = html.Length;

            while (!finished && Pos <= len)
            {
                char c = html[Pos];
                switch (step)
                {
                    case 0:

                        if (c == '<')
                        {
                            tagStart = Pos + 1;
                            step = 1;
                        }
                        Pos++;
                        break;
                    case 1:
                        // skip whitespace between opening caret and text -- probably not allowed but can't hurt to do this
                        if (c == ' ')
                        {
                            Pos++;
                        }
                        else
                        {
                            step = 2;
                        }
                        break;
                    case 2:
                        if (isHtmlTagEnd(c))
                        {
                            return html.SubstringBetween(tagStart, Pos).Trim();
                        }
                        else
                        {
                            Pos++;
                        }
                        break;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Start: Expects the position to be after an opening caret for a close tag, and returns the tag name.
        /// End: Position after closing caret
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public string GetCloseTag(char[] html)
        {
            bool finished = false;
            int step = 0;
            int nameStart = 0;
            string name = null;
            char c;
            int len = html.Length;

            while (!finished && Pos < len)
            {
                c = html[Pos];
                switch (step)
                {
                    case 0:
                        if (CharacterData.IsType(c, CharacterType.HtmlTagNameStart))
                        {
                            nameStart = Pos;
                            step = 1;
                        }
                        Pos++;
                        break;
                    case 1:
                        if (!CharacterData.IsType(c, CharacterType.HtmlTagNameExceptStart))
                        {
                            name = html.SubstringBetween(nameStart, Pos);
                            step = 2;
                        }
                        else
                        {
                            Pos++;
                        }
                        break;
                    case 2:
                        if (c == '>')
                        {
                            finished = true;
                        }
                        Pos++;
                        break;
                }
            }
            return name;
        }

        public string GetOpenText(char[] html)
        {
            int pos = html.CharIndexOf('<', Pos);
            if (pos > Pos)
            {
                int startPos = Pos;
                Pos = pos;
                return html.SubstringBetween(startPos, pos);
            }
            else if (pos == -1)
            {
                int oldPos = Pos;
                Pos = html.Length;
                return html.SubstringBetween(oldPos, Pos);
            }
            else
            {
                return null;
            }
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
        public bool GetTagAttribute(char[] html)
        {
            bool finished = false;
            int step = 0;
            string aName = null;
            string aValue = null;
            int nameStart = -1;
            int valStart = -1;
            bool isQuoted = false;
            char quoteChar = ' ';
            int len = html.Length;

            while (!finished && Pos < len)
            {
                char c = html[Pos];
                switch (step)
                {
                    case 0: // find name
                        if (CharacterData.IsType(c, CharacterType.HtmlTagNameStart))
                        {
                            step = 1;
                            nameStart = Pos;
                            Pos++;
                        }
                        else if (CharacterData.IsType(c, CharacterType.HtmlTagAny))
                        {
                            finished = true;
                        }
                        else
                        {
                            Pos++;
                        }

                        break;
                    case 1:
                        if (!CharacterData.IsType(c, CharacterType.HtmlTagNameExceptStart))
                        {
                            step = 2;
                            aName = html.SubstringBetween(nameStart, Pos);
                        }
                        else
                        {
                            Pos++;
                        }
                        break;
                    case 2: // find value
                        switch (c)
                        {
                            case '=':
                                step = 3;
                                Pos++;
                                break;
                            case ' ':
                                Pos++;
                                break;
                            default:
                                // anything else means new attribute
                                finished = true;
                                break;
                        }
                        break;
                    case 3: // find quote start
                        switch (c)
                        {
                            case '\\':
                            case '>':
                                finished = true;
                                break;
                            case ' ':
                                Pos++;
                                break;
                            case '"':
                            case '\'':
                                isQuoted = true;
                                valStart = Pos + 1;
                                Pos++;
                                quoteChar = c;
                                step = 4;
                                break;
                            default:
                                valStart = Pos;
                                step = 4;
                                break;
                        }
                        // any non-whitespace is part of the attribute   

                        break;
                    case 4: // parse the attribute until whitespace or closing quote
                        if ((isQuoted && c == quoteChar) ||
                            (!isQuoted && isHtmlTagEnd(c)))
                        {
                            aValue = html.SubstringBetween(valStart, Pos);
                            if (isQuoted)
                            {
                                isQuoted = false;
                                Pos++;
                            }
                            finished = true;
                        }
                        else
                        {
                            Pos++;
                        }
                        break;
                }
            }
            if (aName != null)
            {
                // 12-15-11 - don't replace a valid attribute with a bad one

                var curVal = Element.GetAttribute(aName);
                if (string.IsNullOrEmpty(curVal))
                {
                    if (aValue == null)
                    {
                        Element.SetAttribute(aName);
                    }
                    else
                    {
                        Element.SetAttribute(aName, aValue);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Add a new parent of type tagId
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public IterationData AddNewParent(ushort tagId, int pos)
        {

            //ushort generatedTagId = HtmlData.CreateParentFor(tagId);

            // this is largely copied from below, can we eliminate duoplication somehow?

            Object = new DomElement(tagId);
            Parent.Element.ChildNodes.AddAlways(Object);

            return AddNewChild(pos);
        }

        public IterationData AddNewChild()
        {
            return AddNewChild(Pos);

        }

        public IterationData AddNewChild(int pos)
        {
            IterationData subItem = new IterationData
            {
                Parent = this,
                AllowLiterals = true,
                Pos = pos,
                HtmlStart = Pos,
                ReadTextOnly = ReadTextOnly
            };
            return subItem;

        }


        /// <summary>
        /// Move pointer to the first character after the closing caret of this tag. 
        /// </summary>
        /// <returns>
        /// Returns True if there are children
        /// </returns>
        public bool MoveOutsideTag(char[] html)
        {
            int endPos = html.CharIndexOf('>', Pos);

            HtmlStart = Pos + 1;
            if (endPos > 0)
            {
                Pos = endPos + 1;
                return html[endPos - 1] == '/' ? false :
                    Object.InnerHtmlAllowed || Object.InnerTextAllowed;

            }
            else
            {
                Pos = html.Length;
                return false;
            }

        }
        protected bool isHtmlTagEnd(char c)
        {
            //return c == '/' || c == ' ' || c == '>';
            // ~ 2% speed improvement
            return CharacterData.IsType(c, CharacterType.HtmlTagEnd);
        }

        public  ushort ParentTagID()
        {
            return Parent != null ?
                Parent.Element.NodeNameID:
                (ushort)0;
        }

    }
}
