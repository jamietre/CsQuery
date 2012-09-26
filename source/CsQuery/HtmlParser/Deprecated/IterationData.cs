using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Implementation;
using CsQuery.StringScanner;

namespace CsQuery.HtmlParser.Deprecated
{
    /// <summary>
    /// A class encapsulating the state of an HTML parser element.
    /// </summary>

    public class IterationData
    {
        /// <summary>
        /// When true, literals that are not the child of another element should be wrapped in a
        /// "span" block.
        /// </summary>

        public bool WrapLiterals;

        /// <summary>
        /// The insertion mode active at this time.
        /// </summary>

        public InsertionMode InsertionMode;

        /// <summary>
        /// State of the tokenizer.
        /// </summary>

        public TokenizerState TokenizerState;

        /// <summary>
        /// The parent object to this object.
        /// </summary>

        public IterationData Parent;

        /// <summary>
        /// The DomObject that this IterationData is creating.
        /// </summary>

        public IDomObject Element;

        /// <summary>
        /// The current placeholder/pointer position.
        /// </summary>

        public int Pos;

        /// <summary>
        /// The starting position of content (inner HTML) for this element. When a non-text character is
        /// found, the content from this position until the curren pointer position will be mapped to a
        /// text node.
        /// </summary>

        public int HtmlStart;

        /// <summary>
        /// Use this to prepare the iterator object to continue finding siblings. It retains the parent.
        /// It just avoids having to recreate an instance of this object for the next tag.
        /// </summary>

        public void Reset()
        {
            TokenizerState = TokenizerState.Default;
            HtmlStart = Pos;
            InsertionMode = InsertionMode.Default;
            Element = null;
        }

        /// <summary>
        /// Use this to prepare the iterator object to continue finding siblings. It retains the parent.
        /// It just avoids having to recreate an instance of this object for the next tag.
        /// </summary>
        ///
        /// <param name="pos">
        /// The index position to which to reset the pointer.
        /// </param>

        public void Reset(int pos)
        {
            Pos = pos;
            Reset();
        }

        /// <summary>
        /// Read content from the current position as text only (if InsertionMode.Text)
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>

        public void ReadText(char[] html)
        {
            // deal with when we're in a literal block (script/textarea)
            
            if (InsertionMode==InsertionMode.Text)
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
        /// Advance the pointer to the next caret, or past the end if none is found.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public bool FindNextTag(char[] html)
        {
            Pos = html.CharIndexOf('<', Pos);

            // only consider text to be HTML if a non-whitespace character follows a caret
            if (Pos < 0 || 
                (Pos>0 && 
                    (Pos < html.Length && CharacterData.IsType(html[Pos+1],CharacterType.Whitespace) ||
                    (Pos==html.Length-1))
                ))
            {
                // done - no new tags found

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
        /// Returns a literal object for the text between HtmlStart (the last position of the end of a
        /// tag) and the current position. If !AllowLiterals then it's wrapped in a span.
        /// </summary>
        ///
        /// <param name="factory">
        /// The HTML factory to operate against
        /// </param>
        /// <param name="literal">
        /// [out] The literal.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>

        public bool TryGetLiteral(HtmlElementFactory factory, out IDomObject literal)
        {


            if (Pos <= HtmlStart)
            {
                literal = null;
                return false;
            }

            // There's plain text -return it as a literal.
            
            DomText lit;
            switch(InsertionMode) {
                case InsertionMode.Invalid:
                    lit = new DomInvalidElement();
                    break;
                case InsertionMode.Text:
                    InsertionMode =InsertionMode.Default;
                    lit = new DomInnerText();
                    break;
                default:
                    lit = new DomText();
                    break;
            }
            literal = lit;

            //if (factory.IsBound)
            //{
            //    lit.SetTextIndex(factory.Document, factory.Document.DocumentIndex.TokenizeString(HtmlStart, Pos - HtmlStart));
            //}
            //else
            //{
                string text = factory.Html.SubstringBetween(HtmlStart, Pos);
                literal.NodeValue = HtmlData.HtmlDecode(text);
            //}

            if (WrapLiterals)
            {
                DomElement wrapper = DomElement.Create("span");
                wrapper.AppendChildUnsafe(literal);
                literal = wrapper;
            }
        

            if (Parent != null)
            {
                ((DomElement)Parent.Element).AppendChildUnsafe(literal);
                Reset();
                return false;
            }
            else
            {
                TokenizerState = TokenizerState.Finished;
                return true;
            }
        }

        /// <summary>
        /// Close out this element. This method will return true if something can be yielded; this this
        /// means it's got a parent at the top of the heirarchy. Otherwise it's just closed but false is
        /// returned.
        /// </summary>
        ///
        /// <param name="factory">
        /// The HTML factory to operate against.
        /// </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process close element in this collection.
        /// </returns>

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
                TokenizerState = TokenizerState.Finished;
            }
        }

        /// <summary>
        /// Call when attribute parsing is finished; either adds this to the parent, or yields it if
        /// complete &amp; no parent. The return value indicates whether there are any children.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML
        /// </param>
        /// <param name="completeElement">
        /// [out] The complete element.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>


        public bool FinishTagOpener(char[] html,out IDomObject completeElement)
        {

            bool hasChildren = MoveOutsideTag(html);

            // tricky part: if there are children, push ourselves back on the stack and start with a new
            // object from this position. The children will add themselves as they are created, avoiding
            // recursion. When the close tag is found, the parent will be yielded if it's a root element. I
            // think there's a slightly better way to do this, capturing all the yield logic at the end of
            // the stack but it works. 

            if (Parent != null)
            {
                ((DomElement)Parent.Element).AppendChildUnsafe(Element);
                completeElement = null;
            }
            else if (!hasChildren)
            {
                completeElement = Element;
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
        /// Returns the tag name. The placeholder should be on the opening caret of a tag, and upon exit
        /// will be on the first stop character (e.g. space after the tag name)
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML
        /// </param>
        ///
        /// <returns>
        /// The new tag name.
        /// </returns>

        public string GetTagOpener(char[] html)
        {
            bool finished = false;
            int step = 0;
            int tagStart = -1;
            int len = html.Length;

            while (!finished && Pos < len)
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
                        // skip a space between opening caret and text
                        // this is not part of the spec but it seems reaonsable to be lax about this.
                        // TODO: verify browser behavior

                        if (c == ' ')
                        {
                            return string.Empty;
                        }
                        else
                        {
                            step = 2;
                        }
                        break;
                    case 2:
                        if (CharacterData.IsType(c, CharacterType.HtmlTagOpenerEnd))
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
        /// Return the current closing tag. Expects the placeholder to be _after_ an opening caret for a
        /// close tag, and returns the tag name. Upon exit, the placeholder is at the next position after
        /// the closing caret.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// The close tag.
        /// </returns>

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

        /// <summary>
        /// Returns all text from the current position up to but not including the next opening caret.
        /// The placeholder will be on the opening caret.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// The open text, or null if the placeholder is already at a caret.
        /// </returns>

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
        /// Parse an HTML attribute construct: {x=["|']y["|']]} or just {x}) and add the attribute to the
        /// current element if successful. The placeholder should be tag opening construct but after the
        /// tag name. Upon exit it will be positioned after last character of attribute, or  positioned
        /// ON closing caret of tag opener if failed.
        /// </summary>
        ///
        /// <remarks>
        /// An unquoted attribute value is specified by providing the following parts in exactly the
        /// following order:
        /// 
        /// - an attribute name
        /// - zero or more space characters
        /// - a single "=" character
        /// - zero or more space characters
        /// - an attribute value
        /// 
        /// Attribute value for unquoted:
        /// 
        /// - must not contain any literal space characters
        /// - must not contain any """, "'", "=", "&gt;", "&lt;", or "`", characters
        /// - must not be the empty string
        /// 
        /// In practice: Chrome allows all these characters embedded inside an unquoted value except &gt;
        /// 
        /// http://www.w3.org/TR/html-markup/syntax.html#attr-value-unquoted.
        /// </remarks>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
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
                        if (CharacterData.IsType(c, CharacterType.HtmlSpace))
                        {
                            Pos++;
                            break;
                        }
                        else if (c == '=')
                        {
                            step = 3;
                            Pos++;
                        } else {
                            // anything else means new attribute
                            finished = true;
                            break;
                        }
                        break;
                    case 3: // find quote start
                        if (CharacterData.IsType(c, CharacterType.HtmlSpace))
                        {
                            Pos++;
                            break;
                        }
                        else
                        {
                            switch (c)
                            {
                                case '>':
                                    finished = true;
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
                        }

                        break;
                    case 4: // parse the attribute until whitespace or closing quote
                        if ((isQuoted && c == quoteChar) ||
                            (!isQuoted && CharacterData.IsType(c, CharacterType.HtmlAttributeValueTerminator)))
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

                if (!Element.HasAttribute(aName))
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
        /// Add a new parent of type tagId. In other words, wrap the current element in a new element.
        /// </summary>
        ///
        /// <param name="tagId">
        /// The token for the tag to add
        /// </param>
        /// <param name="pos">
        /// The index position for the iteration data
        /// </param>
        ///
        /// <returns>
        /// The iteration data representing the current element (replaces the
        /// </returns>

        public IterationData AddNewParent(ushort tagId, int pos)
        {
            Element = DomElement.Create(tagId);
            ((DomElement)Parent.Element).AppendChildUnsafe(Element);
            return AddNewChild(pos);
        }

        /// <summary>
        /// Adds a new child to this item, and returns it.
        /// </summary>
        ///
        /// <returns>
        /// New IterationData that is a child of the current IterationData.
        /// </returns>

        public IterationData AddNewChild()
        {
            return AddNewChild(Pos);

        }

        /// <summary>
        /// Adds a new child to this item, and returns it
        /// </summary>
        ///
        /// <param name="pos">
        /// The index position for the iteration data.
        /// </param>
        ///
        /// <returns>
        /// New IterationData that is a child of the current IterationData
        /// </returns>

        public IterationData AddNewChild(int pos)
        {
            IterationData subItem = new IterationData
            {
                Parent = this,
                Pos = pos,
                HtmlStart = Pos,
                InsertionMode = InsertionMode
            };
            return subItem;

        }

        /// <summary>
        /// Move pointer to the first character after the closing caret of this tag.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        ///
        /// <returns>
        /// Returns True if there are children.
        /// </returns>

        public bool MoveOutsideTag(char[] html)
        {
            int endPos = html.CharIndexOf('>', Pos);

            HtmlStart = Pos + 1;
            if (endPos > 0)
            {
                Pos = endPos + 1;
                return html[endPos - 1] == '/' ? false :
                    Element.InnerHtmlAllowed || Element.InnerTextAllowed;

            }
            else
            {
                Pos = html.Length;
                return false;
            }

        }

        /// <summary>
        /// Gets the token for the parent node to this node.
        /// </summary>
        ///
        /// <returns>
        /// A ushort of the parent's token, or 0 if there is no parent.
        /// </returns>

        public ushort ParentTagID()
        {
            return Parent != null ?
                Parent.Element.NodeNameID :
                (ushort)0;
        }

    }
}
