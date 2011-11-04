using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Jtc.CsQuery
{
    
    // unsafe struct Token {
    //    public Token(string tokenName)
    //    {
    //        length = (byte)Math.Min(tokenName.Length, 20);
    //        if (length == 20)
    //        {
    //            name = tokenName.Substring(0, 20).ToCharArray();
    //            //copyToFixed(tokenName.Substring(0, 20));
    //        } else {
    //            name = tokenName.ToCharArray();
    //            //copyToFixed(tokenName);
    //        }
            
    //    }
    //    public string Name() {
    //        //return _Name.ToString().Trim();
    //        string result="";
    //        //fixed (char* pName = name)
    //        //{
    //        //    for (int i = 0; i < 20; i++)
    //        //    {
    //        //        result += *((char*)pName);
    //        //    }
    //        //}
          
    //        return result;
    //    }
    //    private void copyToFixed(string source)
    //    {
    //        int len = source.Length;
    //        if (len > 20)
    //        {
    //            throw new Exception("Length of source >20 for a token. Bad.");
    //        }

    //        fixed (char* pName = name) {
    //            IntPtr ptr = (IntPtr)pName;
    //            Marshal.Copy(source.ToCharArray(), 0,ptr,len);
    //        }
            
            
    //    }
    //    public char[] NameAsCharArray()
    //    {
    //        char[] result = new char[length];
    //        fixed (char* pName = name)
    //        {
    //            for (int i = 0; i < length; i++)
    //            {
    //                result[i] += *((char*)pName);
    //            }
    //        }
    //        return result;
    //    }
        
    //    //fixed char name[20];
    //    char[] name;
    //    byte length;
    //}
    public static class DomData
    {
        public const short StyleNodeId = 0;
        public const short ClassAttrID = 1;
        public const short ValueNodeId=2;
        public const short IDNodeId=3;
        public const short SelectedAttrId=4;
        public const short ReadonlyAttrId=5;
        public const short CheckedAttrId=6;
        
        public const short InputNodeId=10;

        static DomData()
        {
            // "input" is hardcoded
            HashSet<string> noInnerHtmlAllowed = new HashSet<string>(new string[]{
            "base","basefont","frame","link","meta","area","col","hr","param","script","textarea",
                "img","br", "!doctype","!--"
            });
    
            TokenIDs = new Dictionary<string, short>();
            TokenID("style"); //0
            TokenID("class"); //1
            // inner text allowed
            TokenID("value"); //2
            TokenID("id"); //3
            TokenID("selected"); //4
            TokenID("readonly"); //5
            TokenID("checked"); //6

            noInnerHtmlIDFirst = nextID;
            TokenID("script"); //7
            TokenID("textarea"); //8
            TokenID("input"); //9
            
            // no inner html allowed
            
            foreach (string tag in noInnerHtmlAllowed)
            {
                TokenID(tag);
            }
            noInnerHtmlIDLast = (short)(nextID - 1);
        }

        private static short noInnerHtmlIDFirst;
        private static short noInnerHtmlIDLast;
        


        const int maxSize = 1000;
        private static short nextID=0;
        //private static Token[]  Tokens = new Token[maxSize];
        //private static List<Token> Tokens = new List<Token>();
        //private static char[][] Tokens = new char[1000][];
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
        public static bool NoInnerHtmlAllowed(short nodeId)
        {
            return nodeId >= noInnerHtmlIDFirst &&
                nodeId <= noInnerHtmlIDLast;
        }
        public static bool InnerTextAllowed(short nodeId)
        {
            return nodeId == 7 || nodeId == 8 || !NoInnerHtmlAllowed(nodeId );
        }
        
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
        public static string TokenName(short id)
        {
            return id < 0 ? "" : Tokens[id];
        }
    }
}
