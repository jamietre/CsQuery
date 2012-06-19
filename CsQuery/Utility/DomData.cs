using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CsQuery.Utility
{
    /// <summary>
    /// Utility functions 
    /// 
    /// </summary>
    public static class DomData
    {
        #region contstants and data

        // when false, will use binary data for the character set

#if DEBUG_PATH
        public static bool Debug = true;
        public const int pathIdLength = 3;
        public const char indexSeparator = '>';
#else
        public static bool Debug = false;
        /// <summary>
        /// Length of each node's path ID (in characters), sets a limit on the number of child nodes before a reindex
        /// is required. For most cases, a small number will yield better performance. In production we probably can get
        /// away with just 1 (meaning a char=65k possible values). 
        /// </summary>
        public const int pathIdLength = 1;
        /// <summary>
        /// The character used to separate the unique part of an index entry from its path. When debugging
        /// it is useful to have a printable character. Otherwise we want something that is guaranteed to be
        /// a unique stop character.
        /// </summary>
        public const char indexSeparator = (char)1;
#endif
        

        /// Hardcode some token IDs to improve performance of things that are referred to often
        
        //public const ushort StyleAttrId = 2;
        public const ushort ClassAttrId = 3;
        public const ushort ValueAttrId=4;
        public const ushort IDAttrId=5;
        
        public const ushort tagSCRIPT= 6;
        public const ushort tagTEXTAREA= 7;
        public const ushort tagSTYLE = 8;
        public const ushort tagINPUT = 9;
        public const ushort tagSELECT = 10;
        public const ushort tagOPTION = 11;
        public const ushort tagP = 12;
        public const ushort tagTR=13;
        public const ushort tagTD = 14;
        public const ushort tagTH = 15;
        public const ushort tagHEAD = 16;
        public const ushort tagBODY = 17;
        public const ushort tagDT = 18;
        public const ushort tagCOLGROUP = 19;
        public const ushort tagDD = 20;
        public const ushort tagLI = 21;
        public const ushort tagDL = 22;
        public const ushort tagTABLE = 23;
        public const ushort tagOPTGROUP = 24;
        public const ushort tagUL = 25;
        public const ushort tagOL = 26;
        public const ushort tagTBODY = 27;
        public const ushort tagTFOOT = 28;
        public const ushort tagTHEAD = 29;

        public static ushort SelectedAttrId;
        public static ushort ReadonlyAttrId;
        public static ushort CheckedAttrId;
        
        // HTML spec for whitespace
        // U+0020 SPACE, U+0009 CHARACTER TABULATION (tab), U+000A LINE FEED (LF), U+000C FORM FEED (FF), and U+000D CARRIAGE RETURN (CR).

        public static char[] Whitespace = new char[] { '\x0020', '\x0009', '\x000A', '\x000C', '\x000D' };
        
        // U+0022 QUOTATION MARK characters ("), U+0027 APOSTROPHE characters ('), U+003D EQUALS SIGN characters (=), 
        // U+003C LESS-THAN SIGN characters (<), U+003E GREATER-THAN SIGN characters (>), or U+0060 GRAVE ACCENT characters (`),
        // and must not be the empty string.}
        
        public static char[] MustBeQuoted = new char[] { '/','\x0022', '\x0027', '\x003D', '\x003C', '\x003E', '\x0060' };
        public static char[] MustBeQuotedAll;

        // Things that can be in a CSS number

        public static HashSet<char> NumberChars = new HashSet<char>("-+0123456789.,");

        // Things that are allowable unit strings in a CSS style.

        public static HashSet<string> Units = new HashSet<string>(new string[] { "%", "in", "cm", "mm", "em", "ex", "pt", "pc", "px" });

        /// <summary>
        /// Fields used internally
        /// </summary>

        private static ushort nextID = 2;
        private static List<string> Tokens = new List<string>();
        private static Dictionary<string, ushort> TokenIDs;
        private static object locker = new Object();
        
        // Constants for path encoding functions

        private static string defaultPadding;
        
        // The character set used to generate path IDs
        // For production use, this is replaced with a string of all ansi characters

        private static char[] baseXXchars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToArray();
        private static int encodingLength; // set in constructor
        private static int maxPathIndex;

        // The length of each path key - this sets an upper limit on the number of subelements
        // that can be added without reindexing the node

        private static HashSet<ushort> TagsNoInnerHtmlAllowed;
        private static HashSet<ushort> TagsBlockElements;
        private static HashSet<ushort> TagsBooleanAttributes;

        #endregion

        #region constructor

        static DomData()
        {
            // For path encoding - when in production mode use a single character value for each path ID. This lets us avoid doing 
            // a division for indexing to determine path depth, and is just faster for a lot of reasons. This should be plenty
            // of tokens: things that are tokenized are tag names style names, class names, attribute names (not values), and ID 
            // values. You'd be hard pressed to exceed this limit on a single web page. Famous last words right? 

            #if !DEBUG_PATH
                baseXXchars = new char[65533];
                for (ushort i = 0; i < 65533; i++)
                {
                    baseXXchars[i] = (char)(i+2);
                }
            #endif

            encodingLength = baseXXchars.Length;
            defaultPadding="";
            for (int i = 1; i < pathIdLength; i++)
            {
                defaultPadding=defaultPadding+"0";
            }
            maxPathIndex = (int)Math.Pow(encodingLength,pathIdLength) -1;

            MustBeQuotedAll = new char[Whitespace.Length + MustBeQuoted.Length];
            MustBeQuoted.CopyTo(MustBeQuotedAll, 0);
            Whitespace.CopyTo(MustBeQuotedAll, MustBeQuoted.Length);

            HashSet<string> noInnerHtmlAllowed = new HashSet<string>(new string[]{
                "BASE","BASEFONT","FRAME","LINK","META","AREA","COL","HR","PARAM","SCRIPT","TEXTAREA","STYLE",
                    "IMG","INPUT","BR", "!DOCTYPE","!--", "COMMAND", "EMBED","KEYGEN","SOURCE","TRACK","WBR"
            });
    
            // these elements will cause certain tags to be closed automatically; this is very important for layout.

            // 6-19-2012: removed "object" - object is inline.

            HashSet<string> blockElements = new HashSet<string>(new string[]{"BODY","BR","ADDRESS","BLOCKQUOTE","CENTER","DIV","DIR","FORM","FRAMESET","H1","H2","H3","H4","H5","H6","HR",
                "ISINDEX","LI","NOFRAMES","NOSCRIPT","OL","P","PRE","TABLE","TR","TEXTAREA","UL",
                // html5 additions
                "ARTICLE","ASIDE","BUTTON","CANVAS","CAPTION","COL","COLGROUP","DD","DL","DT","EMBED","FIELDSET","FIGCAPTION",
                "FIGURE","FOOTER","HEADER","HGROUP","PROGRESS","SECTION","TBODY","THEAD","TFOOT","VIDEO",
                // random
                "APPLET","LAYER","LEGEND"
            });

            HashSet<string> booleanAttributes = new HashSet<string>(new string[] {
            "autobuffer", "autofocus", "autoplay", "async", "checked", "compact", "controls", "declare", "defaultmuted", "defaultselected",
            "defer", "disabled", "draggable", "formNoValidate", "hidden", "indeterminate", "ismap", "itemscope","loop", "multiple",
            "muted", "nohref", "noresize", "noshade", "nowrap", "novalidate", "open", "pubdate", "readonly", "required", "reversed",
            "scoped", "seamless", "selected", "spellcheck", "truespeed"," visible"
            });

         
            
            

            TokenIDs = new Dictionary<string, ushort>();
            // where Style used to be
            TokenID("unused",true); //2

            TokenID("class",true); //3
            // inner text allowed
            TokenID("value",true); //4
            TokenID("id",true); //5

            //noInnerHtmlIDFirst = nextID;
            // the node types that have inner content which is not parsed as HTML ever
            TokenID("script",true); //6
            TokenID("textarea",true); //7
            TokenID("style",true); //8


            TokenID("input",true); //9
            TokenID("select", true); //10
            TokenID("option", true); //11

            TokenID("p", true); //12
            TokenID("tr", true); //13
            TokenID("td", true); //14
            TokenID("th", true); //15
            TokenID("head", true); //16
            TokenID("body", true); //17
            TokenID("dt", true); //18
            TokenID("colgroup", true); //19
            TokenID("dd", true); //20
            TokenID("li", true); //21
            TokenID("dl", true); //22
            TokenID("table", true); //23
            TokenID("optgroup", true); //24
            TokenID("ul", true); //25
            TokenID("ol", true); //26
            TokenID("tbody", true); //27
            TokenID("tfoot", true); //28
            TokenID("thead", true); //29
            
            if (nextID !=  30)
            {
                throw new InvalidOperationException("Something went wrong with the constant map in DomData");
            }
            
            SelectedAttrId= TokenID("selected",true); 
            ReadonlyAttrId = TokenID("readonly",true); 
            CheckedAttrId = TokenID("checked",true); 

            TagsNoInnerHtmlAllowed = PopulateTokenHashset(noInnerHtmlAllowed);
            TagsBooleanAttributes=PopulateTokenHashset(booleanAttributes);
            TagsBlockElements=PopulateTokenHashset(blockElements);

        }
        private static HashSet<ushort> PopulateTokenHashset(IEnumerable<string> tokens)
        {
            var set = new HashSet<ushort>();
            foreach (var item in tokens)
            {
                set.Add(TokenID(item,true));
            }
            return set;
        }
        #endregion


        #region public methods

        /// <summary>
        /// A list of all keys (tokens) created
        /// </summary>
        public static IEnumerable<string> Keys
        {
            get
            {
                return Tokens;
            }
        }

        /// <summary>
        /// This type does not allow HTML children. Some of these types may allow text but not HTML.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool NoInnerHtmlAllowed(ushort nodeId)
        {
            return TagsNoInnerHtmlAllowed.Contains(nodeId);
        }

        /// <summary>
        /// This type does not allow HTML children. Some of these types may allow text but not HTML.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool NoInnerHtmlAllowed(string nodeName)
        {
            return NoInnerHtmlAllowed(TokenID(nodeName,true));
        }

        /// <summary>
        /// Text is allowed within this node type. Is includes all types that also permit HTML.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool InnerTextAllowed(ushort nodeId)
        {
            return nodeId == tagSCRIPT || nodeId == tagTEXTAREA ||  nodeId == tagSTYLE || !NoInnerHtmlAllowed(nodeId);
        }
        public static bool InnerTextAllowed(string nodeName)
        {
            return InnerTextAllowed(TokenID(nodeName,true));
        }
        public static bool IsBlock(ushort nodeId)
        {
            return TagsBlockElements.Contains(nodeId);
        }
        public static bool IsBlock(string nodeName)
        {
            return IsBlock(TokenID(nodeName,true));
        }
        /// <summary>
        /// The attribute is a boolean type
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool IsBoolean(ushort nodeId)
        {
            return TagsBooleanAttributes.Contains(nodeId);
        }
        /// <summary>
        /// The attribute is a boolean type
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static bool IsBoolean(string nodeName)
        {
            return IsBoolean(TokenID(nodeName,true));
        }
        /// <summary>
        /// Return a token ID for a name, adding to the index if it doesn't exist.
        /// When indexing tags and attributes, ignoreCase should be used
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static ushort TokenID(string tokenName, bool ignoreCase = false)
        {
            ushort id;
            if (String.IsNullOrEmpty(tokenName))
            {
                return 0;
            }
            if (ignoreCase) {
                tokenName = tokenName.ToLower();
            }

            if (!TokenIDs.TryGetValue(tokenName, out id))
            {
                
                lock(locker) {
                    if (!TokenIDs.TryGetValue(tokenName, out id))
                    {
                        Tokens.Add(tokenName);
                        TokenIDs.Add(tokenName, nextID);
                        // if for some reason we go over 65,535, will overflow and crash. no need 
                        // to check
                        id = nextID++;
                    }
                }
            }
            return id;
        }
        /// <summary>
        /// Return a token name for an ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string TokenName(ushort id)
        {
            return id <= 0 ? "" : Tokens[id-2];
        }

       

        /// <summary>
        /// Encode to base XX (defined in constants)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string BaseXXEncode(int number)
        {

            // optimize for small numbers - this should mostly eliminate the ineffieciency of base62
            // encoding while giving benefits for storage space

            if (number < encodingLength)
            {
                return defaultPadding +baseXXchars[number];
            }

            if (number > maxPathIndex)
            {
                throw new OverflowException("Maximum number of child nodes (" + maxPathIndex + ") exceeded."); 
            }
            string sc_result = "";
            int num_to_encode = number;
            int i = 0;
            do
            {
                i++;
                sc_result = baseXXchars[(num_to_encode % encodingLength)] + sc_result;
                num_to_encode = ((num_to_encode - (num_to_encode % encodingLength)) / encodingLength);
                
            }
            while (num_to_encode != 0);
            
            return sc_result.PadLeft(pathIdLength, '0');
        }

        /// <summary>
        /// HtmlEncode the string (pass-thru to system; abstracted in case we want to change)
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(string html)
        {
            return System.Web.HttpUtility.HtmlEncode(html);

        }
        public static string HtmlDecode(string html)
        {
            return System.Web.HttpUtility.HtmlDecode(html);
        }


        /// <summary>
        /// Encode text as part of an attribute
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AttributeEncode(string text)
        {
            string quoteChar;
            string attribute = AttributeEncode(text,
                CQ.DefaultDomRenderingOptions.HasFlag(DomRenderingOptions.QuoteAllAttributes),
                out quoteChar);
            return quoteChar + attribute + quoteChar;
        }
        
        /// <summary>
        /// Htmlencode a string, except for double-quotes, so it can be enclosed in single-quotes
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AttributeEncode(string text, bool alwaysQuote, out string quoteChar)
        {
            if (text == "")
            {
                quoteChar = "\"";
                return "";
            }

            bool hasQuotes = text.IndexOf("\"") >= 0;
            bool hasSingleQuotes = text.IndexOf("'") >= 0;
            string result = text;
            if (hasQuotes || hasSingleQuotes)
            {

                //un-encode quotes or single-quotes when possible. When writing the attribute it will use the right one
                if (hasQuotes && hasSingleQuotes)
                {
                    result = result.Replace("'", "&#39;");
                    quoteChar = "\'";
                }
                else if (hasQuotes)
                {
                    quoteChar = "'";
                }
                else
                {
                    quoteChar = "\"";
                }
            }
            else
            {
                if (alwaysQuote)
                {
                    quoteChar = "\"";
                }
                else
                {
                    quoteChar = result.IndexOfAny(Utility.DomData.MustBeQuotedAll) >= 0 ? "\"" : "";
                }
            }

            return result;
        }

        #endregion

    }
}
