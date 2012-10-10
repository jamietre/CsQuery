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
    /// Element factory to build a CsQuery DOM using HtmlParserSharp.
    /// </summary>

    public class ElementFactory
    {
        #region constructors

        /// <summary>
        /// Static constructor.
        /// </summary>

        static ElementFactory()
        {
            ConfigureDefaultContextMap();
        }


        #endregion

        #region static methods

        /// <summary>
        /// Creates a new document from a string of HTML using the options passed.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        /// <param name="parsingMode">
        /// (optional) the parsing mode.
        /// </param>
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>
        /// <param name="docType">
        /// (optional) type of the document.
        /// </param>
        ///
        /// <returns>
        /// A new document.
        /// </returns>

        public static IDomDocument Create(string html, 
            HtmlParsingMode parsingMode=HtmlParsingMode.Auto, 
            HtmlParsingOptions options= HtmlParsingOptions.Default,
            DocType docType = DocType.Default)
        {

            using (var reader = new StringReader(html ?? "")) {
                return GetNewParser(parsingMode,options,docType).Parse(reader);
            }
        }

        /// <summary>
        /// Creates a new document from a Stream of HTML using the options passed.
        /// </summary>
        ///
        /// <param name="html">
        /// The HTML.
        /// </param>
        /// <param name="parsingMode">
        /// (optional) the parsing mode.
        /// </param>
        /// <param name="parsingOptions">
        /// (optional) options for controlling the parsing.
        /// </param>
        /// <param name="docType">
        /// (optional) type of the document.
        /// </param>
        ///
        /// <returns>
        /// A new document.
        /// </returns>

        public static IDomDocument Create(Stream html, 
            HtmlParsingMode parsingMode = HtmlParsingMode.Auto,
            HtmlParsingOptions parsingOptions = HtmlParsingOptions.Default,
            DocType docType = DocType.Default)
        {
            
            using (var reader = new StreamReader(html))
            {
                return GetNewParser(parsingMode, parsingOptions, docType).Parse(reader);
            }
        }

        private static ElementFactory GetNewParser()
        {
            return new ElementFactory();
        }
        private static ElementFactory GetNewParser(HtmlParsingMode parsingMode, HtmlParsingOptions parsingOptions, DocType docType)
        {
            var parser = new ElementFactory();
            parser.HtmlParsingMode = parsingMode;
            parser.DocType = GetDocType(docType);
            parser.HtmlParsingOptions = MergeOptions(parsingOptions);
            return parser;
       }


        #endregion

        #region private properties

        private static IDictionary<string, string> DefaultContext;
        private Tokenizer tokenizer;
        private CsQueryTreeBuilder treeBuilder;

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the HTML parsing mode.
        /// </summary>

        public HtmlParsingMode HtmlParsingMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the HTML parsing mode.
        /// </summary>

        public HtmlParsingOptions HtmlParsingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of the document.
        /// </summary>

        public DocType DocType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a context for the fragment, e.g. a tag name
        /// </summary>

        public string FragmentContext
        {
            get;
            set;
        }
        #endregion

        #region public methods

        /// <summary>
        /// Given a TextReader, create a new IDomDocument from the input.
        /// </summary>
        ///
        /// <param name="reader">
        /// The HTML input.
        /// </param>
        ///
        /// <returns>
        /// A populated IDomDocument.
        /// </returns>

        public IDomDocument Parse(TextReader reader)
        {
            if (reader.Peek() < 0)
            {
                return new DomFragment();
            }

            TextReader source = reader;

            if (HtmlParsingMode == HtmlParsingMode.Auto || 
                ((HtmlParsingMode == HtmlParsingMode.Fragment )
                    && String.IsNullOrEmpty(FragmentContext)))
            {

                string ctx;
                source= GetContextFromStream(source, out ctx);

                if (HtmlParsingMode == HtmlParsingMode.Auto)
                {
                    switch (ctx)
                    {
                        case "document":
                            HtmlParsingMode = HtmlParsingMode.Document;
                            ctx = "";
                            break;
                        case "html":
                            HtmlParsingMode = HtmlParsingMode.Content;
                            break;
                        default:
                            HtmlParsingMode = HtmlParsingMode.Fragment;
                            HtmlParsingOptions = HtmlParsingOptions.AllowSelfClosingTags;
                            break;
                    }
                }

                if (HtmlParsingMode == HtmlParsingMode.Fragment) 
                {
                    FragmentContext = ctx;
                }
            }
            


            //if (HtmlParsingMode == HtmlParser.HtmlParsingMode.Fragment)
            //{
            //    source = new StringReader(HtmlPreprocessor.ExpandSelfClosingTags(source.ReadToEnd()));
            //}

            Reset();
            Tokenize(source);

            return treeBuilder.Document;
        }

        #endregion

        #region private methods

        private static HtmlParsingOptions MergeOptions(HtmlParsingOptions options)
        {
            if (options.HasFlag(HtmlParsingOptions.Default))
            {
                return CsQuery.Config.HtmlParsingOptions | options & ~(HtmlParsingOptions.Default);
            }
            else
            {
                return options;
            }
        }
        private static DocType GetDocType(DocType docType)
        {
            return docType == DocType.Default ? Config.DocType : docType;
        }
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
                    treeBuilder.SetFragmentContext(FragmentContext);
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
        /// Gets a context by inspecting the beginning of a stream. Will restore the stream to its
        /// unaltered state.
        /// </summary>
        ///
        /// <param name="reader">
        /// The HTML input.
        /// </param>
        /// <param name="context">
        /// [out] The context (e.g. the valid parent of the first tag name found).
        /// </param>
        ///
        /// <returns>
        /// The a new TextReader which is a clone of the original.
        /// </returns>

        private TextReader GetContextFromStream(TextReader reader, out string context)
        {
            
            int pos = 0;
            string tag = "";
            string readSoFar = "";
            int mode=0;
            char[] buf = new char[1];
            bool finished = false;

            while (!finished && reader.Read(buf,0,1)>0)
            {
                
                char cur = buf[0];
                readSoFar += cur;

                switch(mode) {
                    case 0:
                        if (cur=='<') {
                            mode=1;
                        }
                        break;
                    case 1:
                        if (CharacterData.IsType(cur, CharacterType.HtmlTagOpenerEnd))
                        {
                            finished = true;
                            break;
                        }
                        tag += cur;
                        break;
                }
                pos++;
            }
            context = GetContext(tag);
            return new TextReaderCombiner(new StringReader(readSoFar), reader);
        }

        private void InitializeTreeBuilder()
        {
            treeBuilder = new CsQueryTreeBuilder();

            treeBuilder.NamePolicy = XmlViolationPolicy.Allow;
            treeBuilder.WantsComments = !HtmlParsingOptions.HasFlag(HtmlParsingOptions.IgnoreComments);
            treeBuilder.AllowSelfClosingTags = HtmlParsingOptions.HasFlag(HtmlParsingOptions.AllowSelfClosingTags);
            
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

            ConfigureTreeBuilderForParsingMode();
            tokenizer.Start();

            bool swallowBom = true;


            try
            {
                char[] buffer = new char[2048];
                UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
                bool lastWasCR = false;
                int len = -1;
                if ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
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
            
            SetDefaultContext("html", "document");
            SetDefaultContext("!doctype", "document");
            SetDefaultContext("body", "html");

        }
        #endregion

    }
}
