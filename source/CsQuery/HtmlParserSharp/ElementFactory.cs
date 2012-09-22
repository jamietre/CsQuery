using System;
using System.IO;
using System.Collections.Generic;
using HtmlParserSharp.Core;
using HtmlParserSharp.Common;
using CsQuery;
using CsQuery.Utility;
using CsQuery.StringScanner;
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
            DefaultContext = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            SetDefaultContext("tbody,thead", "table");
            SetDefaultContext("tr", "tbody");
            SetDefaultContext("td,th", "tr");
        }


        #endregion

        #region static methods

        public static IDomDocument Create(string html, HtmlParsingMode mode=HtmlParsingMode.Content) {
            using (var reader = new StringReader(html)) {
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
        private bool autoSetContext = false;
        private HtmlParsingMode HtmlParsingMode;

        #endregion

        #region public methods

        public IDomDocument Parse(TextReader reader, HtmlParsingMode mode=HtmlParsingMode.Document)
        {
            HtmlParsingMode = mode;
            Reset();

            switch (mode)
            {
                
                case HtmlParsingMode.Document:
                    break;
                case HtmlParsingMode.Content:
                    treeBuilder.SetFragmentContext("body");
                    break;
                case HtmlParsingMode.Fragment:
                    //treeBuilder.SetFragmentContext("body");
                    autoSetContext = true;
                    break;
                default:
                    throw new NotImplementedException("Unknown parsing mode.");
            }
            Tokenize(reader);

            return treeBuilder.Document;
        }

        #endregion

        #region private methods

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

        private void Reset()
        {
            treeBuilder = new DomTreeBuilder();
            tokenizer = new Tokenizer(treeBuilder, false);
            treeBuilder.IsIgnoringComments = false;
            treeBuilder.DoctypeExpectation = DoctypeExpectation.Auto;

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

            if (!autoSetContext)
            {
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
                    if (autoSetContext)
                    {
                        string ctx = GetContext(buffer);
                        treeBuilder.SetFragmentContext( ctx);
                        autoSetContext = false;
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
        #endregion

    }
}
