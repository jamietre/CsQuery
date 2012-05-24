using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using CsQuery.Utility.StringScanner;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.OutputFormatters
{
    /// <summary>
    /// Removes all extraneous whitespace
    /// </summary>
    public class FormatPlainText: IOutputFormatter
    {
        protected IStringInfo stringInfo;
        protected bool endingBlock = false;
        protected bool skipWhitespace = false;

        public string Format(CQ selection)
        {
            stringInfo = CharacterData.CreateStringInfo();

            StringBuilder sb = new StringBuilder();
            foreach (IDomObject obj in selection) {
                AddContents(sb,obj);
            }
            return sb.ToString();
        }

        protected void AddContents(StringBuilder sb, IDomObject startEl)
        {

            if (startEl.HasChildren)
            {
                foreach (IDomObject el in startEl.ChildNodes)
                {
                    if (el.NodeType == NodeType.TEXT_NODE)
                    {
                        IDomText txtNode  = (IDomText)el;
                        stringInfo.Target = el.NodeValue;
                        if (!stringInfo.Whitespace) {
                            string val = txtNode.NodeValue;
                            if (skipWhitespace)
                            {
                                val = CleanFragment(val);
                            }
                            // always add if there's actually content
                            endingBlock = false;
                            skipWhitespace = false;
                            sb.Append(val);
                        } else {
                            // just whitespace
                            if (!skipWhitespace)
                            {
                                // if not an ending block convert all whitespace to a single space, and
                                // act like it was an ending block (preventing further whitespace from being added)
                                sb.Append(" ");
                                skipWhitespace = true;
                            }
                        }
                    }
                    else if (el.NodeType == NodeType.ELEMENT_NODE)
                    {
                        IDomElement elNode = (IDomElement)el;
                        // first add any inner contents
                        if (el.NodeName != "head" && el.NodeName != "style" && el.NodeName != "script")
                        {
                            AddContents(sb, el);

                            switch (elNode.NodeName)
                            {
                                case "br":
                                    sb.Append(System.Environment.NewLine);
                                    skipWhitespace = true;
                                    break;
                                case "pre":
                                    sb.Append(el.Render());
                                    break;
                                case "a":
                                    sb.Append(el.Cq().Children().RenderSelection() + " (" + el["href"] + ")");
                                    break;
                                default:
                                    if (elNode.IsBlock)
                                    {
                                        if (!endingBlock)
                                        {
                                            // erase ending whitespace -- scan backwards until non-whitespace
                                            int i = sb.Length - 1;
                                            int count = 0;
                                            while (i >= 0 && CharacterData.IsType(sb[i], CharacterType.Whitespace))
                                            {
                                                i--;
                                                count++;
                                            }
                                            if (i < sb.Length - 1)
                                            {
                                                sb.Remove(i + 1, count);
                                            }

                                            endingBlock = true;
                                            skipWhitespace = true;
                                            sb.Append(System.Environment.NewLine + System.Environment.NewLine);
                                        }

                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private string CleanFragment(string text)
        {
            var charInfo = CharacterData.CreateCharacterInfo();

            StringBuilder sb = new StringBuilder();
            int index=0;
            bool trimmed=false;
            while (index<text.Length) {
                charInfo.Target = text[index];
                if (!trimmed && !charInfo.Whitespace){
                    trimmed = true;
                }
                if (trimmed) {
                    if (charInfo.Whitespace)
                    {
                        // convert all whitespace blocks into a single space
                        sb.Append(" ");
                        trimmed = false;
                    }
                    else
                    {
                        sb.Append(text[index]);
                    }
                }
                index++;
            }

            return sb.ToString();
        }
    }
}
