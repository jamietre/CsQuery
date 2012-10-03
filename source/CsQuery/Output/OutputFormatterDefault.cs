using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery;
using CsQuery.StringScanner;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Output
{
    /// <summary>
    /// Removes all extraneous whitespace
    /// </summary>
    public class OutputFormatterDefault: IOutputFormatter
    {
        /// <summary>
        /// Create a new formatter using the default HtmlEncoder
        /// </summary>

        public OutputFormatterDefault()
        {
            HtmlEncoder = new HtmlEncoderDefault();
        }

        /// <summary>
        /// Create a new formatter using the specified HtmlEncoder.
        /// </summary>
        ///
        /// <param name="encoder">
        /// The encoder.
        /// </param>

        public OutputFormatterDefault(IHtmlEncoder encoder)
        {
            HtmlEncoder = encoder;
        }

        protected IHtmlEncoder HtmlEncoder;

        protected IStringInfo stringInfo;
        protected bool endingBlock = false;
        protected bool skipWhitespace = false;

        public void Format(CQ selection,TextWriter writer)
        {
            stringInfo = CharacterData.CreateStringInfo();

            foreach (IDomObject obj in selection) {
                AddContents(writer,obj);
            }
        }

        protected void AddContents(TextWriter writer, IDomObject startEl)
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
                            writer.Write(val);
                        } else {
                            // just whitespace
                            if (!skipWhitespace)
                            {
                                // if not an ending block convert all whitespace to a single space, and
                                // act like it was an ending block (preventing further whitespace from being added)
                                writer.Write(" ");
                                skipWhitespace = true;
                            }
                        }
                    }
                    else if (el.NodeType == NodeType.ELEMENT_NODE)
                    {
                        IDomElement elNode = (IDomElement)el;
                        // first add any inner contents
                        if (el.NodeName != "HEAD" && el.NodeName != "STYLE" && el.NodeName != "SCRIPT")
                        {
                            AddContents(writer, el);

                            switch (elNode.NodeName)
                            {
                                case "BR":
                                    writer.Write(System.Environment.NewLine);
                                    skipWhitespace = true;
                                    break;
                                case "PRE":
                                    writer.Write(el.Render());
                                    break;
                                case "A":
                                    writer.Write(el.Cq().Children().RenderSelection() + " (" + el["href"] + ")");
                                    break;
                                default:
                                    //if (elNode.IsBlock)
                                    //{
                                    //    if (!endingBlock)
                                    //    {
                                    //        // erase ending whitespace -- scan backwards until non-whitespace
                                    //        int i = sb.Length - 1;
                                    //        int count = 0;
                                    //        while (i >= 0 && CharacterData.IsType(sb[i], CharacterType.Whitespace))
                                    //        {
                                    //            i--;
                                    //            count++;
                                    //        }
                                    //        if (i < sb.Length - 1)
                                    //        {
                                    //            sb.Remove(i + 1, count);
                                    //        }

                                    //        endingBlock = true;
                                    //        skipWhitespace = true;
                                    //        sb.Append(System.Environment.NewLine + System.Environment.NewLine);
                                    //    }

                                    //}
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
