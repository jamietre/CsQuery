using System;
using System.IO;
using System.Collections.Generic;
using HtmlParserSharp.Core;
using HtmlParserSharp.Common;
using CsQuery;
using CsQuery.Implementation;
using CsQuery.Utility;
using CsQuery.StringScanner;
using CsQuery.HtmlParser;
using HtmlParserSharp;

namespace CsQuery.HtmlParser
{
    /// <summary>
    /// This is a simple API for the parsing process.
    /// Part of this is a port of the nu.validator.htmlparser.io.Driver class.
    /// The parser currently ignores the encoding in the html source and parses everything as UTF-8.
    /// </summary>
    /// 
    public class ElementFactory
    {
        #region constructors

        static ElementFactory()
        {
            ConfigureDefaultContextMap();
        }


        #endregion

        #region static methods

        public static IDomDocument Create(string html, HtmlParsingMode mode=HtmlParsingMode.Content) {
            using (var reader = new StringReader(html ?? "")) {
                return Parser.Parse(reader,mode);
            }
        }

        public static IDomDocument Create(Stream html, HtmlParsingMode mode = HtmlParsingMode.Content)
        {
            using (var reader = new StreamReader(html))
            {
                return Parser.Parse(reader, mode);
            }
        }

        private static ElementFactory Parser
        {
            get
            {
                return new ElementFactory();
            }
        }

        #endregion

        #region private properties

        private static IDictionary<string, string> DefaultContext;
        private Tokenizer tokenizer;
        private DomTreeBuilder treeBuilder;
        private HtmlParsingMode HtmlParsingMode;

        #endregion

        #region public methods

        public IDomDocument Parse(TextReader reader, HtmlParsingMode mode=HtmlParsingMode.Auto)
        {
            if (reader.Peek() < 0)
            {
                return new DomFragment();
            }

            HtmlParsingMode = mode;
            Reset();
            Tokenize(reader);

            return treeBuilder.Document;
        }

        #endregion

        #region private methods

        private void ConfigureTreeBuilderForParsingMode()
        {
            
            switch (HtmlParsingMode)
            {

                case HtmlParsingMode.Document:
                    treeBuilder.DoctypeExpectation = DoctypeExpectation.Auto;
                    break;
                case HtmlParsingMode.Content:
                    treeBuilder.SetFragmentContext("body");
                    treeBuilder.DoctypeExpectation = DoctypeExpectation.Html;
                    break;
                case HtmlParsingMode.Fragment:
                    treeBuilder.DoctypeExpectation = DoctypeExpectation.Html;
                    HtmlParsingMode = HtmlParsingMode.Auto;
                    break;
            }
        }

        private static void SetDefaultContext(string tags, string context)
        {
            var tagList = tags.Split(',');
            foreach (var tag in tagList)
            {
                DefaultContext[tag.Trim()] = context;
            }
        }

        /// <summary>
        /// Gets a default context for a tag
        /// </summary>
        ///
        /// <param name="tag">
        /// The tag.
        /// </param>
        ///
        /// <returns>
        /// The context.
        /// </returns>

        private string GetContext(string tag)
        {
            string context;
            if (DefaultContext.TryGetValue(tag, out context))
            {
                return context;
            }
            else
            {
                return "body";
            }
        }

        /// <summary>
        /// Gets a context by inspecting the beginning of a character array
        /// </summary>
        ///
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        ///
        /// <returns>
        /// The context.
        /// </returns>

        private string GetContext(char[] buffer)
        {
            int len = buffer.Length;
            int pos = 0;
            string tag = "";
            int mode=0;
            while (pos < len)
            {
                char cur = buffer[pos];
                switch(mode) {
                    case 0:
                        if (cur=='<') {
                            mode=1;
                        }
                        break;
                    case 1:
                        if (CharacterData.IsType(cur, CharacterType.HtmlTagOpenerEnd))
                        {
                            pos=len;
                            break;
                        }
                        tag += cur;
                        break;
                }
                pos++;
            }
            return GetContext(tag);
        }
        private void InitializeTreeBuilder()
        {
            treeBuilder = new DomTreeBuilder();

            treeBuilder.NamePolicy = XmlViolationPolicy.Allow;
            treeBuilder.IsIgnoringComments = false;
            
            // DocTypeExpectation should be set later depending on fragment/content/document selection


        }
        private void Reset()
        {
            InitializeTreeBuilder();

            tokenizer = new Tokenizer(treeBuilder, false);
            
            

            // optionally: report errors and more

            //treeBuilder.ErrorEvent +=
            //    (sender, a) =>
            //    {
            //        ILocator loc = tokenizer as ILocator;
            //        Console.WriteLine("{0}: {1} (Line: {2})", a.IsWarning ? "Warning" : "Error", a.Message, loc.LineNumber);
            //    };
            //treeBuilder.DocumentModeDetected += (sender, a) => Console.WriteLine("Document mode: " + a.Mode.ToString());
            //tokenizer.EncodingDeclared += (sender, a) => Console.WriteLine("Encoding: " + a.Encoding + " (currently ignored)");
        }

        private void Tokenize(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader was null.");
            }

            if (HtmlParsingMode != HtmlParsingMode.Auto)
            {
                ConfigureTreeBuilderForParsingMode();
                tokenizer.Start();
            }

            bool swallowBom = true;


            try
            {
                char[] buffer = new char[2048];
                UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
                bool lastWasCR = false;
                int len = -1;
                if ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    if (HtmlParsingMode == HtmlParser.HtmlParsingMode.Auto)
                    {
                        string ctx = GetContext(buffer);
                        switch (ctx)
                        {
                            case "*document":
                                HtmlParsingMode = HtmlParsingMode.Document;
                                break;
                            case "*content":
                                HtmlParsingMode = HtmlParsingMode.Content;
                                break;
                            default:
                                HtmlParsingMode = HtmlParsingMode.Fragment;
                                treeBuilder.SetFragmentContext(ctx);
                                break;
                        }
                        ConfigureTreeBuilderForParsingMode();
                        tokenizer.Start();
                    }

                    int streamOffset = 0;
                    int offset = 0;
                    int length = len;
                    if (swallowBom)
                    {
                        if (buffer[0] == '\uFEFF')
                        {
                            streamOffset = -1;
                            offset = 1;
                            length--;
                        }
                    }
                    if (length > 0)
                    {
                        tokenizer.SetTransitionBaseOffset(streamOffset);
                        bufr.Start = offset;
                        bufr.End = offset + length;
                        while (bufr.HasMore)
                        {
                            bufr.Adjust(lastWasCR);
                            lastWasCR = false;
                            if (bufr.HasMore)
                            {
                                lastWasCR = tokenizer.TokenizeBuffer(bufr);
                            }
                        }
                    }
                    streamOffset = length;
                    while ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        tokenizer.SetTransitionBaseOffset(streamOffset);
                        bufr.Start = 0;
                        bufr.End = len;
                        while (bufr.HasMore)
                        {
                            bufr.Adjust(lastWasCR);
                            lastWasCR = false;
                            if (bufr.HasMore)
                            {
                                lastWasCR = tokenizer.TokenizeBuffer(bufr);
                            }
                        }
                        streamOffset += len;
                    }
                }
                tokenizer.Eof();
            }
            finally
            {
                tokenizer.End();
            }
        }

        /// <summary>
        /// Configure default context: creates a default context for arbitrary fragments so they are valid no matter what, 
        /// so that true fragments can be created without concern for the context
        /// </summary>

        private static void ConfigureDefaultContextMap()
        {
            DefaultContext = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            SetDefaultContext("tbody,thead,tfoot,colgroup,caption", "table");
            SetDefaultContext("col", "colgroup");
            SetDefaultContext("tr", "tbody");
            SetDefaultContext("td,th", "tr");

            SetDefaultContext("option,optgroup", "select");

            SetDefaultContext("dt,dd", "dl");
            SetDefaultContext("li", "ol");

            SetDefaultContext("meta", "head");
            SetDefaultContext("title", "head");
            SetDefaultContext("head", "html");

            // pass these through; they will dictate high-level parsing mode
            
            SetDefaultContext("html", "*document");
            SetDefaultContext("!doctype", "*document");
            SetDefaultContext("body", "*content");

        }
        #endregion

    }
}
