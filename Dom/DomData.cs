using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Jtc.CsQuery
{
    public static class DomData
    {
        /// <summary>
        /// Hardcode some token IDs to improve performance for frequent lookups
        /// </summary>
        public const short StyleAttrId = 0;
        public const short ClassAttrId = 1;
        public const short ValueAttrId=2;
        public const short IDAttrId=3;
        public const short ScriptNodeId = 4;
        public const short TextareaNodeId = 5;
        public const short InputNodeId = 6;

        public static short SelectedAttrId;
        public static short ReadonlyAttrId;
        public static short CheckedAttrId;

        private static short noInnerHtmlIDFirst;
        private static short noInnerHtmlIDLast;
        private static short booleanFirst;
        private static short booleanLast;
        private static short blockFirst;
        private static short blockLast;

        static DomData()
        {
            
            HashSet<string> noInnerHtmlAllowed = new HashSet<string>(new string[]{
            "base","basefont","frame","link","meta","area","col","hr","param","script","textarea",
                "img","input","br", "!doctype","!--"
            });
    
            HashSet<string> blockElements = new HashSet<string>(new string[]{"body","br","address","blockquote","center","div","dir","form","frameset","h1","h2","h3","h4","h5","h6","hr",
                "isindex","li","noframes","noscript","object","ol","p","pre","table","tr","textarea","ul",
                // html5 additions
                "article","aside","button","canvas","caption","col","colgroup","dd","dl","dt","embed","fieldset","figcaption",
                "figure","footer","header","hgroup","object","progress","section","tbody","thead","tfoot","video"
            });

            HashSet<string> booleanAttributes = new HashSet<string>(new string[] {
            "autobuffer", "autofocus", "autoplay", "async", "checked", "compact", "controls", "declare", "defaultmuted", "defaultselected",
            "defer", "disabled", "draggable", "formNoValidate", "hidden", "indeterminate", "ismap", "itemscope","loop", "multiple",
            "muted", "nohref", "noresize", "noshade", "nowrap", "novalidate", "open", "pubdate", "readonly", "required", "reversed",
            "scoped", "seamless", "selected", "spellcheck", "truespeed"," visible"
            });

            TokenIDs = new Dictionary<string, short>();
            TokenID("style"); //0
            TokenID("class"); //1
            // inner text allowed
            TokenID("value"); //2
            TokenID("id"); //3

            noInnerHtmlIDFirst = nextID;
            TokenID("script"); //4
            TokenID("textarea"); //5
            TokenID("input"); //6
            
            // no inner html allowed
            
            foreach (string tag in noInnerHtmlAllowed)
            {
                TokenID(tag);
            }
            noInnerHtmlIDLast = (short)(nextID - 1);
            booleanFirst = (short)nextID;
            foreach (string tag in booleanAttributes)
            {
                TokenID(tag);
            }
            SelectedAttrId= TokenID("selected"); 
            ReadonlyAttrId = TokenID("readonly"); 
            CheckedAttrId = TokenID("checked"); 
            booleanLast = (short)(nextID - 1);
            blockFirst = (short)nextID;
            foreach (string tag in blockElements)
            {
                TokenID(tag);
            }
            blockLast = (short)(nextID - 1);
        }

        const int maxSize = 1000;
        private static short nextID=0;

        private static List<string> Tokens = new List<string>();
        
        private static Dictionary<string, short> TokenIDs;
        private static object locker=new Object();
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
        public static bool NoInnerHtmlAllowed(short nodeId)
        {
            return nodeId >= noInnerHtmlIDFirst &&
                nodeId <= noInnerHtmlIDLast;
        }
        public static bool NoInnerHtmlAllowed(string nodeName)
        {
            return NoInnerHtmlAllowed(TokenID(nodeName));
        }
        /// <summary>
        /// Text is allowed within this node type. Is includes all types that also permit HTML.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool InnerTextAllowed(short nodeId)
        {
            return nodeId == ScriptNodeId || nodeId == TextareaNodeId || !NoInnerHtmlAllowed(nodeId);
        }
        public static bool InnerTextAllowed(string nodeName)
        {
            return InnerTextAllowed(TokenID(nodeName));
        }
        public static bool IsBlock(short nodeId)
        {
            return nodeId >= blockFirst
                && nodeId <= blockLast;
        }
        public static bool IsBlock(string nodeName)
        {
            return IsBlock(TokenID(nodeName));
        }
        /// <summary>
        /// The attribute is a boolean type
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool IsBoolean(short nodeId)
        {
            return nodeId >= booleanFirst && nodeId <= booleanLast;
        }
        /// <summary>
        /// The attribute is a boolean type
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static bool IsBoolean(string nodeName)
        {
            return IsBoolean(TokenID(nodeName));
        }
        /// <summary>
        /// Return a token ID for a name, adding to the index if it doesn't exist.
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="toLower"></param>
        /// <returns></returns>
        public static short TokenID(string tokenName, bool toLower = true)
        {
            short id;
            if (toLower) {
                tokenName = tokenName.ToLower();
            }

            if (!TokenIDs.TryGetValue(tokenName, out id))
            {
                
                lock(locker) {
                    //Tokens[nextID] = new Token(tokenName);
                    Tokens.Add(tokenName);
                    //Tokens.SetValue(tokenName.ToCharArray(), nextID);

                    TokenIDs.Add(tokenName, nextID);
                    id = nextID++;
                }
            }
            return id;
        }
        /// <summary>
        /// Return a token name for an ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string TokenName(short id)
        {
            return id < 0 ? "" : Tokens[id];
        }

    }
}
